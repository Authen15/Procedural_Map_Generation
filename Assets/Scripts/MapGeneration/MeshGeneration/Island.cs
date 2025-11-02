using UnityEngine;
using Biome;
using Unity.AI.Navigation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Island : MonoBehaviour
{

	public bool isInitialised;
	public AxialCoordinates coord;
	public BiomeData Biome;
	public float[,] HeightMap;

	public Material BridgeMaterial;
	public GameObject[] Bridges;

	public IslandSpawner IslandSpawner;

	public NavMeshSurface NavMeshSurface;

	public void Initialize(AxialCoordinates coord)
	{
		this.coord = coord;
		name = "Island " + coord.ToString();

		Biome = BiomeManager.Instance.GetBiome();

		GenerateHeightMap();

		isInitialised = true;
	}

	private void GenerateHeightMap()
	{
		HeightMap = NoiseGenerator.GenerateHeightMap(Biome.HeightMapSettings, HexMetrics.IslandSize, coord.x + coord.z * HexMetrics.MapSize);
	}

	public void GenerateIsland()
	{
		GenerateIslandMesh();
		GenerateIslandBridges();
	}

	private void GenerateIslandMesh()
	{
		IslandMeshGenerator meshGenerator = new IslandMeshGenerator(); // TODO generate Mesh only if island is adjacent to player current island
		meshGenerator.GenerateMesh(this, GetComponent<MeshFilter>(), GetComponent<MeshRenderer>());  // TODO add Mesh Collider only when player is on the island

		gameObject.AddComponent<MeshCollider>();  // TODO add Mesh Collider only when player is on the island
		NavMeshSurface.BuildNavMesh();


		Biome.HeightMapSettings.UpdateIslandMesh += () =>
		{ // Update the mesh at runtime when modifying heightmap parameters
			GenerateHeightMap();
			meshGenerator.UpdateMesh(this);
		};
	}

	private void GenerateIslandBridges()
	{
		Bridges = new GameObject[3];

		IslandBridgeGenerator islandBridgeGenerator = new IslandBridgeGenerator();
		islandBridgeGenerator.GenerateIslandBridges(this, BridgeMaterial);

		for (int i = 0; i < Bridges.Length; i++)
		{
			if (Bridges[i] != null)
			{
				Bridges[i].AddComponent<MeshCollider>();
			}
		}
	}

	public float GetCellHeightMapValue(AxialCoordinates cellCoordinates)
	{
		//TODO change coordinates system to be able to use heightmap in shaders (right now the x axis is creating problem because of the shift)
		// Maybe try doing a convertion from axial coord to offset https://www.redblobgames.com/grids/hexagons/
		return HeightMap[cellCoordinates.x + HexMetrics.IslandRadius, cellCoordinates.z + HexMetrics.IslandRadius];
	}

	public void PrintHeightMapDistribution()
	{
		NoiseGenerator.PrintNoiseDistribution(HeightMap, 10);
	}

	// to display normals
	// public void OnDrawGizmosSelected()
	// {

	// 	Gizmos.color = Color.red;
	// 	Mesh mesh = GetComponent<MeshFilter>().sharedMesh;

	// 	// // vertex normals
	// 	// {
	// 	// 	Vector3[] normals = mesh.normals;
	// 	// 	Vector3[] vertices = mesh.vertices;
	// 	// 	for (int i = 0; i < vertices.Length; i++)
	// 	// 	{
	// 	// 		// if (vertices[i].y > HexMetrics.HeightMultiplier / 2f){
	// 	// 		Gizmos.DrawSphere(vertices[i], 0.1f);
	// 	// 		Gizmos.DrawLine(vertices[i], vertices[i] + normals[i]);
	// 	// 		// }
	// 	// 	}
	// 	// }

	// 	// faces triangles
	// 	{
	// 		Gizmos.color = Color.green;
	// 		int[] triangles = mesh.triangles;
	// 		Vector3[] vertices = mesh.vertices;
	// 		for (int i = 0; i < triangles.Length; i += 3)
	// 		{
	// 			Vector3 v0 = vertices[triangles[i]];
	// 			Vector3 v1 = vertices[triangles[i + 1]];
	// 			Vector3 v2 = vertices[triangles[i + 2]];

	// 			Vector3 normal = Vector3.Cross(v1 - v0, v2 - v0).normalized;
	// 			Vector3 center = (v0 + v1 + v2) / 3f;

	// 			Gizmos.DrawLine(center, center + normal);
	// 		}
	// 	}
	// }
}