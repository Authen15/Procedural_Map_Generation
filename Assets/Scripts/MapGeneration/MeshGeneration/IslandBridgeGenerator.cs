using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexGridUtils;

public class IslandBridgeGenerator {
	private static AxialCoordinates[] _bridgesAnchorCells;
	private static GameObject _bridgeTemplate;
    private static Dictionary<AxialCoordinates, int> _bridgeCellsIndexDict;
	private static List<Vector3> _bridgeVertices = new List<Vector3>();
	private static List<int> _bridgeTriangles = new List<int>();
	private static List<Vector2> _bridgeUVs = new List<Vector2>();

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
		_bridgesAnchorCells = new AxialCoordinates[6]; // 6 faces of the hexagon

		int anchorCellIndex = GetIslandCellsNumber(HexMetrics.IslandRadius - 1) // index of the first cell in the last ring
                                            + HexMetrics.IslandRadius / 2; // offset to get the middle cell from the bot Right side
		
		_bridgesAnchorCells[0] = IslandCellsPositions[anchorCellIndex];
		for (int i = 1; i < 6; i++){
			if (HexMetrics.IslandRadius %2 == 0){ // if radius is even, the last ring side have an odd number of cells, the center is a unique cell
				anchorCellIndex += HexMetrics.IslandRadius; // the offset to get other mid cells is the previous + radius
				_bridgesAnchorCells[i] = IslandCellsPositions[anchorCellIndex];
			}else
			{ //TODO, handle case when island radius is odd and the center is composed of 2 cells

			}
		}
    }
	private void GenerateBridgeTemplate(Material bridgeMaterial, int bridgeSize){
		if (_bridgeTemplate != null) return; // if the template already exists, return
		_bridgeTemplate = new GameObject("Bridge Template");

		PopulateBridgeCellHashSet(bridgeSize);

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
				bool isOddRow = z % 2 == 1;
				if (x == 0 && !isOddRow) continue; // skip the first cell in even rows
				AxialCoordinates cellPos = midCellPos + GetDir[(int)Dir.Right] * (x - offset);
				_bridgeCellsIndexDict.Add(cellPos, cellIndex++);
			}
		}
	}
#endregion
    public void GenerateIslandBridges(Island currentIsland, Material bridgeMaterial, int bridgeSize){
		GenerateBridgeTemplate(bridgeMaterial, bridgeSize);
        for (int i = 0; i < 3; i++){
			Island targetIsland = GetNeighborIsland(currentIsland, (BridgesDir)i);
			if (targetIsland == null) continue; // if there is no island in this direction, skip this iteration
			
			AxialCoordinates bridgeAnchorStartCell = _bridgesAnchorCells[i];
			AxialCoordinates bridgeAnchorEndCell = _bridgesAnchorCells[(i + 3) % 6]; // get the opposite bridge anchor cell
			Vector3 anchorWorldPos = CellToWorld(bridgeAnchorStartCell);
			Vector3 bridgePosition = currentIsland.transform.position + anchorWorldPos;

			Quaternion rotation = Quaternion.LookRotation(anchorWorldPos, Vector3.up);
			GameObject bridge = Island.Instantiate(_bridgeTemplate, bridgePosition, rotation, currentIsland.transform);
			currentIsland.Bridges[i] = bridge;
			bridge.name = string.Format("Bridge ({0},{1})", currentIsland.IslandX, currentIsland.IslandZ);

			Mesh mesh = bridge.GetComponent<MeshFilter>().mesh;
			float startHeight = currentIsland.GetCellHeightMapValue(bridgeAnchorStartCell);
			float endHeight = targetIsland.GetCellHeightMapValue(bridgeAnchorEndCell);
			mesh.vertices = UpdateBridgeVerticesHeight(mesh.vertices, startHeight, endHeight);
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


	private static void TriangulateSide(int cellIndex, int vertexIndex, Vector3Int firstTriIndices, Vector3Int secondTriIndices, Dir dir){
		AxialCoordinates sideCellPos = _bridgeCellsIndexDict.Keys.ElementAt(cellIndex) + GetDir[(int)dir];
		if(!_bridgeCellsIndexDict.ContainsKey(sideCellPos)){ // side cell does not exist
			Debug.LogWarning("trying to access a cell not existing in the bridge");
			return;
		}
		int sideCellIndex = _bridgeCellsIndexDict[sideCellPos];
		int sideCellVertexIndex = sideCellIndex*6;

		

		AddTriangle(
			vertexIndex + firstTriIndices.x, 
			vertexIndex + firstTriIndices.y, 
			sideCellVertexIndex + firstTriIndices.z);
			
		AddTriangle(
			vertexIndex + secondTriIndices.x, 
			sideCellVertexIndex + secondTriIndices.y, 
			sideCellVertexIndex + secondTriIndices.z);
	}

	private static void AddTriangle (int v1, int v2, int v3) {
		_bridgeTriangles.Add(v1);
		_bridgeTriangles.Add(v2);
		_bridgeTriangles.Add(v3);
	}
#endregion

	private Vector3[] UpdateBridgeVerticesHeight(Vector3[] vertices, float startHeight, float endHeight){ //TODO convert function to use arrays
		for (int i = 0; i < _bridgeCellsIndexDict.Count; i++){
			AxialCoordinates cellCoord = _bridgeCellsIndexDict.Keys.ElementAt(i);
			int x = cellCoord.x;
			int z = cellCoord.z;
			

			float height = Mathf.Lerp(startHeight, endHeight, z/(float)HexMetrics.DistanceBetweenIslands); // the z coord starts at 0 on the current island and DistanceBetweenIslands on the target island, so we use it to interpolate the height between the two islands
			height = NoiseUtils.RoundToNearestHeightStep(height, HexMetrics.NbHeightSteps);
			// if previous or next cell z coord is different from current cell z coord, then it's a side cell
			// or if it's the first cell / last cell in the array
			bool isSideCell = (i > 0 && _bridgeCellsIndexDict.Keys.ElementAt(i - 1).z != z) || (i < _bridgeCellsIndexDict.Count - 1 && _bridgeCellsIndexDict.Keys.ElementAt(i + 1).z != z) || (i == 0 || i == _bridgeCellsIndexDict.Count - 1);
			if (isSideCell) height += HexMetrics.HeightMultiplier/10f;	 // make some borders for the sides of the bridge


			int vertexIndex = i * 6; // each cell has 6 vertices, so we multiply the cell index by 6 to get the vertex index
			for (int j = 0; j < 6; j++) { // for each vertex update the height
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