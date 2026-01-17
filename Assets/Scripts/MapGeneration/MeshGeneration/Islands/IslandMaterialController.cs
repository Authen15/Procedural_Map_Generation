using UnityEngine;
using Biome;

public class IslandMaterialController
{
    private MeshRenderer _renderer;

    public IslandMaterialController(MeshRenderer meshRenderer)
    {
        _renderer = meshRenderer;    
    }

    public void ApplyMaterial(BiomeData biome)
	{
		Material terrainMaterial = biome.BiomeTerrainMaterial;
		Material grassMaterial = biome.BiomeGrassMaterial;

		if (grassMaterial != null)
		{
			_renderer.sharedMaterials = new Material[] { terrainMaterial, grassMaterial };
		}
		else
		{
			_renderer.sharedMaterial = terrainMaterial;
		}
		SetShaderProperties(terrainMaterial);
	}

    // TODO only need to set this once per material
	private void SetShaderProperties(Material material)
	{
		material.SetFloat("_MinHeight", 0);
		material.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
	}
}