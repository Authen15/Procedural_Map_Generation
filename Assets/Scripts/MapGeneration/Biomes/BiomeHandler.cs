using System.Linq;

public class BiomeHandler
{
    private Biome defaultBiome;
    private Biome[] allBiomes;

    public BiomeHandler(Biome[] allBiomes, Biome defaultBiome)
    {
        this.allBiomes = allBiomes;
        this.defaultBiome = defaultBiome;
    }

    public Biome GetBiome(float moisture, float temperature)
    {
        Biome matchingBiome = allBiomes.FirstOrDefault(biome =>
            moisture >= biome.minMoisture && moisture <= biome.maxMoisture &&
            temperature >= biome.minTemperature && temperature <= biome.maxTemperature);

        return matchingBiome ?? defaultBiome;
    }

    // public Biome GetBiomeFromPosition(){
        
    // }

    public BiomeLevel GetBiomeLevel(Biome biome, float height)
    {
        return biome.levels.FirstOrDefault(level => height >= level.minHeight && height <= level.maxHeight);
    }
}
