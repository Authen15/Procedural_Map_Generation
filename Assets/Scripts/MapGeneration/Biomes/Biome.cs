using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/Biome")]
public class Biome : ScriptableObject
{
    public BiomeLevel[] levels;

    [Header("Moisture Thresholds")]
    public float minMoisture;
    public float maxMoisture;

    [Header("Temperature Thresholds")]
    public float minTemperature;
    public float maxTemperature;

    [Header("Allowed Features")]

    public Color biomeTint = Color.white;

    public List<BiomeFeatureData> biomeFeatureData;

    
}




