using System.Collections.Generic;
using UnityEngine;

public class NoiseManager 
{
    private HeightNoiseGenerator heightNoiseGenerator;
    private MoistureNoiseGenerator moistureNoiseGenerator;
    private TemperatureNoiseGenerator temperatureNoiseGenerator;
    public Dictionary<DataMapSettings, NoiseGenerator> featureNoiseGenerators;

    private int size;

    public NoiseManager(HeightMapSettings heightSettings, MoistureMapSettings moistureSettings, TemperatureMapSettings temperatureSettings, List<DataMapSettings> featureSettingsList, int size)
    {
        this.size = size;

        // Create noise generators for each type of map
        heightNoiseGenerator = new HeightNoiseGenerator(heightSettings);
        moistureNoiseGenerator = new MoistureNoiseGenerator(moistureSettings);
        temperatureNoiseGenerator = new TemperatureNoiseGenerator(temperatureSettings);

        featureNoiseGenerators = new Dictionary<DataMapSettings, NoiseGenerator>();
        foreach (var setting in featureSettingsList)
        {
            featureNoiseGenerators[setting] = new NoiseGenerator(setting);
        }

        InitialiseNoiseGenerators();
    }

    //job is to get the min and max values to normalise the noise
    public void InitialiseNoiseGenerators(){
        for (int i = 0; i < size; i++){
            for (int j = 0; j < size; j++){
                heightNoiseGenerator.UpdateMinMax(heightNoiseGenerator.GetNoiseValue(i, j, size));
                moistureNoiseGenerator.UpdateMinMax(moistureNoiseGenerator.GetNoiseValue(i, j, size));
                temperatureNoiseGenerator.UpdateMinMax(temperatureNoiseGenerator.GetNoiseValue(i, j, size));
                foreach (var setting in featureNoiseGenerators.Keys)
                {
                    featureNoiseGenerators[setting].UpdateMinMax(featureNoiseGenerators[setting].GetNoiseValue(i, j, size));
                }
            }
        }
    }

    public float GetHeightNoiseValue(int x, int y)
    {
        return heightNoiseGenerator.GetNormalizedNoiseValue(x, y, size);
    }

    public float GetMoistureNoiseValue(int x, int y)
    {
        return moistureNoiseGenerator.GetNormalizedNoiseValue(x, y, size);
    }

    public float GetTemperatureNoiseValue(int x, int y)
    {
        return temperatureNoiseGenerator.GetNormalizedNoiseValue(x, y, size);
    }

    public float GetFeatureNoiseValue(DataMapSettings settings, int x, int y)
    {
        return featureNoiseGenerators[settings].GetNormalizedNoiseValue(x, y, size);
    }

    public void Debugging()
    {
        HeightNoiseGenerator heightNoiseGenerator_d = (HeightNoiseGenerator)heightNoiseGenerator;
        Debug.Log("height max: " + heightNoiseGenerator_d.GetCache().maxValue + " min :" + heightNoiseGenerator_d.GetCache().minValue);

        MoistureNoiseGenerator moistureNoiseGenerator_d = (MoistureNoiseGenerator)moistureNoiseGenerator;
        Debug.Log("moisture max: " + moistureNoiseGenerator_d.GetCache().maxValue + " min :" + moistureNoiseGenerator_d.GetCache().minValue);

        TemperatureNoiseGenerator temperatureNoiseGenerator_d = (TemperatureNoiseGenerator)temperatureNoiseGenerator;
        Debug.Log("temperature max: " + temperatureNoiseGenerator_d.GetCache().maxValue + " min :" + temperatureNoiseGenerator_d.GetCache().minValue);
    }
}
