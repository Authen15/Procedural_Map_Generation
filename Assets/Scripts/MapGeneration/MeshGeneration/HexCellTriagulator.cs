using System.Collections.Generic;
using UnityEngine;
using static HexGridUtils;

public static class HexCellTriangulator
{

	public static void Triangulate(in AxialCoordinates[] cellCoords, in int[] neighbourIndices, List<Vector3> vertices, List<int> triangles, float cellHeight = HexMetrics.HeightMultiplier)
	{
		TriangulateCellTopFaces(cellCoords, vertices, triangles);

		TriangulateSideFaces(neighbourIndices, vertices, triangles, cellHeight);
	}

	private static void TriangulateCellTopFaces(in AxialCoordinates[] cellCoords, List<Vector3> vertices, List<int> triangles)
	{
		Vector3[] corners = HexMetrics.corners;
		int indexOffset = 0;
		foreach (AxialCoordinates coord in cellCoords)
		{
			Vector3 cellWorldPos = CellToWorld(coord);
			vertices.Add(cellWorldPos + corners[0]);
			vertices.Add(cellWorldPos + corners[1]);
			vertices.Add(cellWorldPos + corners[2]);
			vertices.Add(cellWorldPos + corners[3]);
			vertices.Add(cellWorldPos + corners[4]);
			vertices.Add(cellWorldPos + corners[5]);


			AddTriangle(triangles, indexOffset + 0, indexOffset + 1, indexOffset + 5);
			AddTriangle(triangles, indexOffset + 1, indexOffset + 4, indexOffset + 5);
			AddTriangle(triangles, indexOffset + 1, indexOffset + 2, indexOffset + 4);
			AddTriangle(triangles, indexOffset + 2, indexOffset + 3, indexOffset + 4);

			indexOffset += 6;
		}
	}

	private static void TriangulateSideFaces(in int[] neighbourIndices, List<Vector3> vertices, List<int> triangles, float cellHeight)
	{
		int size = neighbourIndices.Length;
		for (int i = 0; i < size; i += 6)
		{
			// for each cell we check neighbours in 3 directions, and triangulate between current cell and neighbour cells side faces
			// so the 3 other directions will be triangulated by the neighbour cells not checked by the current cell
			// if there are no neigbours in one of the 3 other directions, we also triangulate this direction as no other cell would do it. this will handle the borders
			int vertexIndex = i; // each cell has 6 vertices
			int targetCellIndex;
			int targetCellVertexIndexOffset;

			targetCellIndex = neighbourIndices[i + (int)Dir.BottomLeft];
			targetCellVertexIndexOffset = targetCellIndex * 6;
			if (targetCellIndex != -1) // if there is a neighbour in this direction
				TriangulateSide(triangles, targetCellVertexIndexOffset, vertexIndex, 4, 3, 1, 0);
			else
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 4, 3);

			targetCellIndex = neighbourIndices[i + (int)Dir.BottomRight];
			targetCellVertexIndexOffset = targetCellIndex * 6;
			if (targetCellIndex != -1)
				TriangulateSide(triangles, targetCellVertexIndexOffset, vertexIndex, 3, 2, 0, 5);
			else
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 3, 2);

			targetCellIndex = neighbourIndices[i + (int)Dir.Left];
			targetCellVertexIndexOffset = targetCellIndex * 6;
			if (targetCellIndex != -1)
				TriangulateSide(triangles, targetCellVertexIndexOffset, vertexIndex, 5, 4, 2, 1);
			else
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 5, 4);


			// ------ if there is no neighbour in those directions, we triangulate those directions to create a border ------
			targetCellIndex = neighbourIndices[i + (int)Dir.Right];
			if (targetCellIndex == -1)
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 2, 1);

			targetCellIndex = neighbourIndices[i + (int)Dir.TopLeft];
			if (targetCellIndex == -1)
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 0, 5);

			targetCellIndex = neighbourIndices[i + (int)Dir.TopRight];
			if (targetCellIndex == -1)
				TriangulateSideNoNeighbours(vertices, triangles, cellHeight, vertexIndex, 1, 0);

		}
	}

	private static void TriangulateSide(List<int> triangles, int targetCellVertexIndexOffset, int currentCellvertexIndexOffset, int currentCellIndex1, int currentCellIndex2, int targetCellIndex1, int targetCellIndex2)
	{
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

	}

	private static void TriangulateSideNoNeighbours(List<Vector3> vertices, List<int> triangles, float cellHeight, int currentCellvertexIndexOffset, int currentCellIndex1, int currentCellIndex2)
	{
		// Neighbor cell does not exist, create border vertices and triangles

		// Get the local positions of the required vertices for the side faces
		Vector3 borderVertex1 = vertices[currentCellvertexIndexOffset + currentCellIndex1] + new Vector3(0, -cellHeight, 0);
		Vector3 borderVertex2 = vertices[currentCellvertexIndexOffset + currentCellIndex2] + new Vector3(0, -cellHeight, 0);

		// Add border vertices to vertices list
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


	private static void AddTriangle(List<int> triangles, int v1, int v2, int v3)
	{
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3);
	}
}