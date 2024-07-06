using System.Collections.Generic;
using UnityEngine;

public class NoiseManager 
{
    private HeightNoiseGenerator _heightNoiseGenerator { get;}
    private MoistureNoiseGenerator _moistureNoiseGenerator { get;}
    private TemperatureNoiseGenerator _temperatureNoiseGenerator { get; }
    public Dictionary<DataMapSettings, NoiseGenerator> FeatureNoiseGenerators;

    private int _mapSize;

    public NoiseManager(HeightMapSettings heightSettings, MoistureMapSettings moistureSettings, TemperatureMapSettings temperatureSettings, List<DataMapSettings> featureSettingsList, int size)
    {
        this._mapSize = size;

        // Create noise generators for each type of map
        _heightNoiseGenerator = new HeightNoiseGenerator(heightSettings);
        _moistureNoiseGenerator = new MoistureNoiseGenerator(moistureSettings);
        _temperatureNoiseGenerator = new TemperatureNoiseGenerator(temperatureSettings);

        // FeatureNoiseGenerators = new Dictionary<DataMapSettings, NoiseGenerator>();
        // foreach (var setting in featureSettingsList)
        // {
        //     FeatureNoiseGenerators[setting] = new NoiseGenerator(setting);
        // }

        InitialiseNoiseGenerators();
    }

    // //job is to get the min and max values to normalise the noise
    public void InitialiseNoiseGenerators(){
        _heightNoiseGenerator.UpdateNoiseGlobalMinMax(_mapSize);
        _moistureNoiseGenerator.UpdateNoiseGlobalMinMax(_mapSize);
        _temperatureNoiseGenerator.UpdateNoiseGlobalMinMax(_mapSize);
        // foreach (var setting in FeatureNoiseGenerators.Keys)
        // {
        //     FeatureNoiseGenerators[setting].UpdateNoiseGlobalMinMax(_mapSize);
        // }
    }

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

    public void Debugging()
    {
        HeightNoiseGenerator heightNoiseGenerator_d = (HeightNoiseGenerator)_heightNoiseGenerator;
        Debug.Log("height max: " + heightNoiseGenerator_d.GetCache().maxValue + " min :" + heightNoiseGenerator_d.GetCache().minValue);

        MoistureNoiseGenerator moistureNoiseGenerator_d = (MoistureNoiseGenerator)_moistureNoiseGenerator;
        Debug.Log("moisture max: " + moistureNoiseGenerator_d.GetCache().maxValue + " min :" + moistureNoiseGenerator_d.GetCache().minValue);

        TemperatureNoiseGenerator temperatureNoiseGenerator_d = (TemperatureNoiseGenerator)_temperatureNoiseGenerator;
        Debug.Log("temperature max: " + temperatureNoiseGenerator_d.GetCache().maxValue + " min :" + temperatureNoiseGenerator_d.GetCache().minValue);
    }

    public void GetFullHeightDataMap(DataMap heightMap)
    {
        heightMap.values = _heightNoiseGenerator.GetFullNoiseMap(_mapSize);
        heightMap.maxValue = _heightNoiseGenerator.GetCache().maxValue;
        heightMap.minValue = _heightNoiseGenerator.GetCache().minValue;
    }


    public void GetFullMoistureDataMap(DataMap moistureMap)
    {
        moistureMap.values = _moistureNoiseGenerator.GetFullNoiseMap(_mapSize);
        moistureMap.maxValue = _moistureNoiseGenerator.GetCache().maxValue;
        moistureMap.minValue = _moistureNoiseGenerator.GetCache().minValue;
    }


    public void GetFullTemperatureDataMap(DataMap temperatureMap)
    {
        temperatureMap.values = _temperatureNoiseGenerator.GetFullNoiseMap(_mapSize);
        temperatureMap.maxValue = _temperatureNoiseGenerator.GetCache().maxValue;
        temperatureMap.minValue = _temperatureNoiseGenerator.GetCache().minValue;
    }

    // public void GetFullFeatureDataMap(DataMap featureMap, DataMapSettings settings)
    // {
    //     featureMap.values =  FeatureNoiseGenerators[settings].GetNoiseMap(_mapSize);
    // }

    public float[,] GetChunkVisibilityNoiseMap(DataMap heightMap, DataMap featureMap, Vector2Int chunkPosition, int chunkSize)
    {
        return _heightNoiseGenerator.GetVisibilityNoiseMap(chunkSize, heightMap.values, featureMap.values, chunkPosition.x * chunkSize, chunkPosition.y * chunkSize);
    }



}
