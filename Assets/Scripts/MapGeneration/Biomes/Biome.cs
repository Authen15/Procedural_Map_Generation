using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Biome")]
public class Biome : ScriptableObject
{
    // public BiomeLevel[] levels;
    public int subBiomeCount = 1; // default to 1

    [Header("Moisture Thresholds")]
    public float[] minMoisture;
    public float[] maxMoisture;

    [Header("Temperature Thresholds")]
    public float[] minTemperature;
    public float[] maxTemperature;

    public Biome()
    {
        minMoisture = new float[subBiomeCount];
        maxMoisture = new float[subBiomeCount];
        minTemperature = new float[subBiomeCount];
        maxTemperature = new float[subBiomeCount];
    }

    // [Header("Allowed Features")]

    // public Color biomeTint = Color.white;

    // public List<BiomeFeatureData> biomeFeatureData;
    
}




