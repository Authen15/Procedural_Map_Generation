using System.Collections.Generic;
using Biome;
using Unity.Mathematics;
using UnityEngine;

public enum NoiseMapType
{
    HeightMap,
    MoistureMap,
    TemperatureMap,
    BiomeMap,
}

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer))]
public class MapPreview : MonoBehaviour 
{
    public bool AutoUpdate;
    public bool WriteTexture;


    public BiomeManager BiomeManager;
    public HeightMapSettings HeightMapSettings;
    public MoistureMapSettings MoistureMapSettings;
    public TemperatureMapSettings TemperatureMapSettings;

    

    public NoiseMapType SelectedNoiseMapType;


    public bool IsThresholdTexture;
    [Range(0, 1)]
    public float MinThresholdValue = 0;
    [Range(0, 1)]
    public float MaxThresholdValue = 1;

    public void UpdateMapPreview()
    {
        NoiseManager noiseManager = new NoiseManager(HeightMapSettings, MoistureMapSettings, TemperatureMapSettings, BiomeManager);

        Texture2D texture;


        if (SelectedNoiseMapType != NoiseMapType.BiomeMap){
            float[,] noiseMap;
            switch (SelectedNoiseMapType)
            {
                case NoiseMapType.HeightMap:
                    noiseMap = noiseManager.GetFullHeightDataMap().Values;
                    break;

                case NoiseMapType.MoistureMap:
                    noiseMap = noiseManager.GetFullMoistureDataMap().Values;
                    break;

                case NoiseMapType.TemperatureMap:
                    noiseMap = noiseManager.GetFullTemperatureDataMap().Values;
                    break;

                default:
                    Debug.LogError("Unexpected noise map type: " + SelectedNoiseMapType);
                    return;
            }

            if(IsThresholdTexture)
                noiseMap = BinaryNoiseMap(noiseMap);

            texture = TextureGenerator.TextureFromNoiseMap(noiseMap);

        }
        else{
            BiomeData[,] biomeTypeMap = noiseManager.GetFullBiomeMap();
            Color[] biomeColorMap = BiomeColorMap(biomeTypeMap);

            texture = TextureGenerator.TextureFromColourMap(biomeColorMap, HexMetrics.MapSize);
        }


        

        
        

        if (WriteTexture){
            switch (SelectedNoiseMapType)
            {
                case NoiseMapType.HeightMap:
                    TextureGenerator.WriteTexture(texture, "Assets/Textures/HeightMap.jpeg");
                    break;

                case NoiseMapType.MoistureMap:
                    TextureGenerator.WriteTexture(texture, "Assets/Textures/MoistureMap.jpeg");
                    break;

                case NoiseMapType.TemperatureMap:
                    TextureGenerator.WriteTexture(texture, "Assets/Textures/TemperatureMap.jpeg");
                    break;
            }
        }
        
        // Apply texture to the MeshRenderer.
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }

    float[,] BinaryNoiseMap(float[,] noiseMap)
    {        
        // Loop through all pixels in the texture
        for (int y = 0; y < HexMetrics.MapSize; y++)
        {
            for (int x = 0; x < HexMetrics.MapSize; x++) 
            {
                float value = noiseMap[x, y];
                value = (value >= MinThresholdValue && value <= MaxThresholdValue) ? 1 : 0;
                noiseMap[x,y] = value;
            }
        }
        return noiseMap;
    }

    Color[] BiomeColorMap(BiomeData[,] biomeDatas){
        Color[] colors = new Color[HexMetrics.MapSize * HexMetrics.MapSize];
        for (int y = 0; y < HexMetrics.MapSize; y++){
            for (int x = 0; x < HexMetrics.MapSize; x++){
                colors[y * HexMetrics.MapSize + x] = biomeDatas[x,y].biomeColor;
            }
        }
        return colors;
    }
}
