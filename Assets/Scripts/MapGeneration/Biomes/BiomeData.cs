using UnityEngine;

namespace Biome {

    [CreateAssetMenu(menuName = "Terrain/Biome")]
    public class BiomeData : ScriptableObject
    {
        public BiomeType BiomeType;

        [HideInInspector]
        public int subBiomeCount;

        public Color biomeColor; // used for preview map

        // [Header("Height Thresholds")]
        // public Vector2[] HeightThresholds;

        [Header("Temperature Thresholds")]
        public Vector2[] TemperatureThresholds;

        [Header("Moisture Thresholds")]
        public Vector2[] MoistureThresholds;


        void OnValidate(){
            subBiomeCount = TemperatureThresholds.Length;
        }


    
    }
}





