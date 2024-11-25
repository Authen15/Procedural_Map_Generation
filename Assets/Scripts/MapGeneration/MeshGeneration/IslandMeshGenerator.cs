using UnityEngine;
using System.Collections.Generic;
using static HexGridUtils;
using UnityEngine.Assertions.Must;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class IslandMeshGenerator {
	private static Vector2[] _uvs;
	private static List<Vector3> _baseVertices; // vertices are the same for each island except for their height
	private static List<int> _triangles;

	[HideInInspector]
	public MeshFilter MeshFilter;
	[HideInInspector]
	public Mesh Mesh;
	[HideInInspector]
	public MeshRenderer MeshRenderer;

	public void GenerateMesh(Island island, MeshFilter meshFilter, MeshRenderer meshRenderer) {
		MeshFilter = meshFilter;
		MeshFilter.mesh = Mesh = new Mesh();
		MeshRenderer = meshRenderer;

		if (_triangles == null && _baseVertices == null){ // store triangles and vertices in a struct as we need them only once
			float startTime = Time.realtimeSinceStartup;
			_triangles = new List<int>();
			_baseVertices = new List<Vector3>(); // * 6 because there are 6 vertices per cell, HexMetrics.IslandRadius*6*6 to duplicate the last ring vertices for island edges triangles
			HexCellTriangulator.Triangulate(IslandCellIndexDict, _baseVertices, _triangles);
			Debug.Log("First Mesh Generating time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
		}
		
		// if (_uvs == null ){ // we can reuse UVs for each islands
		// 	_uvs = new Vector2[HexGridUtils.IslandCellsPositions.Length * 6 + (HexMetrics.IslandRadius*6*6)];
		// 	GenerateUVs();
		// }
		
		Mesh.vertices = _baseVertices.ToArray();
		Mesh.triangles = _triangles.ToArray();
		// Mesh.uv = _uvs;

		UpdateMesh(island);
		ApplyMaterial(island.Biome.BiomeMaterial);

		// mesh.RecalculateUVDistributionMetrics();
		// gameObject.AddComponent<MeshCollider>();
	}

	public void UpdateMesh(Island island){
		Vector3[] vertices = Mesh.vertices;

		UpdateVerticesHeight(island, vertices);

		Mesh.vertices = vertices;
		Mesh.RecalculateNormals();
		Mesh.RecalculateTangents();
	}

    private void ApplyMaterial(Material material) {
        MeshRenderer.sharedMaterial = material;
        SetMaterialProperties(MeshRenderer.sharedMaterial);
	}

	private void SetMaterialProperties(Material material)
	{
		material.SetFloat("_MinHeight", 0);
		material.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
	}
	
	private static void UpdateVerticesHeight(Island island, Vector3[] vertices){
		for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++){
			int vertexIndex = cellIndex * 6; // 6 vertices per cell
			float height = island.GetCellHeightMapValue(IslandCellsPositions[cellIndex]) * HexMetrics.HeightMultiplier;
			
			for (int i = 0; i < 6; i++) { // for each vertex update the height
				vertices[vertexIndex + i].y = height;
			}
		}
	}

	// private static void GenerateUVs(){
		// Vector2[] uvCorners = HexMetrics.uvCorners;

		// Vector2 maxCellPos = HexGridUtils.CellToUV(new Vector2(HexMetrics.IslandSize / 1.5f, HexMetrics.IslandSize / 1.5f));
		// // float maxUVX = float.MinValue;
		// // float maxUVY = float.MinValue;
		
		// for (int cellIndex = 0; cellIndex < HexGridUtils.IslandCellsPositions.Length; cellIndex++){
		// 	int vertexIndex = cellIndex * 6;
		// 	Vector2 cellPos = HexGridUtils.CellToUV(HexGridUtils.IslandCellsPositions[cellIndex]);

		// 	for (int i = 0; i < 6; i++) {
		// 		Vector2 uv = uvCorners[i];
		// 		uv /= maxCellPos.x;
		// 		Vector2 cellPosNormalized = cellPos / maxCellPos.x;
		// 		uv += cellPosNormalized;
		// 		uv += new Vector2(0.5f, 0.5f);
		// 		_uvs[vertexIndex + i] = uv;
		// 		if(uv.x > 1f || uv.y > 1) Debug.Log("UV are incorrect > 1 " + uv);
				

		// 		// if(uv.x > maxUVX) maxUVX = uv.x;
		// 		// if(uv.y > maxUVY) maxUVY = uv.y;
		// 	}

		// 	// Debug.Log(
		// 	// 	// "uv " + cellIndex + " " + cellGridPos + " " + (cellGridPos + posOffset) + " are \n"
		// 	// 	"uv " + cellIndex + " are \n"
		// 	// 	+ _hex_uvs[vertexIndex].ToString(".0###########") + "\n"
		// 	// 	+ _hex_uvs[vertexIndex+1].ToString(".0###########") + "\n"
		// 	// 	+ _hex_uvs[vertexIndex+2].ToString(".0###########") + "\n"
		// 	// 	+ _hex_uvs[vertexIndex+3].ToString(".0###########") + "\n"
		// 	// 	+ _hex_uvs[vertexIndex+4].ToString(".0###########") + "\n"
		// 	// 	+ _hex_uvs[vertexIndex+5].ToString(".0###########")
		// 	// );
		// }
		// Debug.Log("maxUVX " + maxUVX + " maxUVY" + maxUVY);
	// }
}