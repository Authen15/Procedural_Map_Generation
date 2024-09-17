using UnityEngine;
using Biome;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Island : MonoBehaviour {

	public int IslandX;
	public int IslandZ; 
	public BiomeData Biome;

	[HideInInspector]
	public MeshFilter MeshFilter;
	[HideInInspector]
	public Mesh Mesh;
	[HideInInspector]
	public MeshRenderer MeshRenderer;
	
    public void Initialize(int x, int z)
    {
		IslandX = x;
		IslandZ = z;

		Biome = BiomeManager.Instance.GetBiome();

		MeshFilter = GetComponent<MeshFilter>();
		MeshFilter.mesh = Mesh = new Mesh();
		MeshRenderer = GetComponent<MeshRenderer>();

		HexIslandMeshGenerator.GenerateMesh(this); // TODO generate Mesh only if island is adjacent to player current island
		gameObject.AddComponent<MeshCollider>();  // TODO add Mesh Collider only when player is on the island

		Biome.HeightMapSettings.UpdateIslandMesh += () => HexIslandMeshGenerator.UpdateMesh(this); // Update the mesh at runtime when modifying heightmap parameters
	}
}