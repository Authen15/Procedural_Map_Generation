using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexGridUtils;

public static class HexCellTriangulator {

	public static void Triangulate(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles){
		float startTime = Time.realtimeSinceStartup;
		TriangulateCellTopFaces(cellDict, vertices, triangles);
		Debug.Log("TriangulateCellTopFaces time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));

		startTime = Time.realtimeSinceStartup;
		TriangulateSideFaces(cellDict, vertices, triangles);
		Debug.Log("TriangulateSideFaces time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
	}

    private static void TriangulateCellTopFaces(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles){
		
		Vector3[] corners = HexMetrics.corners;
		int indexOffset = 0;
		foreach (AxialCoordinates cell in cellDict.Keys)
		{
			Vector3 cellWorldPos = CellToWorld(cell);
			for (int i = 0; i < 6; i++) {
				vertices.Add(cellWorldPos + corners[i]);				
			}

			AddTriangle(triangles, indexOffset + 0, indexOffset + 1, indexOffset + 5);
			AddTriangle(triangles, indexOffset + 1, indexOffset + 4, indexOffset + 5);	
			AddTriangle(triangles, indexOffset + 1, indexOffset + 2, indexOffset + 4);
			AddTriangle(triangles, indexOffset + 2, indexOffset + 3, indexOffset + 4);

			indexOffset += 6;
		}
	}

	private static void TriangulateSideFaces(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles){
		foreach (AxialCoordinates cellCoord in cellDict.Keys){
			// for each cell we check neighbours in 3 directions, and triangulate between current cell and neighbour cells side faces
			// so the 3 other directions will be triangulated by the neighbour cells not checked by the current cell
			// if there are no neigbours in one of the 3 other directions, we also triangulate this direction as no other cell would do it. this will handle the borders
			int cellIndex = cellDict[cellCoord];
			int vertexIndex = cellIndex * 6; // each cell has 6 vertices
			for (int j = 0; j < 6; j++) {
				AxialCoordinates targetCell = cellCoord + GetDir[j];
				bool hasNeighboor = cellDict.TryGetValue(targetCell, out int targetCellIndex);
				int targetCellVertexIndexOffset = targetCellIndex * 6;
				switch ((Dir)j) 
				{
					case Dir.BottomLeft: TriangulateBotLeftSide(targetCellVertexIndexOffset, vertices, triangles, hasNeighboor, vertexIndex); break;
					case Dir.BottomRight: TriangulateBotRightSide(targetCellVertexIndexOffset, vertices, triangles, hasNeighboor, vertexIndex); break;
					case Dir.Left: TriangulateLeftSide(targetCellVertexIndexOffset, vertices, triangles, hasNeighboor, vertexIndex); break;
					case Dir.Right: 
						if (!hasNeighboor) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateRightSide(targetCellVertexIndexOffset, vertices, triangles, vertexIndex); break;
					case Dir.TopLeft: 
						if (!hasNeighboor) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopLeftSide(targetCellVertexIndexOffset, vertices, triangles, vertexIndex); break;
					case Dir.TopRight: 
						if (!hasNeighboor) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopRightSide(targetCellVertexIndexOffset, vertices, triangles, vertexIndex); break;
					default :
						break;
				}
			}
		}	
	}
    
	// called for any cell
    private static void TriangulateBotLeftSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, bool hasNeighboor, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, hasNeighboor, 4, 3, 1, 0);
	}
	private static void TriangulateBotRightSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, bool hasNeighboor, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, hasNeighboor, 3, 2, 0, 5);
	}
	private static void TriangulateLeftSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, bool hasNeighboor, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, hasNeighboor, 5, 4, 2, 1);
	}

	// called only for border cells that have no neighbour in the direction
	private static void TriangulateRightSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, false, 2, 1, 5, 4);
	}
	private static void TriangulateTopLeftSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, false, 0, 5, 3, 2);
	}
	private static void TriangulateTopRightSide(int targetCellVertexIndexOffset, List<Vector3> vertices, List<int> triangles, int currentCellvertexIndexOffset){
		TriangulateSide(vertices, triangles, targetCellVertexIndexOffset, currentCellvertexIndexOffset, false, 1, 0, 4, 3);
	}

    private static void TriangulateSide(List<Vector3> vertices, List<int> triangles, int targetCellVertexIndexOffset, int currentCellvertexIndexOffset, bool hasNeighboor, int currentCellIndex1, int currentCellIndex2, int targetCellIndex1, int targetCellIndex2) {
		// Check if neighboring cell exists
		if (hasNeighboor) {
			AddTriangle(
				triangles,
				currentCellvertexIndexOffset + currentCellIndex1, 
				currentCellvertexIndexOffset + currentCellIndex2, 
				targetCellVertexIndexOffset + targetCellIndex1);
				
			AddTriangle(
				triangles,
				currentCellvertexIndexOffset + currentCellIndex1, 
				targetCellVertexIndexOffset + targetCellIndex1, 
				targetCellVertexIndexOffset + targetCellIndex2);
		} else {
			// Neighbor cell does not exist, create border vertices and triangles
			
			// Get the local positions of the required vertices for the side faces
			Vector3 borderVertex1 = vertices[currentCellvertexIndexOffset + currentCellIndex1] + new Vector3(0,-1,0);//corners[firstTriIndices.z];
			Vector3 borderVertex2 = vertices[currentCellvertexIndexOffset + currentCellIndex2] + new Vector3(0,-1,0); //corners[secondTriIndices.z];

			// Add border vertices to bridge vertices list
			int borderVertexIndex1 = vertices.Count;
			vertices.Add(borderVertex1);
			int borderVertexIndex2 = vertices.Count;
			vertices.Add(borderVertex2);

			// Create border triangles
			AddTriangle(
				triangles,
				currentCellvertexIndexOffset + currentCellIndex1, 
				currentCellvertexIndexOffset + currentCellIndex2, 
				borderVertexIndex1);

			AddTriangle(
				triangles,
				currentCellvertexIndexOffset + currentCellIndex2, 
				borderVertexIndex2, 
				borderVertexIndex1);
		}
	}


	private static void AddTriangle (List<int> triangles, int v1, int v2, int v3) {
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3);
	}
}