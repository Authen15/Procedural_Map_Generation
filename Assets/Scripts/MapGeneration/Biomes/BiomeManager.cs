using UnityEngine;

namespace Biome {

    public enum BiomeType
    {
        BOREAL = 0,
        MESA = 1,
        DESERT = 2,
        ICE = 3,
        TOUNDRA = 4,
        JUNGLE = 5,
    }

    public class BiomeManager : MonoBehaviour
    {
        [SerializeField]
        private BiomeData[] _allBiomes;

        // public string[] ShaderReferenceSuffix = {
        //     "BOREAL",
        //     "MESA"
        // };


        private static BiomeManager _instance;
        public static BiomeManager Instance{
            get{
                if (_instance == null){
                    Debug.LogWarning("BiomeManager is null");
                }
                return _instance;
            }
        }
        
        void Awake(){
            _instance = this;
        }

        // public BiomeData GetBiome(float moisture, float temperature)
        // {
        //     for(int i = 0; i < _allBiomes.Length; i++){
        //         for(int j = 0; j < _allBiomes[i].subBiomeCount; j++)
        //         if(moisture >= _allBiomes[i].MoistureThresholds[j].x && moisture <= _allBiomes[i].MoistureThresholds[j].y &&
        //             temperature >= _allBiomes[i].TemperatureThresholds[j].x && temperature <= _allBiomes[i].TemperatureThresholds[j].y
        //             // height >= _allBiomes[i].HeightThresholds[j].x && height <= _allBiomes[i].HeightThresholds[j].y
        //             )
        //         {
        //             return _allBiomes[i];
        //         }
        //     }
        //     Debug.Log("No biome found for moisture: " + moisture + " and temperature: " + temperature);
        //     return _defaultBiome;
        // }

        public BiomeData GetBiome() //TODO make a more complex system for biome distribution
        {
            return _allBiomes[Random.Range(0, _allBiomes.Length)];
        }
    }
}
