// using System;
// using Unity.Mathematics;
// 
// using UnityEngine;
// using UnityEngine.UI;

// public class HeightNoiseGenerator : NoiseGenerator
// {
//     // private static HeightMapSettings _settings;
    
//     // public HeightNoiseGenerator(HeightMapSettings settings) :
//     // {
//     //     _settings = settings;
//     // }

//     public static void GenerateFullNoiseMap(int mapSize){
 
//         // float[,] noiseMap = GenerateNoiseMap(mapSize);

//         AnimationCurve heightCurve_threadsafe = new AnimationCurve (_settings.HeightCurve.keys);
//         // int[] debugDistrib = new int[10];

//         // float[,] fallOffMap = GenerateFalloffMap(mapSize);

//         for(int y = 0; y < mapSize; y++){
//             for(int x = 0; x < mapSize; x++){
//                 float rawNoise = noiseMap[x, y];
//                 rawNoise = NoiseUtils.CDF(rawNoise, 0.5f, 0.2f);

//                 rawNoise = Mathf.Clamp(rawNoise, 0, 1);
//                 // int index = (int)(rawNoise / .1f);
//                 // debugDistrib[index] += 1;


//                 float finalValue = heightCurve_threadsafe.Evaluate(rawNoise); // height curve bound must be < 0 and > 1
//                 finalValue += _settings.BaseHeight;

//                 finalValue -= fallOffMap[x,y];

//                 finalValue = Mathf.Clamp(finalValue, 0, 1);

//                 // UpdateCacheValues(finalValue);
//                 NoiseUtils.RoundToNearestHeightStep(finalValue, HexMetrics.NbHeightSteps);

//                 noiseMap[x, y] = finalValue;
                
//             }
//         }
//         // for (int i = 0; i < 10; i ++){
//         //     Debug.Log("i = " + i + " : " + debugDistrib[i]);
//         // }
//         DataMap.Values = noiseMap;
//     }

//     private float[,] GenerateFalloffMap(int mapSize)
//     {
//         float[,] map = new float[mapSize, mapSize];

//         for (int y = 0; y < mapSize; y++)
//         {
//             for (int x = 0; x < mapSize; x++)
//             {
//                 float nx = x / (float)mapSize * 2 - 1;
//                 float ny = y / (float)mapSize * 2 - 1;

//                 float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
//                 map[x, y] = Evaluate(value);
//             }
//         }

//         return map;
//     }

//     float Evaluate(float value)
//     {
//         float a = _settings.FallOffMapSteepness;
//         float b = _settings.FallOffMapOffset;

//         return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
//     }

//     public float[,] GenerateVisibilityNoiseMap(int size, float[,] heightMap, float[,] moistureMap, int offsetX, int offsetY){
 
//         float[,] noiseMap = new float[size, size];

//         for(int y = 0; y < size; y++){
//             for(int x = 0; x < size; x++){
//                 //TODO add a threshold using noise, and modify the threshold depending on biome
//                 if(moistureMap[x + offsetX, y + offsetY] > 0.7f){
//                     float rawNoise = heightMap[x + offsetX, y + offsetY];
//                     float offset = 0.1f;

//                     // Remap the value based on distance to 0.35 and offset
//                     float distance = Mathf.Abs(rawNoise - 0.35f);
//                     if(distance <= offset){
//                         float t = distance / offset;
//                         float mappedValue = 1 - t;
//                         noiseMap[x, y] = mappedValue;
//                     } else {
//                         noiseMap[x, y] = 0;
//                     }
//                 }
//             }
//         }
//         return noiseMap;
//     }

//     // public NoiseCache GetCache(){
//     //     return base._cache;
//     // }

// }
