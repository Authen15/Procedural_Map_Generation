using UnityEngine;

public class TemperatureNoiseGenerator : NoiseGenerator
{
    private float latitudeSensitivity;
    private float equatorBias;
    private float elevationSensitivity;
    private float baseTemperature;

    public TemperatureNoiseGenerator(TemperatureMapSettings settings) : base(settings)
    {
        this.latitudeSensitivity = settings.latitudeSensitivity;
        this.equatorBias = settings.equatorBias;
        this.elevationSensitivity = settings.elevationSensitivity;
        this.baseTemperature = settings.baseTemperature;
    }


    // public float[,] GetChunkNoiseMap(Chunk chunk){
    //     int size = chunk.size;
    //     int xOffset = chunk.x * size;
    //     int yOffset = chunk.z * size;
    //     float[,] noiseMap = GetNoiseMap(size, xOffset, yOffset);

    //     // for(int y = 0; y < size; y++){
    //     //     for(int x = 0; x < size; x++){
    //     //         float rawNoise = noiseMap[x, y];
    //     //         float latitudeEffect = Mathf.Abs((float)y / size - 0.5f) * latitudeSensitivity;
    //     //         float latitudeAdjustedNoise = 1 - latitudeEffect + equatorBias;

    //     //         latitudeAdjustedNoise -= rawNoise * elevationSensitivity;

    //     //         float finalValue = latitudeAdjustedNoise + rawNoise;

    //     //         if(finalValue > 1) Debug.Log("Noise value > 1 in TemperatureNoiseGenerator");
    //     //         if(finalValue < 0) Debug.Log("Noise value < 0 in TemperatureNoiseGenerator");

    //     //         noiseMap[x, y] = finalValue;
    //     //     }
    //     // }
    //     // float minLatitudeEffect = 0;
    //     // float maxLatitudeEffect = latitudeSensitivity * 0.5f;
    //     // float minElevationEffect = -elevationSensitivity;
    //     // float maxElevationEffect = 0;

    //     // float minFinalValue = 1 - maxLatitudeEffect + equatorBias + minElevationEffect + 0;
    //     // float maxFinalValue = 1 - minLatitudeEffect + equatorBias + maxElevationEffect + 1;

    //     // for (int y = 0; y < size; y++)
    //     // {
    //     //     for (int x = 0; x < size; x++)
    //     //     {
    //     //         float rawNoise = noiseMap[x, y];
    //     //         float latitudeEffect = Mathf.Abs((float)y / size - 0.5f) * latitudeSensitivity;
    //     //         float latitudeAdjustedNoise = 1 - latitudeEffect + equatorBias;

    //     //         latitudeAdjustedNoise -= rawNoise * elevationSensitivity;

    //     //         float finalValue = latitudeAdjustedNoise + rawNoise;

    //     //         float normalizedValue = Mathf.InverseLerp(minFinalValue, maxFinalValue, finalValue);
    //     //         noiseMap[x, y] = normalizedValue;

    //     //         if (normalizedValue > 1) Debug.Log("Noise value > 1 in TemperatureNoiseGenerator");
    //     //         if (normalizedValue < 0) Debug.Log("Noise value < 0 in TemperatureNoiseGenerator");
    //     //     }
    //     // }
    //     return noiseMap;
    // }

    public float[,] GetFullNoiseMap(int mapSize){
 
        float[,] noiseMap = GetNoiseMap(mapSize);

        float minLatitudeEffect = 0;
        float maxLatitudeEffect = latitudeSensitivity * 0.5f;
        float minElevationEffect = -elevationSensitivity;
        float maxElevationEffect = 0;

        float minFinalValue = 1 - maxLatitudeEffect + equatorBias + minElevationEffect + 0;
        float maxFinalValue = 1 - minLatitudeEffect + equatorBias + maxElevationEffect + 1;

        for (int y = 0; y < mapSize; y++)
        {
            for (int x = 0; x < mapSize; x++)
            {
                float rawNoise = noiseMap[x, y];
                float latitudeEffect = Mathf.Abs((float)y / mapSize - 0.5f) * latitudeSensitivity;
                float latitudeAdjustedNoise = 1 - latitudeEffect + equatorBias;

                latitudeAdjustedNoise -= rawNoise * elevationSensitivity;

                float finalValue = latitudeAdjustedNoise + rawNoise + baseTemperature;

                // UpdateCacheValues(finalValue);
                // noiseMap[x, y] = finalValue;
                finalValue = Mathf.InverseLerp(minFinalValue, maxFinalValue, finalValue);

                finalValue = Mathf.Clamp(finalValue, 0, 1);
                noiseMap[x, y] = finalValue;

                // if (normalizedValue > 1) Debug.Log("Noise value > 1 in TemperatureNoiseGenerator");
                // if (normalizedValue < 0) Debug.Log("Noise value < 0 in TemperatureNoiseGenerator");
            }
        }
        return noiseMap;
    }

    public NoiseCache GetCache(){
        return base._cache;
    }
}
