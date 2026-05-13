using System.Collections.Generic;
using UnityEngine;
using static HexGridUtils;

public static class HexCellTriangulator
{

	public static void Triangulate(in AxialCoordinates[] cellCoords, in int[] neighbourIndices, List<Vector3> vertices, List<int> triangles, float cellHeight = HexMetrics.HeightMultiplier)
	{
		TriangulateCellTopFaces(cellCoords, vertices, triangles);

		// TriangulateSideFaces(neighbourIndices, vertices, triangles, cellHeight);
	}


	// create a cell for each coord in cellCoords, 6 vertices for the top hexagon and 6 for the bottom one, then triangulate the top face and the side faces
	private static void TriangulateCellTopFaces(in AxialCoordinates[] cellCoords, List<Vector3> vertices, List<int> triangles)
	{
		Vector3[] corners = HexMetrics.corners;
		int indexOffset = 0;
		foreach (AxialCoordinates coord in cellCoords)
		{
			Vector3 cellWorldPos = CellToWorld(coord);

			// top face vertices
			vertices.Add(cellWorldPos + corners[0]);
			vertices.Add(cellWorldPos + corners[1]);
			vertices.Add(cellWorldPos + corners[2]);
			vertices.Add(cellWorldPos + corners[3]);
			vertices.Add(cellWorldPos + corners[4]);
			vertices.Add(cellWorldPos + corners[5]);

			// bottom face vertices
			vertices.Add(cellWorldPos + corners[0] + Vector3.down * HexMetrics.HeightMultiplier);
			vertices.Add(cellWorldPos + corners[1] + Vector3.down * HexMetrics.HeightMultiplier);
			vertices.Add(cellWorldPos + corners[2] + Vector3.down * HexMetrics.HeightMultiplier);
			vertices.Add(cellWorldPos + corners[3] + Vector3.down * HexMetrics.HeightMultiplier);
			vertices.Add(cellWorldPos + corners[4] + Vector3.down * HexMetrics.HeightMultiplier);
			vertices.Add(cellWorldPos + corners[5] + Vector3.down * HexMetrics.HeightMultiplier);


			TriangulateFaces(triangles, indexOffset);


			indexOffset += 12;
		}
	}

	private static void TriangulateFaces(List<int> triangles, int indexOffset)
	{
		// ******* Top face *******
		AddTriangle(triangles, indexOffset + 0, indexOffset + 1, indexOffset + 5);
		AddTriangle(triangles, indexOffset + 1, indexOffset + 4, indexOffset + 5);
		AddTriangle(triangles, indexOffset + 1, indexOffset + 2, indexOffset + 4);
		AddTriangle(triangles, indexOffset + 2, indexOffset + 3, indexOffset + 4);

		// ******* Side faces *******

		// top right
		AddTriangle(triangles, indexOffset + 1, indexOffset + 6, indexOffset + 7);
		AddTriangle(triangles, indexOffset + 1, indexOffset + 0, indexOffset + 6);

		// right
		AddTriangle(triangles, indexOffset + 2, indexOffset + 7, indexOffset + 8);
		AddTriangle(triangles, indexOffset + 2, indexOffset + 1, indexOffset + 7);

		// bot right
		AddTriangle(triangles, indexOffset + 3, indexOffset + 8, indexOffset + 9);
		AddTriangle(triangles, indexOffset + 3, indexOffset + 2, indexOffset + 8);

		// bot left
		AddTriangle(triangles, indexOffset + 4, indexOffset + 9, indexOffset + 10);
		AddTriangle(triangles, indexOffset + 4, indexOffset + 3, indexOffset + 9);

		// left
		AddTriangle(triangles, indexOffset + 5, indexOffset + 10, indexOffset + 11);
		AddTriangle(triangles, indexOffset + 5, indexOffset + 4, indexOffset + 10);

		// top left
		AddTriangle(triangles, indexOffset + 0, indexOffset + 11, indexOffset + 6);
		AddTriangle(triangles, indexOffset + 0, indexOffset + 5, indexOffset + 11);
	}

	private static void AddTriangle(List<int> triangles, int v1, int v2, int v3)
	{
		triangles.Add(v1);
		triangles.Add(v2);
		triangles.Add(v3);
	}
}