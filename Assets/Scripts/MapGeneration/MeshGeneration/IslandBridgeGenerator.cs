using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexGridUtils;

public class IslandBridgeGenerator {
	private static AxialCoordinates[,] _bridgesAnchorCells;
	private static GameObject _bridgeTemplate;
    private static Dictionary<AxialCoordinates, int> _bridgeCellsIndexDict;
	private static List<Vector3> _bridgeVertices = new List<Vector3>();
	private static List<int> _bridgeTriangles = new List<int>();
	private static List<Vector2> _bridgeUVs = new List<Vector2>();
	private static int _bridgeSize = 8;

	private enum BridgesDir { 
        BottomRight = 0,
        Bottom = 1,
        BottomLeft = 2, 
        TopLeft = 3, 
        Top = 4, 
        TopRight = 5, 
    };

#region Initialization
	public IslandBridgeGenerator(){
		FetchBridgeAnchorCells();
	}

	private static void FetchBridgeAnchorCells(){ 
		if (_bridgesAnchorCells != null) return; // if already initialized, return
		_bridgesAnchorCells = new AxialCoordinates[6, _bridgeSize]; // 6 faces of the hexagon

		int anchorMidCellIndexOffset = GetIslandCellsNumber(HexMetrics.IslandRadius - 1) // index of the first cell in the last ring
                                            + HexMetrics.IslandRadius / 2; // offset to get the middle cell from the bot Right side
		
		for (int i = 0; i < 6; i++){ // iterate through the 6 sides
			for (int j = 0; j < _bridgeSize; j++){ // iterate through the cells of the bridge anchored to the island
				if (HexMetrics.IslandRadius %2 == 0){ // if radius is even, the last ring side have an odd number of cells, the center is a unique cell
					int anchorMidCellIndex = anchorMidCellIndexOffset + i * HexMetrics.IslandRadius; // the offset to get other mid cells is the previous + radius
					AxialCoordinates cellCoord = IslandCellsPositions[anchorMidCellIndex + j - _bridgeSize/2]; // we offset by half the bridgesize to get the cell at the left and at the right of the mid cell
					_bridgesAnchorCells[i,j] = cellCoord;
					
				}else
				{ //TODO, handle case when island radius is odd and the center is composed of 2 cells

				}
			}
		}
    }
	private void GenerateBridgeTemplate(Material bridgeMaterial){
		if (_bridgeTemplate != null) return; // if the template already exists, return
		_bridgeTemplate = new GameObject("Bridge Template");

		PopulateBridgeCellHashSet(_bridgeSize);

		MeshRenderer meshRenderer = _bridgeTemplate.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = bridgeMaterial;
		MeshFilter meshFilter = _bridgeTemplate.AddComponent<MeshFilter>();

		TriangulateBridgeTopFaces();
		TriangulateSideFaces();
		GenerateBridgeUVs();

		Mesh mesh = new Mesh();
		mesh.vertices = _bridgeVertices.ToArray();
		mesh.triangles = _bridgeTriangles.ToArray();
		mesh.uv = _bridgeUVs.ToArray();
		mesh.RecalculateNormals();
		mesh.RecalculateTangents();

		meshFilter.mesh = mesh;
	}

	private void PopulateBridgeCellHashSet(int bridgeSize){
		_bridgeCellsIndexDict = new Dictionary<AxialCoordinates, int>();
		int cellIndex = 0;
		for (int z = 0; z < HexMetrics.DistanceBetweenIslands; z++){
			AxialCoordinates midCellPos = new AxialCoordinates(-z/2, z+1); // get the middle cell, -z/2 to avoid the offset, 
			int offset = bridgeSize/2; // we place cells from left to right of the middle cell
			for (int x = 0; x < bridgeSize; x++){
				bool isEvenRow = z % 2 == 0;
				if (x == 0 && isEvenRow) continue; // skip the first cell in even rows
				AxialCoordinates cellPos = midCellPos + GetDir[(int)Dir.Right] * (x - offset);
				_bridgeCellsIndexDict.Add(cellPos, cellIndex++);
			}
		}
	}
#endregion
    public void GenerateIslandBridges(Island currentIsland, Material bridgeMaterial){
		GenerateBridgeTemplate(bridgeMaterial);
        for (int i = 0; i < 3; i++){
			Island targetIsland = GetNeighborIsland(currentIsland, (BridgesDir)i);
			if (targetIsland == null) continue; // if there is no island in this direction, skip this iteration
			
			AxialCoordinates bridgeAnchorStartCell = _bridgesAnchorCells[i, _bridgeSize/2];
			// AxialCoordinates bridgeAnchorEndCell = _bridgesAnchorCells[(i + 3) % 6]; // get the opposite bridge anchor cell
			Vector3 anchorWorldPos = CellToWorld(bridgeAnchorStartCell);
			Vector3 bridgePosition = currentIsland.transform.position + anchorWorldPos;

			Quaternion rotation = Quaternion.LookRotation(anchorWorldPos, Vector3.up);
			GameObject bridge = Island.Instantiate(_bridgeTemplate, bridgePosition, rotation, currentIsland.transform);
			currentIsland.Bridges[i] = bridge;
			bridge.name = string.Format("Bridge ({0},{1})", currentIsland.IslandX, currentIsland.IslandZ);

			Mesh mesh = bridge.GetComponent<MeshFilter>().mesh;
			// float startHeight = currentIsland.GetCellHeightMapValue(bridgeAnchorStartCell);
			// float endHeight = targetIsland.GetCellHeightMapValue(bridgeAnchorEndCell);
			mesh.vertices = UpdateBridgeVerticesHeight(currentIsland, targetIsland, mesh.vertices, (BridgesDir)i);
		}
    }

