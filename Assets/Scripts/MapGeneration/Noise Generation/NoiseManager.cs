using System.Collections.Generic;
using Biome;
using UnityEngine;

public class NoiseManager 
{
    // private HeightNoiseGenerator _heightNoiseGenerator { get;}
    // private MoistureNoiseGenerator _moistureNoiseGenerator { get;}
    // private TemperatureNoiseGenerator _temperatureNoiseGenerator { get; }


    // private BiomeManager _biomeManager;

    // public NoiseManager(HeightMapSettings heightSettings, MoistureMapSettings moistureSettings, TemperatureMapSettings temperatureSettings, BiomeManager biomeManager)
    // {

    //     // Create noise generators for each type of map
    //     // _heightNoiseGenerator = new HeightNoiseGenerator(heightSettings);
    //     // _moistureNoiseGenerator = new MoistureNoiseGenerator(moistureSettings);
    //     // _temperatureNoiseGenerator = new TemperatureNoiseGenerator(temperatureSettings);

    //     _biomeManager = biomeManager;

    //     InitialiseNoiseGenerators();
    // }

    // //job is to get the min and max values to normalise the noise
    // public void InitialiseNoiseGenerators(){
        // _heightNoiseGenerator.UpdateNoiseGlobalMinMax(HexMetrics.MapCellGridSize);
        // _moistureNoiseGenerator.UpdateNoiseGlobalMinMax(HexMetrics.MapCellGridSize);
        // _temperatureNoiseGenerator.UpdateNoiseGlobalMinMax(HexMetrics.MapCellGridSize);
    // }

    // public float[,] GetChunkHeightMap(Chunk chunk)
    // {
    //     return _heightNoiseGenerator.GetChunkNoiseMap(chunk);
    // }

    // public float[,] GetChunkMoistureMap(Chunk chunk)
    // {
    //     return _moistureNoiseGenerator.GetChunkNoiseMap(chunk);
    // }

    // public float[,] GetChunkTemperatureMap(Chunk chunk)
    // {
    //     return _temperatureNoiseGenerator.GetChunkNoiseMap(chunk);
    // }

    // public float GetFeatureNoiseValue(DataMapSettings settings, int x, int y)
    // {
    //     return FeatureNoiseGenerators[settings].GetNoiseValue(x, y, _mapSize);
    // }

    // public void Debugging()
    // {
    //     HeightNoiseGenerator heightNoiseGenerator_d = (HeightNoiseGenerator)_heightNoiseGenerator;
    //     Debug.Log("height max: " + heightNoiseGenerator_d.GetCache().maxValue + " min :" + heightNoiseGenerator_d.GetCache().minValue);

    //     MoistureNoiseGenerator moistureNoiseGenerator_d = (MoistureNoiseGenerator)_moistureNoiseGenerator;
    //     Debug.Log("moisture max: " + moistureNoiseGenerator_d.GetCache().maxValue + " min :" + moistureNoiseGenerator_d.GetCache().minValue);

    //     TemperatureNoiseGenerator temperatureNoiseGenerator_d = (TemperatureNoiseGenerator)_temperatureNoiseGenerator;
    //     Debug.Log("temperature max: " + temperatureNoiseGenerator_d.GetCache().maxValue + " min :" + temperatureNoiseGenerator_d.GetCache().minValue);
    // }

    // public DataMap GetFullHeightDataMap()
    // {
    //     DataMap heightMap = _heightNoiseGenerator.DataMap;
    //     if (!heightMap.IsGenerated){
    //         _heightNoiseGenerator.GenerateFullNoiseMap(HexMetrics.MapCellGridSize);
    //         heightMap.IsGenerated = true;
    //     }
    //     return heightMap;
    // }


    // public DataMap GetFullMoistureDataMap()
    // {
    //     DataMap moistureMap = _moistureNoiseGenerator.DataMap;
    //     if (!moistureMap.IsGenerated){
    //         _moistureNoiseGenerator.GenerateFullNoiseMap(HexMetrics.MapCellGridSize);
    //         moistureMap.IsGenerated = true;
    //     }
    //     return moistureMap;
    // }


    // public DataMap GetFullTemperatureDataMap()
    // {
    //     DataMap temperatureMap = _temperatureNoiseGenerator.DataMap;
    //     if (!temperatureMap.IsGenerated){
    //         _temperatureNoiseGenerator.GenerateFullNoiseMap(HexMetrics.MapCellGridSize);
    //         temperatureMap.IsGenerated = true;
    //     }
    //     return temperatureMap;
    // }

    // public BiomeData[,] GetFullBiomeMap(){

    //     DataMap moistureMap = GetFullMoistureDataMap();
    //     DataMap temperatureMap = GetFullTemperatureDataMap();
    //     DataMap heightMap = GetFullHeightDataMap();

    //     BiomeData[,] biomeDatas = new BiomeData[HexMetrics.MapCellGridSize,HexMetrics.MapCellGridSize];
    //     for (int y = 0; y < HexMetrics.MapCellGridSize; y++){
    //         for (int x = 0; x < HexMetrics.MapCellGridSize; x++){
    //             float moisture = moistureMap.Values[x,y];
    //             float temperature = temperatureMap.Values[x,y];
    //             float height = heightMap.Values[x,y];
    //             BiomeData biomeData = _biomeManager.GetBiome(moisture, temperature);

    //             biomeDatas[x,y] = biomeData;
    //         }
    //     }
    //     return biomeDatas;
    // }

    // public void GetFullFeatureDataMap(DataMap featureMap, DataMapSettings settings)
    // {
    //     featureMap.Values =  FeatureNoiseGenerators[settings].GetNoiseMap(HexMetrics.MapSize);
    // }

    // public float[,] GetChunkVisibilityNoiseMap(DataMap heightMap, DataMap featureMap, Vector2Int chunkPosition, int chunkSize)
    // {
    //     return _heightNoiseGenerator.GenerateVisibilityNoiseMap(chunkSize, heightMap.Values, featureMap.Values, chunkPosition.x * chunkSize, chunkPosition.y * chunkSize);
    // }



}
