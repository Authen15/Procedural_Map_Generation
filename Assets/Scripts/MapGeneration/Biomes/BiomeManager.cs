using UnityEngine;

namespace Biome {

    public enum BiomeType
    {
        Default = -1,
        Boreal = 0,
        Mesa = 1,
        Desert = 2,
        Ice = 3,
    }

    public class BiomeManager : MonoBehaviour
    {

        [SerializeField]
        private BiomeData _defaultBiome;
        [SerializeField]
        private BiomeData[] _allBiomes;

        public BiomeData GetBiome(float moisture, float temperature)
        {
            for(int i = 0; i < _allBiomes.Length; i++){
                for(int j = 0; j < _allBiomes[i].subBiomeCount; j++)
                if(moisture >= _allBiomes[i].MoistureThresholds[j].x && moisture <= _allBiomes[i].MoistureThresholds[j].y &&
                    temperature >= _allBiomes[i].TemperatureThresholds[j].x && temperature <= _allBiomes[i].TemperatureThresholds[j].y
                    // height >= _allBiomes[i].HeightThresholds[j].x && height <= _allBiomes[i].HeightThresholds[j].y
                    )
                {
                    return _allBiomes[i];
                }
            }
            Debug.Log("No biome found for moisture: " + moisture + " and temperature: " + temperature);
            return _defaultBiome;
        }

        // public Biome GetBiomeFromPosition(){
            
        // }
    }
}
