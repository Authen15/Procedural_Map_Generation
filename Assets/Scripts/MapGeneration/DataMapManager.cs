using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataMapManager 
{
    public DataMap heightMap;
    public DataMap moistureMap;
    public DataMap temperatureMap;
    public DataMap featureMap;

    private HeightMapSettings HeightMapSettings;
    private HeightMapSettings MoistureMapSettings;
    private TemperatureMapSettings TemperatureMapSettings;
    public Dictionary<DataMapSettings, DataMap> featureMaps = new Dictionary<DataMapSettings, DataMap>();

    private int size;

    public DataMapManager(HeightMapSettings heightSettings, HeightMapSettings moistureSettings, TemperatureMapSettings temperatureSettings, List<DataMapSettings> featureSettingsList, int size)
    {
        this.HeightMapSettings = heightSettings;
        this.MoistureMapSettings = moistureSettings;
        this.TemperatureMapSettings = temperatureSettings;
        this.size = size;

        foreach (var setting in featureSettingsList)
        {
            featureMaps[setting] = null; // Just set it to null first
        }
    }

    public void GenerateDataMaps() 
    {
        heightMap = DataMapGenerator.GenerateHeightDataMap(size, HeightMapSettings, Vector2.zero);

        // moistureMap = DataMapGenerator.GenerateDataMap(size, MoistureMapSettings, Vector2.zero);
        moistureMap = DataMapGenerator.GenerateHeightDataMap(size, MoistureMapSettings, Vector2.zero);


        // Generate temperature map based on the height map
        temperatureMap = DataMapGenerator.GenerateTemperatureDataMap(size, heightMap, TemperatureMapSettings, Vector2.zero);

        // Generate feature maps based on the settings
        foreach (var setting in featureMaps.Keys.ToList()) // Using ToList to allow modification during iteration
        {
            featureMaps[setting] = new DataMap(DataMapGenerator.GenerateDataMap(size, setting, Vector2.zero).values);
        }
    }

    // Add a method to fetch the correct noise map
    public DataMap GetFeatureDataMap(DataMapSettings settings)
    {
        return featureMaps[settings];
    }

    public float RoundToNearestHeightStep(float value, int steps) 
    {
        float stepValue = 1f / steps;
        return Mathf.Round(value * steps) * stepValue;
    }
         
}
