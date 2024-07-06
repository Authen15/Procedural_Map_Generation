using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer))]
public class MapPreview : MonoBehaviour 
{
    // public HexGridManager HexGridManager;
    public bool AutoUpdate;
    // public int TextureSize;

    public HeightMapSettings heightMapSettings;
    public MoistureMapSettings moistureMapSettings;
    public TemperatureMapSettings temperatureMapSettings;
    public List<DataMapSettings> featureMapSettings;

    public enum NoiseMapType
    {
        HeightMap,
        MoistureMap,
        TemperatureMap,
        featureMaps
    }

    public DataMapSettings featureMapToDraw;
    public NoiseMapType selectedNoiseMapType;

    public bool IsThresholdTexture;
    [Range(0, 1)]
    public float thresholdValue = 0.5f;

    public void UpdateMapPreview()
    {
        int TextureSize = HexMetrics.MapSize;

        NoiseManager noiseManager = new NoiseManager(heightMapSettings, moistureMapSettings, temperatureMapSettings, featureMapSettings, TextureSize);

        DataMap heightMap = new DataMap(new float[TextureSize, TextureSize]);
        DataMap moistureMap = new DataMap(new float[TextureSize, TextureSize]);
        DataMap temperatureMap = new DataMap(new float[TextureSize, TextureSize]);
        DataMap featureMap = new DataMap(new float[TextureSize, TextureSize]);

        noiseManager.GetFullHeightDataMap(heightMap);
        noiseManager.GetFullMoistureDataMap(moistureMap);
        noiseManager.GetFullTemperatureDataMap(temperatureMap);
        // noiseManager.GetFullFeatureDataMap(featureMap, featureMapToDraw);
        
        // if (HexGridManager == null) return;

        // Ensure only data maps of HexGridManager are initialized
        // HexGridManager.InitializeDataMaps(heightMap, moistureMap, temperatureMap);
        // HexGridManager.noiseManager.InitialiseNoiseGenerators();


        float[,] noiseMap;

        switch (selectedNoiseMapType)
        {
            case NoiseMapType.HeightMap:
                noiseMap = heightMap.values;
                break;

            case NoiseMapType.MoistureMap:
                noiseMap = moistureMap.values;
                break;

            case NoiseMapType.TemperatureMap:
                noiseMap = temperatureMap.values;
                break;
            case NoiseMapType.featureMaps:
                noiseMap = featureMap.values;
                break;

            default:
                Debug.LogError("Unexpected noise map type: " + selectedNoiseMapType);
                return;  // Handle any unexpected cases
        }

        Texture2D texture;
        if(IsThresholdTexture)
        {
            texture = TextureFromDataMap(noiseMap, thresholdValue);
        }else{

            // Convert the noise map to a texture.
            texture = TextureFromDataMap(noiseMap);
        }
        
        // Apply texture to the MeshRenderer.
        GetComponent<MeshRenderer>().sharedMaterial.mainTexture = texture;
    }




    Texture2D TextureFromDataMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        Texture2D texture = new Texture2D(width, height);
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float value = heightMap[x, y];
                Color color = new Color(value, value, value, 1);
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }

    Texture2D TextureFromDataMap(float[,] heightMap, float thresholdValue)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        
        Texture2D texture = new Texture2D(width, height);
        
        // Loop through all pixels in the texture
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++) 
            {
                float value = heightMap[x, y];
                Color color = value > thresholdValue ? Color.white : Color.black;
                texture.SetPixel(x, y, color);
            }
        }
        
        texture.Apply();
        return texture;
    }


    // private float [,] CreateHeightValuesArray(int size){
    //     float[,] noise = new float[size, size ];

    //     // for(int i = 0; i < size; i++){
    //     //     for(int j = 0; j < size; j++){
    //     //         noise[i, j] =  hexGridManager.noiseManager.GetHeightNoiseValue(i,j);
    //     //     }
    //     // }
    //     noise = hexGridManager.noiseManager.GetFullHeightMap().values;
    //     return noise;
    // }


    // private float [,] CreateMoistureValuesArray(int size){
    //     float[,] noise = new float[size, size ];

    //     // for(int i = 0; i < size; i++){
    //     //     for(int j = 0; j < size; j++){
    //     //         noise[i, j] =  hexGridManager.noiseManager.GetMoistureNoiseValue(i,j);
    //     //     }
    //     // }
    //     noise = hexGridManager.noiseManager.GetFullMoistureMap().values;
    //     return noise;
    // }

    // private float [,] CreateTemperatureValuesArray(int size){
    //     float[,] noise = new float[size, size ];

    //     // for(int i = 0; i < size; i++){
    //     //     for(int j = 0; j < size; j++){
    //     //         noise[i, j] =  hexGridManager.noiseManager.GetTemperatureNoiseValue(i,j);
    //     //     }
    //     // }
    //     noise = hexGridManager.noiseManager.GetFullTemperatureMap().values;
    //     return noise;
    // }

    // private float [,] CreateFeatureMapValuesArray(int size, DataMapSettings featureMapSettings){
    //     float[,] noise = new float[size, size ];

    //     for(int i = 0; i < size; i++){
    //         for(int j = 0; j < size; j++){
    //             noise[i, j] =  hexGridManager.noiseManager.GetFeatureNoiseValue(featureMapSettings, i,j);
    //         }
    //     }
    //     return noise;
    // }

}
