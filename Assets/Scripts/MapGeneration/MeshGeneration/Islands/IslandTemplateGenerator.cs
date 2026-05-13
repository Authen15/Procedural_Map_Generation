using UnityEngine;
using System.Collections.Generic;
using static HexGridUtils;
using static IslandData;

public static class IslandTemplateGenerator
{
	public static Mesh TemplateMesh;

	private static List<Vector3> _baseVertices;
	private static List<int> _triangles;
	private static Vector2[] _uvs0; // uv for the whole island
	private static Vector2[] _uvs1; // uv for a cell

	public static void CreateTemplate()
	{
		if (TemplateMesh != null)
		{
			Debug.LogWarning("Island Template Mesh already created");
			return;
		}

		TemplateMesh = new Mesh();
		TemplateMesh.name = "Island Mesh";

		_triangles = new List<int>();
		_baseVertices = new List<Vector3>();
		HexCellTriangulator.Triangulate(CellCoords, CellNeighbours, _baseVertices, _triangles);

		_uvs0 = new Vector2[_baseVertices.Count];
		GenerateIslandUVs();
		_uvs1 = new Vector2[_baseVertices.Count];
		GenerateCellUVs();

		TemplateMesh.SetVertices(_baseVertices.ToArray());
		TemplateMesh.SetTriangles(_triangles.ToArray(), 0);
		TemplateMesh.SetUVs(0, _uvs0);
		TemplateMesh.SetUVs(1, _uvs1);
		TemplateMesh.RecalculateNormals();
		// TemplateMesh.normals = CalculateCustomNormals(_baseVertices, _triangles);
	}

	// calculate vertex normals based on adjacent triangles normals not weighted
	static Vector3[] CalculateCustomNormals(List<Vector3> vertices, List<int> triangles)
	{
		Vector3[] normals = new Vector3[vertices.Count];
		for (int i = 0; i < triangles.Count; i += 3)
		{
			Vector3 v0 = vertices[triangles[i]];
			Vector3 v1 = vertices[triangles[i + 1]];
			Vector3 v2 = vertices[triangles[i + 2]];

			Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;

			normals[triangles[i]] += normal;
			normals[triangles[i + 1]] += normal;
			normals[triangles[i + 2]] += normal;
		}

		for (int i = 0; i < normals.Length; i++)
		{
			normals[i].Normalize();
		}

		return normals;
	}

	// generate UVs for the whole island based on the local position of the cell in the island
	static void GenerateIslandUVs()
	{
		Vector3 localMaxPos = CellToWorld(new AxialCoordinates(HexMetrics.IslandSize, HexMetrics.IslandSize));
		AxialCoordinates offset = new AxialCoordinates(HexMetrics.IslandSize / 2, HexMetrics.IslandSize / 2); // center is 0,0 so we offset coords

		for (int cellIndex = 0; cellIndex < CellCoords.Length; cellIndex++)
		{
			int vertexIndex = cellIndex * 12;
			AxialCoordinates offsettedCoord = CellCoords[cellIndex] + offset;
			Vector3 cellLocalPos = CellToWorld(offsettedCoord);
			Vector2 cellUvPos = new Vector2(cellLocalPos.x / localMaxPos.x, cellLocalPos.z / localMaxPos.z);

			_uvs0[vertexIndex] = cellUvPos;
			_uvs0[vertexIndex + 1] = cellUvPos;
			_uvs0[vertexIndex + 2] = cellUvPos;
			_uvs0[vertexIndex + 3] = cellUvPos;
			_uvs0[vertexIndex + 4] = cellUvPos;
			_uvs0[vertexIndex + 5] = cellUvPos;
			_uvs0[vertexIndex + 6] = cellUvPos;
			_uvs0[vertexIndex + 7] = cellUvPos;
			_uvs0[vertexIndex + 8] = cellUvPos;
			_uvs0[vertexIndex + 9] = cellUvPos;
			_uvs0[vertexIndex + 10] = cellUvPos;
			_uvs0[vertexIndex + 11] = cellUvPos;

			if (cellUvPos.x > 1 || cellUvPos.y > 1 || cellUvPos.x < 0 || cellUvPos.y < 0) Debug.Log($"Error at index {vertexIndex} uv is out of bounds values (0,1) {cellUvPos}");
		}
	}

	static void GenerateCellUVs()
	{
		for (int cellIndex = 0; cellIndex < CellCoords.Length; cellIndex++)
		{
			int vertexIndex = cellIndex * 12;
			float floatIndex = cellIndex + 0.5f; // +0.5f to avoid precision error when using floor in hlsl

			_uvs1[vertexIndex] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 1] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 2] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 3] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 4] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 5] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 6] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 7] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 8] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 9] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 10] = new Vector2(floatIndex, 0);
			_uvs1[vertexIndex + 11] = new Vector2(floatIndex, 0);
		}
	}
}