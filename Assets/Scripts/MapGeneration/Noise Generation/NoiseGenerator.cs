using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using UnityEngine.PlayerLoop;

public class NoiseGenerator
{
    protected NoiseCache cache;

    public NoiseGenerator(DataMapSettings dataMapSettings)
    {
        cache = new NoiseCache(dataMapSettings);
    }

    public virtual float GetNormalizedNoiseValue(int x, int y, int size)
    {
        float normalizedValue = Mathf.InverseLerp(cache.minValue, cache.maxValue, GetNoiseValue(x, y, size));
        return normalizedValue;
    }

    public virtual float GetNoiseValue(int x, int y, int size)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseValue = 0;
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;

        for (int i = 0; i < cache.Settings.octaves; i++)
        {
            float sampleX = (x - halfWidth + cache.OctaveOffsets[i].x) / cache.Settings.scale * frequency;
            float sampleY = (y - halfHeight + cache.OctaveOffsets[i].y) / cache.Settings.scale * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseValue += perlinValue * amplitude;
            amplitude *= cache.Settings.persistance;
            frequency *= cache.Settings.lacunarity;
        }


        return noiseValue;
    }

    public void UpdateMinMax(float value)
    {
        if (value < cache.minValue) cache.minValue = value;
        if (value > cache.maxValue) cache.maxValue = value;
    }
}

public class NoiseCache
{
    public Vector2[] OctaveOffsets { get; private set; }
    public NoiseSettings Settings { get; private set; }
    public float minValue = float.MaxValue;
    public float maxValue = float.MinValue;

    public NoiseCache(DataMapSettings dataMapSettings)
    {
        // Debug.Log(dataMapSettings.noiseSettings.scale);
        Settings = dataMapSettings.noiseSettings;
        OctaveOffsets = new Vector2[Settings.octaves];
        System.Random prng = new System.Random(Settings.seed);
        float amplitude = 1;

        for (int i = 0; i < Settings.octaves; i++)
        {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            OctaveOffsets[i] = new Vector2(offsetX, offsetY);
            amplitude *= Settings.persistance;  
        }
    }

}

[System.Serializable]
public class NoiseSettings
{
    public float scale = 50;
    public int octaves = 6;
    [Range(0,1)]
    public float persistance = .6f;
    public float lacunarity = 2;
    public int seed;

    public NoiseSettings(float scale, int octaves, float persistance, float lacunarity, int seed)
    {
        // Validate and set
        this.scale = Mathf.Max(scale, 0.01f);
        this.octaves = Mathf.Max(octaves, 1);
        this.lacunarity = Mathf.Max(lacunarity, 1);
        this.persistance = Mathf.Clamp01(persistance);
        this.seed = seed;
    }
}
