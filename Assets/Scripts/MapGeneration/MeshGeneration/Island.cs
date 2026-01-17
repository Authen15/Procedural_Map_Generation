using UnityEngine;
using Biome;
using Unity.AI.Navigation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class Island : MonoBehaviour
{
	public bool IsInitialized { get; private set; }
	public AxialCoordinates Coord { get; private set; }
	public BiomeData Biome { get; private set; }
	public Material BridgeMaterial;

	// Cached data
	private float[] _heightMapFlat;
	[SerializeField] private MeshFilter _meshFilter;
	[SerializeField] private MeshCollider _meshCollider;
	[SerializeField] private MeshRenderer _meshRenderer;
	[SerializeField] private IslandSpawner _islandSpawner;
	[SerializeField] private NavMeshSurface _navMeshSurface;

	// Controlers
	private IslandMeshController _meshController;
	private IslandMaterialController _materialController;
	private BridgeController _bridgeController;

	private void Awake()
	{
		_meshController = new IslandMeshController(_meshFilter);
		_materialController = new IslandMaterialController(_meshRenderer);
		_bridgeController = new BridgeController();
	}

	public void Start()
	{
		// Update the mesh at runtime when modifying heightmap parameters
		Biome.HeightMapSettings.UpdateIslandMesh += () =>
		{
			GenerateHeightMap();
			_meshController.UpdateMesh(_heightMapFlat);
		};
		PlayerEventManager.Instance.OnIslandChanged.AddListener(SetMeshCollider);

	}

	public void Initialize(AxialCoordinates coord)
	{
		if (IsInitialized && Coord == coord) return;

		Coord = coord;
		name = $"Island {coord}";
		transform.position = HexGridUtils.IslandToWorld(Coord);


		Biome = BiomeManager.Instance.GetBiome();

		// Generate (or re-generate)
		GenerateHeightMap();

		_meshController.UpdateMesh(_heightMapFlat);

		_materialController.ApplyMaterial(Biome);

		StartCoroutine(IslandNavMeshUtils.BuildNavMeshAsync(transform, _navMeshSurface, () => _islandSpawner.SetActive(true)));

		IsInitialized = true;
	}

	public void Cleanup()
	{
		StopAllCoroutines();
		_islandSpawner.Cleanup();

		IsInitialized = false;
	}

	public void SetMeshCollider(AxialCoordinates currentIsland)
	{
		if (currentIsland == Coord)
		{
			_meshCollider.sharedMesh = _meshFilter.mesh;
		}
		else
		{
			_meshCollider.sharedMesh = null;
		}
	}

	private void GenerateHeightMap()
	{
		if (_heightMapFlat == null) _heightMapFlat = new float[HexMetrics.MapSize * HexMetrics.MapSize];

		_heightMapFlat = NoiseGenerator.GenerateHeightMap(Biome.HeightMapSettings, HexMetrics.IslandSize, Coord.S + Coord.R * HexMetrics.MapSize);
	}

	public float GetCellHeightMapValue(AxialCoordinates cellCoordinates)
	{
		int size = HexMetrics.IslandSize;
		int index = (cellCoordinates.R + HexMetrics.IslandRadius) * size +
					(cellCoordinates.S + HexMetrics.IslandRadius);
		return _heightMapFlat[index];
	}

	public void UpdateBridges()
	{
		_bridgeController.UpdateBridges(this);
	}

	public void PrintHeightMapDistribution()
	{
		NoiseGenerator.PrintNoiseDistribution(_heightMapFlat, HexMetrics.IslandSize, 10);
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