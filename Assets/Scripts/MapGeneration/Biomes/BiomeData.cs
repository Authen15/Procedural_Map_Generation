using UnityEngine;

namespace Biome {

    [CreateAssetMenu(menuName = "Terrain/Biome")]
    public class BiomeData : ScriptableObject
    {
        // public string ShaderReferenceSuffix;
        // public BiomeType BiomeType;
        public Material BiomeMaterial;

        // [HideInInspector]
        // public int SubBiomeCount;

        public HeightMapSettings HeightMapSettings;

        // public Color BiomeColor; // used for preview map

        // [Header("Height Thresholds")]
        // public Vector2[] HeightThresholds;

        // [Header("Temperature Thresholds")]
        // public Vector2[] TemperatureThresholds;

        // [Header("Moisture Thresholds")]
        // public Vector2[] MoistureThresholds;


        // void OnValidate(){
        //     SubBiomeCount = TemperatureThresholds.Length;
        // }


    
    }
}





