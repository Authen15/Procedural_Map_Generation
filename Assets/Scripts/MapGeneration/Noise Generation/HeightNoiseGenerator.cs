using System;
using Unity.VisualScripting;
using UnityEngine;

public class HeightNoiseGenerator : NoiseGenerator
{
    private AnimationCurve heightCurve;
    private float baseHeight;

    public HeightNoiseGenerator(HeightMapSettings settings) : base(settings)
    {
        this.heightCurve = settings.heightCurve;
        this.baseHeight = settings.baseHeight;
    }

    // public float[,] GetChunkNoiseMap(Chunk chunk){
    //     int size = chunk.size;
    //     int xOffset = chunk.x * size;
    //     int yOffset = chunk.z * size;
    //     float[,] noiseMap = GetNoiseMap(size, xOffset, yOffset);

    //     AnimationCurve heightCurve_threadsafe = new AnimationCurve (heightCurve.keys);

    //     for(int y = 0; y < size; y++){
    //         for(int x = 0; x < size; x++){
    //             float rawNoise = noiseMap[x, y];
    //             float finalValue = heightCurve_threadsafe.Evaluate(rawNoise); // height curve bound must be < 0 and > 1

    //             if(finalValue > 1) Debug.Log("Noise value > 1 in HeightNoiseGenerator");
    //             if(finalValue < 0) Debug.Log("Noise value < 0 in HeightNoiseGenerator");

    //             noiseMap[x, y] = finalValue;
    //         }
    //     } 
    //     return noiseMap;
    // }

    public float[,] GetFullNoiseMap(int mapSize){
 
        float[,] noiseMap = GetNoiseMap(mapSize);

        AnimationCurve heightCurve_threadsafe = new AnimationCurve (heightCurve.keys);

        for(int y = 0; y < mapSize; y++){
            for(int x = 0; x < mapSize; x++){
                float rawNoise = noiseMap[x, y];
                float finalValue = heightCurve_threadsafe.Evaluate(rawNoise); // height curve bound must be < 0 and > 1
                finalValue += baseHeight;

                finalValue = Mathf.Clamp(finalValue, 0, 1);
                // if(finalValue > 1) Debug.Log("Noise value > 1 in HeightNoiseGenerator");
                // if(finalValue < 0) Debug.Log("Noise value < 0 in HeightNoiseGenerator");

                finalValue = MathF.Log(finalValue + 1, 2);
                // UpdateCacheValues(finalValue);
                NoiseUtils.RoundToNearestHeightStep(finalValue, HexMetrics.nbHeightSteps);
                
                noiseMap[x, y] = finalValue;
                
            }
        }
        return noiseMap;
    }

    public float[,] GetVisibilityNoiseMap(int size, float[,] heightMap, float[,] moistureMap, int offsetX, int offsetY){
 
        float[,] noiseMap = new float[size, size];

        for(int y = 0; y < size; y++){
            for(int x = 0; x < size; x++){
                //TODO add a threshold using noise, and modify the threshold depending on biome
                if(moistureMap[x + offsetX, y + offsetY] > 0.7f){
                    float rawNoise = heightMap[x + offsetX, y + offsetY];
                    float offset = 0.1f;

                    // Remap the value based on distance to 0.35 and offset
                    float distance = Mathf.Abs(rawNoise - 0.35f);
                    if(distance <= offset){
                        float t = distance / offset;
                        float mappedValue = 1 - t;
                        noiseMap[x, y] = mappedValue;
                    } else {
                        noiseMap[x, y] = 0;
                    }
                }
            }
        }
        return noiseMap;
    }

    public NoiseCache GetCache(){
        return base._cache;
    }

}
