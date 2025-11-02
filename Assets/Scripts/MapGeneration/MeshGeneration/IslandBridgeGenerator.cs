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
		_bridgeTemplate.SetActive(false);


		PopulateBridgeCellHashSet(_bridgeSize);

		MeshRenderer meshRenderer = _bridgeTemplate.AddComponent<MeshRenderer>();
		meshRenderer.sharedMaterial = bridgeMaterial;
		MeshFilter meshFilter = _bridgeTemplate.AddComponent<MeshFilter>();

		HexCellTriangulator.Triangulate(_bridgeCellsIndexDict, _bridgeVertices, _bridgeTriangles, HexMetrics.HeightMultiplier / 4f);
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
			Vector3 anchorWorldPos = CellToWorld(bridgeAnchorStartCell);
			Vector3 bridgePosition = currentIsland.transform.position + anchorWorldPos;

			Quaternion rotation = Quaternion.LookRotation(anchorWorldPos, Vector3.up);
			GameObject bridge = Island.Instantiate(_bridgeTemplate, bridgePosition, rotation, currentIsland.transform);
			bridge.SetActive(true);
			currentIsland.Bridges[i] = bridge;
			bridge.name = $"Bridge {currentIsland.coord}";

			Mesh mesh = bridge.GetComponent<MeshFilter>().mesh;
			mesh.vertices = UpdateBridgeVerticesHeight(currentIsland, targetIsland, mesh.vertices, (BridgesDir)i);
		}
    }

    private Island GetNeighborIsland(Island currentIsland, BridgesDir dir)
    {
		AxialCoordinates islandCoord;
        switch (dir){
			case BridgesDir.BottomRight: 
				islandCoord = new AxialCoordinates(currentIsland.coord.x + 1, currentIsland.coord.z -1);
				break;
			case BridgesDir.Bottom:
				islandCoord = new AxialCoordinates(currentIsland.coord.x, currentIsland.coord.z - 1);
				break;
			case BridgesDir.BottomLeft:
				islandCoord = new AxialCoordinates(currentIsland.coord.x - 1, currentIsland.coord.z);
				break;
			default :
				return null;
		}
		if (!MapManager.Instance.IslandDict.ContainsKey(islandCoord)) return null;
		return MapManager.Instance.IslandDict[islandCoord];
    }

	private Vector3[] UpdateBridgeVerticesHeight(Island currentIsland, Island targetIsland, Vector3[] vertices, BridgesDir dir) {
		float averageStartHeight = 0;
		float averageEndHeight = 0;

		for (int i = 0; i < _bridgesAnchorCells.GetLength(1); i++){
			AxialCoordinates startCellCoord = _bridgesAnchorCells[(int)dir, i];
			averageStartHeight += currentIsland.GetCellHeightMapValue(startCellCoord) * HexMetrics.HeightMultiplier;

			AxialCoordinates endCellCoord = _bridgesAnchorCells[((int)dir + 3) % 6, i];
			averageEndHeight += targetIsland.GetCellHeightMapValue(endCellCoord) * HexMetrics.HeightMultiplier;
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