    private Island GetNeighborIsland(Island currentIsland, BridgesDir dir)
    {
		AxialCoordinates islandCoord;
        switch (dir){
			case BridgesDir.BottomRight: 
				islandCoord = new AxialCoordinates(currentIsland.IslandX + 1, currentIsland.IslandZ -1);
				break;
			case BridgesDir.Bottom:
				islandCoord = new AxialCoordinates(currentIsland.IslandX, currentIsland.IslandZ - 1);
				break;
			case BridgesDir.BottomLeft:
				islandCoord = new AxialCoordinates(currentIsland.IslandX - 1, currentIsland.IslandZ);
				break;
			default :
				return null;
		}
		if (!MapManager.Instance.IslandDict.ContainsKey(islandCoord)) return null;
		return MapManager.Instance.IslandDict[islandCoord];
    }

#region Triangulation
	private void TriangulateBridgeTopFaces(){
		foreach (AxialCoordinates cell in _bridgeCellsIndexDict.Keys)
		{
			Vector3 cellWorlPos = CellToWorld(cell);
			TriangulateCellTopFace(cellWorlPos);
		}
	}

	private void TriangulateCellTopFace(Vector3 cellWorldPos){
		Vector3[] corners = HexMetrics.corners;
		int indexOffset = _bridgeVertices.Count;
		for (int i = 0; i < 6; i++) {
			_bridgeVertices.Add(cellWorldPos + corners[i]);				
		}

		AddTriangle(indexOffset + 0, indexOffset + 1, indexOffset + 5);
		AddTriangle(indexOffset + 1, indexOffset + 4, indexOffset + 5);	
		AddTriangle(indexOffset + 1, indexOffset + 2, indexOffset + 4);
		AddTriangle(indexOffset + 2, indexOffset + 3, indexOffset + 4);
	}

