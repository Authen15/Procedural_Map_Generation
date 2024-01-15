using System.Linq;
using UnityEngine;

public class BiomeHandling
{
    private Biome defaultBiome;
    private Biome[] allBiomes;

    public BiomeHandling(Biome[] allBiomes, Biome defaultBiome)
    {
        this.allBiomes = allBiomes;
        this.defaultBiome = defaultBiome;
    }

    public Biome GetBiome(float height, float moisture, float temperature)
    {
        Biome matchingBiome = allBiomes.FirstOrDefault(biome =>
            moisture >= biome.minMoisture && moisture <= biome.maxMoisture &&
            temperature >= biome.minTemperature && temperature <= biome.maxTemperature);

        return matchingBiome ?? defaultBiome;
    }

    public BiomeLevel GetBiomeLevel(Biome biome, float height)
    {
        return biome.levels.FirstOrDefault(level => height >= level.minHeight && height <= level.maxHeight);
    }
}
