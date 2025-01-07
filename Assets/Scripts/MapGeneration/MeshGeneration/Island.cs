using UnityEngine;
using Biome;
using Unity.AI.Navigation;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class Island : MonoBehaviour {

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

	private void GenerateHeightMap(){
		HeightMap = NoiseGenerator.GenerateHeightMap(Biome.HeightMapSettings, HexMetrics.IslandSize, coord.x + coord.z * HexMetrics.MapSize);
	}

	public void GenerateIsland(){
		GenerateIslandMesh();
		GenerateIslandBridges();
	}

	private void GenerateIslandMesh(){
		IslandMeshGenerator meshGenerator = new IslandMeshGenerator(); // TODO generate Mesh only if island is adjacent to player current island
		meshGenerator.GenerateMesh(this, GetComponent<MeshFilter>(), GetComponent<MeshRenderer>());  // TODO add Mesh Collider only when player is on the island

		gameObject.AddComponent<MeshCollider>();  // TODO add Mesh Collider only when player is on the island
		NavMeshSurface.BuildNavMesh();


		Biome.HeightMapSettings.UpdateIslandMesh += () => { // Update the mesh at runtime when modifying heightmap parameters
			GenerateHeightMap();
			meshGenerator.UpdateMesh(this); 
		};
	}

	private void GenerateIslandBridges(){
		Bridges = new GameObject[3];

		IslandBridgeGenerator islandBridgeGenerator = new IslandBridgeGenerator();
		islandBridgeGenerator.GenerateIslandBridges(this, BridgeMaterial);

		for (int i = 0; i < Bridges.Length; i++){
			if (Bridges[i] != null){
				Bridges[i].AddComponent<MeshCollider>();
			}
		}
	}

	public float GetCellHeightMapValue(AxialCoordinates cellCoordinates){
		//TODO change coordinates system to be able to use heightmap in shaders (right now the x axis is creating problem because of the shift)
		// Maybe try doing a convertion from axial coord to offset https://www.redblobgames.com/grids/hexagons/
		return HeightMap[cellCoordinates.x + HexMetrics.IslandRadius, cellCoordinates.z + HexMetrics.IslandRadius];
	}
}