	private void TriangulateSideFaces(){
		for (int i = 0; i < _bridgeCellsIndexDict.Count; i++){
			AxialCoordinates cellCoord = _bridgeCellsIndexDict.Keys.ElementAt(i);
			// for each cell we check neighbours in 3 directions, and triangulate between current cell and neighbour cells side faces
			// so the 3 other directions will be triangulated by the neighbour cells not checked by the current cell
			// if there are no neigbours in one of the 3 other directions, we also triangulate this direction as no other cell would do it. this will handle the borders
			int vertexIndex = i * 6; // each cell has 6 vertices
			for (int j = 0; j < 6; j++) {
				AxialCoordinates targetCell = cellCoord + GetDir[j];
				switch ((Dir)j) 
				{
					case Dir.BottomLeft: TriangulateBotLeftSide(i, vertexIndex); break;
					case Dir.BottomRight: TriangulateBotRightSide(i, vertexIndex); break;
					case Dir.Left: TriangulateLeftSide(i, vertexIndex); break;
					case Dir.Right: 
						if (!_bridgeCellsIndexDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateRightSide(i, vertexIndex); break;
					case Dir.TopLeft: 
						if (!_bridgeCellsIndexDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopLeftSide(i, vertexIndex); break;
					case Dir.TopRight: 
						if (!_bridgeCellsIndexDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopRightSide(i, vertexIndex); break;
				}
			}
		}	
	}

	private static void TriangulateBotLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(4,3,1);
		Vector3Int secondTriIndices = new Vector3Int(4,1,0);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomLeft);
	}

	private static void TriangulateLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(5,4,2);
		Vector3Int secondTriIndices = new Vector3Int(5,2,1);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Left);
	}

	private static void TriangulateTopLeftSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(0,5,3);
		Vector3Int secondTriIndices = new Vector3Int(0,3,2);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopLeft);
	}

	private static void TriangulateTopRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(1,0,4);
		Vector3Int secondTriIndices = new Vector3Int(1,4,3);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.TopRight);
	}

	private static void TriangulateRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(2,1,5);
		Vector3Int secondTriIndices = new Vector3Int(2,5,4);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.Right);
	}

	private static void TriangulateBotRightSide(int cellIndex, int vertexIndex){
		Vector3Int firstTriIndices = new Vector3Int(3,2,0);
		Vector3Int secondTriIndices = new Vector3Int(3,0,5);
		TriangulateSide(cellIndex, vertexIndex, firstTriIndices, secondTriIndices, HexGridUtils.Dir.BottomRight);
	}


	private static void TriangulateSide(int cellIndex, int vertexIndex, Vector3Int firstTriIndices, Vector3Int secondTriIndices, Dir dir) {
		AxialCoordinates sideCellPos = _bridgeCellsIndexDict.Keys.ElementAt(cellIndex) + GetDir[(int)dir];
		
		// Check if neighboring cell exists
		if (_bridgeCellsIndexDict.ContainsKey(sideCellPos)) {
			// Neighboring cell exists, proceed with existing vertices
			int sideCellIndex = _bridgeCellsIndexDict[sideCellPos];
			int sideCellVertexIndex = sideCellIndex * 6;

			AddTriangle(
				vertexIndex + firstTriIndices.x, 
				vertexIndex + firstTriIndices.y, 
				sideCellVertexIndex + firstTriIndices.z);
				
			AddTriangle(
				vertexIndex + secondTriIndices.x, 
				sideCellVertexIndex + secondTriIndices.y, 
				sideCellVertexIndex + secondTriIndices.z);
		} else {
			// Neighbor cell does not exist, create border vertices and triangles
			// Vector3[] corners = HexMetrics.corners;
			
			// Get the local positions of the required vertices for the side faces
			Vector3 borderVertex1 = _bridgeVertices[vertexIndex + firstTriIndices.x] + new Vector3(0,-1,0);//corners[firstTriIndices.z];
			Vector3 borderVertex2 = _bridgeVertices[vertexIndex + firstTriIndices.y] + new Vector3(0,-1,0); //corners[secondTriIndices.z];

			// Add border vertices to bridge vertices list
			int borderVertexIndex1 = _bridgeVertices.Count;
			_bridgeVertices.Add(borderVertex1);
			int borderVertexIndex2 = _bridgeVertices.Count;
			_bridgeVertices.Add(borderVertex2);

			// Create border triangles
			AddTriangle(
				vertexIndex + firstTriIndices.x, 
				vertexIndex + firstTriIndices.y, 
				borderVertexIndex1);

			AddTriangle(
				vertexIndex + firstTriIndices.y, 
				borderVertexIndex2, 
				borderVertexIndex1);

			// AddTriangle(
			// 	vertexIndex + secondTriIndices.x, 
			// 	borderVertexIndex2,
			// 	borderVertexIndex1);
		}
	}


	private static void AddTriangle (int v1, int v2, int v3) {
		_bridgeTriangles.Add(v1);
		_bridgeTriangles.Add(v2);
		_bridgeTriangles.Add(v3);
	}
#endregion

	private Vector3[] UpdateBridgeVerticesHeight(Island currentIsland, Island targetIsland, Vector3[] vertices, BridgesDir dir) {
		float averageStartHeight = 0;
		float averageEndHeight = 0;

		for (int i = 0; i < _bridgesAnchorCells.GetLength(1); i++){
			AxialCoordinates startCellCoord = _bridgesAnchorCells[(int)dir, i];
			averageStartHeight += currentIsland.GetCellHeightMapValue(startCellCoord);

			AxialCoordinates endCellCoord = _bridgesAnchorCells[((int)dir + 3) % 6, i];
			averageEndHeight += targetIsland.GetCellHeightMapValue(endCellCoord);
		}
		averageStartHeight /= _bridgeSize;
		averageEndHeight /= _bridgeSize;


		for (int i = 0; i < _bridgeCellsIndexDict.Count; i++) {
			AxialCoordinates cellCoord = _bridgeCellsIndexDict.Keys.ElementAt(i);
			int z = cellCoord.z;
			bool isNewRow = i > 0 && _bridgeCellsIndexDict.Keys.ElementAt(i - 1).z != z;
			
			// Interpolate height for each cell base on it's z position in the bridge
			float height = Mathf.Lerp(averageStartHeight, averageEndHeight, (z - 1) / (float)(HexMetrics.DistanceBetweenIslands - 1));

			height = NoiseUtils.RoundToNearestHeightStep(height, HexMetrics.NbHeightSteps);

			// Determine if the cell is a side cell and raise it slightly for the fence effect
			bool isSideCell = isNewRow || 
							(i < _bridgeCellsIndexDict.Count - 1 && _bridgeCellsIndexDict.Keys.ElementAt(i + 1).z != z) || 
							(i == 0 || i == _bridgeCellsIndexDict.Count - 1);
			if (isSideCell) height += HexMetrics.HeightMultiplier / 8f;


			// Update vertex heights for the current cell
			int vertexIndex = i * 6; // each cell has 6 vertices
			for (int j = 0; j < 6; j++) { // set the height for each vertex in the cell
				vertices[vertexIndex + j].y = height;
			}
		}
		return vertices;
	}





#region UV
	void GenerateBridgeUVs(){
		for (int i = 0; i < _bridgeVertices.Count; i++){
			_bridgeUVs.Add(HexMetrics.uvCorners[i%6]); // each cell has 6 vertices
		}
	}
#endregion
}