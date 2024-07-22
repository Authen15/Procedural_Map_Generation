using System;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class MoistureNoiseGenerator : NoiseGenerator
{
    private float baseMoisture;

    public MoistureNoiseGenerator(MoistureMapSettings settings) : base(settings)
    {
        this.baseMoisture = settings.baseMoisture;
    }


    // public float[,] GetChunkNoiseMap(Chunk chunk){
    //     int size = chunk.size;
    //     int xOffset = chunk.x * size;
    //     int yOffset = chunk.z * size;
    //     float[,] noiseMap = GetNoiseMap(size, xOffset, yOffset);
    //     return noiseMap;
    // }

    public void GenerateFullNoiseMap(int mapSize){
 
        float[,] noiseMap = GenerateNoiseMap(mapSize);

        for(int y = 0; y < mapSize; y++){
            for(int x = 0; x < mapSize; x++){
                float finalValue = noiseMap[x, y];
                finalValue += baseMoisture;
                
                // UpdateCacheValues(finalValue);
                // if(finalValue > 1) Debug.Log("Noise value > 1 in MoistureNoiseGenerator");
                // if(finalValue < 0) Debug.Log("Noise value < 0 in MoistureNoiseGenerator");
                finalValue = Mathf.Clamp(finalValue, 0, 1);
                noiseMap[x, y] = finalValue;
            }
        } 
        DataMap.Values = noiseMap;
    }

    public NoiseCache GetCache(){
        return base._cache;
    }
}
