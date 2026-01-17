using UnityEngine;
using System.Collections.Generic;
using static HexGridUtils;

public static class IslandTemplateGenerator
{
	public static Mesh TemplateMesh;

	private static List<Vector3> _baseVertices; // vertices are the same for all islands except for their height
	private static List<int> _triangles; // same for all islands
	private static Vector2[] _uvs; // same for all islands


	static IslandTemplateGenerator()
    {
        CreateTemplate();
    }

	static void CreateTemplate()
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
        HexCellTriangulator.Triangulate(IslandCellIndexDict, _baseVertices, _triangles);

        _uvs = new Vector2[_baseVertices.Count];
        GenerateUVs();

		TemplateMesh.vertices = _baseVertices.ToArray();
		TemplateMesh.triangles = _triangles.ToArray();
		TemplateMesh.uv = _uvs;
		TemplateMesh.normals = CalculateCustomNormals(TemplateMesh.vertices, TemplateMesh.triangles);
	}

	// calculate vertex normals based on adjacent triangles normals not weighted
	static Vector3[] CalculateCustomNormals(Vector3[] vertices, int[] triangles)
	{
		Vector3[] normals = new Vector3[vertices.Length];
		for (int i = 0; i < triangles.Length; i += 3)
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

	static void GenerateUVs()
	{

		Vector3 localMaxPos = CellToWorld(new AxialCoordinates(HexMetrics.IslandSize, HexMetrics.IslandSize));
		AxialCoordinates offset = new AxialCoordinates(HexMetrics.IslandSize / 2, HexMetrics.IslandSize / 2); // center is 0,0 so we offset coords

		for (int cellIndex = 0; cellIndex < IslandCellsPositions.Length; cellIndex++)
		{
			int vertexIndex = cellIndex * 6;
			AxialCoordinates offsettedCoord = IslandCellsPositions[cellIndex] + offset;
			Vector3 cellLocalPos = CellToWorld(offsettedCoord);
			Vector2 cellUvPos = new Vector2(cellLocalPos.x / localMaxPos.x, cellLocalPos.z / localMaxPos.z);

			_uvs[vertexIndex] = cellUvPos;
			_uvs[vertexIndex + 1] = cellUvPos;
			_uvs[vertexIndex + 2] = cellUvPos;
			_uvs[vertexIndex + 3] = cellUvPos;
			_uvs[vertexIndex + 4] = cellUvPos;
			_uvs[vertexIndex + 5] = cellUvPos;

			if (cellUvPos.x > 1 || cellUvPos.y > 1 || cellUvPos.x < 0 || cellUvPos.y < 0) Debug.Log($"Error at index {vertexIndex} uv is out of bounds values (0,1) {cellUvPos}");
		}
	}
}