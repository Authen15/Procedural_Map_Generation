using UnityEngine;
using System.Collections.Generic;
using static HexGridUtils;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IslandMeshGenerator
{
	private static Vector2[] _uvs;
	private static List<Vector3> _baseVertices; // vertices are the same for each island except for their height
	private static List<int> _triangles;

	[HideInInspector]
	public MeshFilter MeshFilter;
	[HideInInspector]
	public Mesh Mesh;
	[HideInInspector]
	public MeshRenderer MeshRenderer;

	public void GenerateMesh(Island island, MeshFilter meshFilter, MeshRenderer meshRenderer)
	{
		MeshFilter = meshFilter;
		MeshFilter.mesh = Mesh = new Mesh();
		MeshFilter.mesh.name = $"{island} mesh";
		MeshRenderer = meshRenderer;

		if (_triangles == null && _baseVertices == null)
		{ // store triangles and vertices in a struct as we need them only once
			float startTime = Time.realtimeSinceStartup;
			_triangles = new List<int>();
			_baseVertices = new List<Vector3>(); // * 6 because there are 6 vertices per cell, HexMetrics.IslandRadius*6*6 to duplicate the last ring vertices for island edges triangles
			HexCellTriangulator.Triangulate(IslandCellIndexDict, _baseVertices, _triangles);
			Debug.Log("First Mesh Generating time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
		}

		if (_uvs == null)
		{ // we can reuse UVs for each islands
			_uvs = new Vector2[_baseVertices.Count];
			GenerateUVs(island);
		}

		Mesh.vertices = _baseVertices.ToArray();
		Mesh.triangles = _triangles.ToArray();
		Mesh.normals = CalculateCustomNormals(Mesh.vertices, Mesh.triangles);
		Mesh.uv = _uvs;

		UpdateMesh(island);
		ApplyMaterial(island);

		// mesh.RecalculateUVDistributionMetrics();
		// gameObject.AddComponent<MeshCollider>();
	}

	public void UpdateMesh(Island island)
	{
		Vector3[] vertices = Mesh.vertices;

		UpdateVerticesHeight(island, vertices);

		Mesh.vertices = vertices;
		Mesh.RecalculateTangents();
		Mesh.RecalculateBounds();
	}

	// calculate vertex normals based on adjacent triangles normals not weighted
	Vector3[] CalculateCustomNormals(Vector3[] vertices, int[] triangles)
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

	private void ApplyMaterial(Island island)
	{
		Material terrainMaterial = island.Biome.BiomeTerrainMaterial;
		Material grassMaterial = island.Biome.BiomeGrassMaterial;

		if (grassMaterial != null)
		{
			MeshRenderer.sharedMaterials = new Material[] { terrainMaterial, grassMaterial };
		}
		else
		{
			MeshRenderer.sharedMaterial = terrainMaterial;
		}
		SetTerrainMaterialProperties(terrainMaterial);
	}

	private void SetTerrainMaterialProperties(Material material)
	{
		material.SetFloat("_MinHeight", 0);
		material.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
	}

	private static void UpdateVerticesHeight(Island island, Vector3[] vertices)
	{
		for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++)
		{
			int vertexIndex = cellIndex * 6; // 6 vertices per cell
			float height = island.GetCellHeightMapValue(IslandCellsPositions[cellIndex]) * HexMetrics.HeightMultiplier;

			for (int i = 0; i < 6; i++)
			{ // for each vertex update the height
				vertices[vertexIndex + i].y = height;
			}
		}
	}

	private static void GenerateUVs(Island island)
	{

		Vector3 localMaxPos = CellToLocal(new AxialCoordinates(HexMetrics.IslandSize, HexMetrics.IslandSize), island.coord);
		AxialCoordinates offset = new AxialCoordinates(HexMetrics.IslandSize / 2, HexMetrics.IslandSize / 2); // center is 0,0 so we offset coords

		for (int cellIndex = 0; cellIndex < IslandCellsPositions.Length; cellIndex++)
		{
			int vertexIndex = cellIndex * 6;
			AxialCoordinates offsettedCoord = IslandCellsPositions[cellIndex] + offset;
			Vector3 cellLocalPos = CellToLocal(offsettedCoord, island.coord);
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