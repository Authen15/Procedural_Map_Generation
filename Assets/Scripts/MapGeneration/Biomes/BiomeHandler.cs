public class BiomeHandler
{
    private Biome _defaultBiome;
    private Biome[] _allBiomes;

    public BiomeHandler(Biome[] allBiomes, Biome defaultBiome)
    {
        this._allBiomes = allBiomes;
        this._defaultBiome = defaultBiome;
    }

    public Biome GetBiome(float moisture, float temperature)
    {
        for(int i = 0; i < _allBiomes.Length; i++){
            for(int j = 0; j < _allBiomes[i].subBiomeCount; j++)
                if(moisture >= _allBiomes[i].minMoisture[j] && moisture <= _allBiomes[i].maxMoisture[j] &&
                temperature >= _allBiomes[i].minTemperature[j] && temperature <= _allBiomes[i].maxTemperature[j]){
                    return _allBiomes[i];
                }
        }
        UnityEngine.Debug.Log("No biome found for moisture: " + moisture + " and temperature: " + temperature);
        return _defaultBiome;
    }

    // public Biome GetBiomeFromPosition(){
        
    // }
}
