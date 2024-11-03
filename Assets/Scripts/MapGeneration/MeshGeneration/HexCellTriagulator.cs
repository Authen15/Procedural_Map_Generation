using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static HexGridUtils;

public static class HexCellTriangulator {

	public static void Triangulate(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles){
		TriangulateCellTopFaces(cellDict, vertices, triangles);
		TriangulateSideFaces(cellDict, vertices, triangles);
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
		for (int i = 0; i < cellDict.Count; i++){
			AxialCoordinates cellCoord = cellDict.Keys.ElementAt(i);
			// for each cell we check neighbours in 3 directions, and triangulate between current cell and neighbour cells side faces
			// so the 3 other directions will be triangulated by the neighbour cells not checked by the current cell
			// if there are no neigbours in one of the 3 other directions, we also triangulate this direction as no other cell would do it. this will handle the borders
			int vertexIndex = i * 6; // each cell has 6 vertices
			for (int j = 0; j < 6; j++) {
				AxialCoordinates targetCell = cellCoord + GetDir[j];
				switch ((Dir)j) 
				{
					case Dir.BottomLeft: TriangulateBotLeftSide(cellDict, vertices, triangles, i, vertexIndex); break;
					case Dir.BottomRight: TriangulateBotRightSide(cellDict, vertices, triangles, i, vertexIndex); break;
					case Dir.Left: TriangulateLeftSide(cellDict, vertices, triangles, i, vertexIndex); break;
					case Dir.Right: 
						if (!cellDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateRightSide(cellDict, vertices, triangles, i, vertexIndex); break;
					case Dir.TopLeft: 
						if (!cellDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopLeftSide(cellDict, vertices, triangles, i, vertexIndex); break;
					case Dir.TopRight: 
						if (!cellDict.ContainsKey(targetCell)) // if there is no neighbour in this direction, we triangulate this direction to create a border
							TriangulateTopRightSide(cellDict, vertices, triangles, i, vertexIndex); break;
				}
			}
		}	
	}
    
    private static void TriangulateBotLeftSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 4, 3, 1, 0, Dir.BottomLeft);
	}

	private static void TriangulateLeftSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 5, 4, 2, 1, Dir.Left);
	}

	private static void TriangulateTopLeftSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 0, 5, 3, 2, Dir.TopLeft);
	}

	private static void TriangulateTopRightSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 1, 0, 4, 3, Dir.TopRight);
	}

	private static void TriangulateRightSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 2, 1, 5, 4, Dir.Right);
	}

	private static void TriangulateBotRightSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex){
		TriangulateSide(cellDict, vertices, triangles, cellIndex, vertexIndex, 3, 2, 0, 5, Dir.BottomRight);
	}

    private static void TriangulateSide(Dictionary<AxialCoordinates, int> cellDict, List<Vector3> vertices, List<int> triangles, int cellIndex, int vertexIndex, int currentCellIndex1, int currentCellIndex2, int targetCellIndex1, int targetCellIndex2, Dir dir) {
		AxialCoordinates sideCellPos = cellDict.Keys.ElementAt(cellIndex) + GetDir[(int)dir];
		
		// Check if neighboring cell exists
		if (cellDict.ContainsKey(sideCellPos)) {
			// Neighboring cell exists, proceed with existing vertices
			int sideCellIndex = cellDict[sideCellPos];
			int sideCellVertexIndex = sideCellIndex * 6;

			AddTriangle(
				triangles,
				vertexIndex + currentCellIndex1, 
				vertexIndex + currentCellIndex2, 
				sideCellVertexIndex + targetCellIndex1);
				
			AddTriangle(
				triangles,
				vertexIndex + currentCellIndex1, 
				sideCellVertexIndex + targetCellIndex1, 
				sideCellVertexIndex + targetCellIndex2);
		} else {
			// Neighbor cell does not exist, create border vertices and triangles
			
			// Get the local positions of the required vertices for the side faces
			Vector3 borderVertex1 = vertices[vertexIndex + currentCellIndex1] + new Vector3(0,-1,0);//corners[firstTriIndices.z];
			Vector3 borderVertex2 = vertices[vertexIndex + currentCellIndex2] + new Vector3(0,-1,0); //corners[secondTriIndices.z];

			// Add border vertices to bridge vertices list
			int borderVertexIndex1 = vertices.Count;
			vertices.Add(borderVertex1);
			int borderVertexIndex2 = vertices.Count;
			vertices.Add(borderVertex2);

			// Create border triangles
			AddTriangle(
				triangles,
				vertexIndex + currentCellIndex1, 
				vertexIndex + currentCellIndex2, 
				borderVertexIndex1);

			AddTriangle(
				triangles,
				vertexIndex + currentCellIndex2, 
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