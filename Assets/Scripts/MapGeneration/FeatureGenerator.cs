using UnityEngine;
using System.Collections.Generic;

public class FeatureGenerator 
{
    private Dictionary<DataMapSettings, DataMap> featureMaps;
    private HexMapData hexMapData;

    public FeatureGenerator(Dictionary<DataMapSettings, DataMap> featureMaps, HexMapData hexMapData)
    {
        this.featureMaps = featureMaps;
        this.hexMapData = hexMapData;
    }

    public void GenerateFeatures()
    {
        foreach(HexCell cell in hexMapData.cells)
        {
            Biome biome = cell.Biome;
            BiomeLevel level = cell.BiomeLevel;
            foreach(BiomeFeatureData biomeFeatureData in biome.biomeFeatureData)
            {
                var noiseMap = featureMaps[biomeFeatureData.featurePack.noiseMapSettings]; //Don't forget to add the dataMapSettings for the features to the HexGridManager //TODO get the dataMapSettings from the biomeFeatureData

                // Check the noise value for this cell and decide whether to instantiate the feature or not
                float noiseValue = noiseMap.values[cell.X, cell.Z];
                
                if(IsFeaturePlacable(biomeFeatureData, level, noiseValue, cell.GetPlaceHolderTransform())){
                    // Debug.Log("Generating feature for cell: " + cell.X + ", " + cell.Z);
                    GameObject featureObject =  HandleFeaturePlacement(biomeFeatureData, cell.GetPlaceHolderTransform());
                    HandleFeatureShader(biomeFeatureData, featureObject, biome.biomeTint, noiseValue);
                }
            }
        }
    }
            

    private bool IsFeaturePlacable(BiomeFeatureData biomeFeatureData, BiomeLevel level, float noiseValue, Transform placeHolder)
    {
        return Random.Range(0f, 1f) < biomeFeatureData.probability && IsFeatureInBiomeLevel(biomeFeatureData, level) && IsFeatureInNoiseThrehold(biomeFeatureData, noiseValue) && IsPlaceHolderEmpty(placeHolder);
    }

    private bool IsFeatureInBiomeLevel(BiomeFeatureData biomeFeatureData, BiomeLevel level)
    {
        return biomeFeatureData.applicableLevels.Contains(level);
    }

    private bool IsFeatureInNoiseThrehold(BiomeFeatureData biomeFeatureData, float noiseValue)
    {
        return noiseValue >= biomeFeatureData.featureThresholdMin && noiseValue <= biomeFeatureData.featureThresholdMax; 
    }

    private bool IsPlaceHolderEmpty(Transform placeHolder)
    {
        return placeHolder.childCount == 0;
    }

    //instantiate feature and place it at the center of the placeholder
    private GameObject HandleFeaturePlacement(BiomeFeatureData biomeFeatureData, Transform placeHolder)
    {
        GameObject chosenPrefab = biomeFeatureData.featurePack.prefabs[Random.Range(0, biomeFeatureData.featurePack.prefabs.Count)];

        GameObject featureObject = Object.Instantiate(chosenPrefab);
        featureObject.transform.SetParent(placeHolder, false);
        featureObject.transform.localPosition = Vector3.zero; //make sure the object instantiated is at the center of the placeholder
        RandomizeTransform(featureObject.transform);

        ApplyScaleMultiplier(featureObject.transform, biomeFeatureData.scaleMultiplier);

        return featureObject;

    }

    private void RandomizeTransform(Transform transform)
    {
        RandomizePosition(transform);
        RandomizeRotation(transform);
        RandomizeScale(transform);
    }

    private void RandomizePosition(Transform transform)
    {
        float RandomOffset = 0.5f * HexMetrics.innerRadius;
        float randomXOffset = Random.Range(-RandomOffset, RandomOffset);
        float randomZOffset = Random.Range(-RandomOffset, RandomOffset);
        transform.localPosition += new Vector3(randomXOffset, 0, randomZOffset);
    }

    private void RandomizeRotation(Transform transform)
    {
        float randomRotation = Random.Range(0, 360);
        transform.Rotate(new Vector3(0, randomRotation, 0));
    }

    private void RandomizeScale(Transform transform)
    {
        float randomScale = Random.Range(0.8f, 1.2f);
        transform.localScale = new Vector3(randomScale, randomScale, randomScale);
    }

    private void ApplyScaleMultiplier(Transform transform, Vector3 scaleMultiplier)
    {
        transform.localScale = Vector3.Scale(transform.localScale, scaleMultiplier);
    }
   
   private void HandleFeatureShader(BiomeFeatureData biomeFeatureData, GameObject featureObject, Color biomeTint, float noiseValue)
   {
        Renderer renderer = featureObject.GetComponent<Renderer>();
        if (renderer != null && renderer.sharedMaterial != null)
        {
            // Create an instance of the material to avoid changing the shared material
            Material matInstance = new Material(biomeFeatureData.featurePack.material);
            ApplyShaderParameters(matInstance, biomeFeatureData.shaderParameters);

            if(biomeFeatureData.isAffectedByBiomeColor)
            {
                AffectFeatureByBiomeColor(matInstance, biomeFeatureData.shaderParameters, biomeTint, noiseValue); //affect the feature color by the biome color depending on the noise value
            }

            renderer.material = matInstance;
        }
   }
   
   
    private void ApplyShaderParameters(Material material, ShaderParameters parameters)
    {
        material.SetColor("_BaseColor", parameters.BaseColor);
        material.SetColor("_SnowColor", parameters.SnowColor);
        material.SetFloat("_SnowOpacity", parameters.SnowOpacity);
        material.SetFloat("_Height", parameters.Height);
    }

    private void AffectFeatureByBiomeColor(Material material, ShaderParameters parameters, Color biomeTint, float noiseValue)
    {
        Color baseColor = parameters.BaseColor;
        Color interpolatedBaseColor = Color.Lerp(parameters.BaseColor, biomeTint, noiseValue * 1.5f);

        material.SetColor("_BaseColor", interpolatedBaseColor);
    }





}
