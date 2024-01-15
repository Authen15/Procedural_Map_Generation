using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(MeshRenderer))]
public class MapPreview : MonoBehaviour 
{
    public HexGridManager hexGridManager;
    public bool autoUpdate;

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
        if (hexGridManager == null) return;

        // Ensure data maps of HexGridManager are initialized
        hexGridManager.InitializeDataMaps();

        float[,] noiseMap;

        switch (selectedNoiseMapType)
        {
            case NoiseMapType.HeightMap:
                noiseMap = hexGridManager.dataMapManager.heightMap.values;
                break;

            case NoiseMapType.MoistureMap:
                noiseMap = hexGridManager.dataMapManager.moistureMap.values;
                break;

            case NoiseMapType.TemperatureMap:
                noiseMap = hexGridManager.dataMapManager.temperatureMap.values;
                break;
            case NoiseMapType.featureMaps:
                noiseMap = hexGridManager.dataMapManager.featureMaps[featureMapToDraw].values;
                break;

            default:
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

}
