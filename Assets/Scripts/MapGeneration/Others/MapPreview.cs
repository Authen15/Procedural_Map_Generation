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

        // Ensure only data maps of HexGridManager are initialized
        hexGridManager.InitializeDataMaps();
        hexGridManager.noiseManager.InitialiseNoiseGenerators();

        float[,] noiseMap;

        switch (selectedNoiseMapType)
        {
            case NoiseMapType.HeightMap:
                noiseMap = CreateHeightValuesArray(hexGridManager.mapSize);
                break;

            case NoiseMapType.MoistureMap:
                noiseMap = CreateMoistureValuesArray(hexGridManager.mapSize);
                break;

            case NoiseMapType.TemperatureMap:
                noiseMap = CreateTemperatureValuesArray(hexGridManager.mapSize);
                break;
            case NoiseMapType.featureMaps:
                noiseMap = CreateFeatureMapValuesArray(hexGridManager.mapSize, featureMapToDraw);
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


    private float [,] CreateHeightValuesArray(int size){
        float[,] noise = new float[size, size ];

        for(int i = 0; i < size; i++){
            for(int j = 0; j < size; j++){
                noise[i, j] =  hexGridManager.noiseManager.GetHeightNoiseValue(i,j);
            }
        }
        return noise;
    }


    private float [,] CreateMoistureValuesArray(int size){
        float[,] noise = new float[size, size ];

        for(int i = 0; i < size; i++){
            for(int j = 0; j < size; j++){
                noise[i, j] =  hexGridManager.noiseManager.GetMoistureNoiseValue(i,j);
            }
        }
        return noise;
    }

    private float [,] CreateTemperatureValuesArray(int size){
        float[,] noise = new float[size, size ];

        for(int i = 0; i < size; i++){
            for(int j = 0; j < size; j++){
                noise[i, j] =  hexGridManager.noiseManager.GetTemperatureNoiseValue(i,j);
            }
        }
        return noise;
    }

    private float [,] CreateFeatureMapValuesArray(int size, DataMapSettings featureMapSettings){
        float[,] noise = new float[size, size ];

        for(int i = 0; i < size; i++){
            for(int j = 0; j < size; j++){
                noise[i, j] =  hexGridManager.noiseManager.GetFeatureNoiseValue(featureMapSettings, i,j);
            }
        }
        return noise;
    }

}
