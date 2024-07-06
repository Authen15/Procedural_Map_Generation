using UnityEngine;

public class NoiseGenerator
{
    protected NoiseCache _cache;

    public DataMap DataMap;

    public NoiseGenerator(DataMapSettings dataMapSettings)
    {
        _cache = new NoiseCache(dataMapSettings);
        DataMap = new DataMap();
    }

    // public virtual float[,] GetNoiseMap(int size, int xOffset, int yOffset){
    //     return CreateNoiseMap(size, xOffset, yOffset, false, true);
    // }

    public virtual float[,] GenerateNoiseMap(int size){
        return GenerateNoiseMap(size, 0, 0, false, true);
    }

    public virtual float GetNoiseValue(int x, int y, int size)
    {
        float amplitude = 1;
        float frequency = 1;
        float noiseValue = 0;
        float halfWidth = size / 2f;
        float halfHeight = size / 2f;

        for (int i = 0; i < _cache.Settings.octaves; i++)
        {
            float sampleX = (x - halfWidth + _cache.OctaveOffsets[i].x) / _cache.Settings.scale * frequency;
            float sampleY = (y - halfHeight + _cache.OctaveOffsets[i].y) / _cache.Settings.scale * frequency;
            float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
            noiseValue += perlinValue * amplitude;
            amplitude *= _cache.Settings.persistance;
            frequency *= _cache.Settings.lacunarity;
        }

        return noiseValue;
    }

    public void UpdateCacheValues(float value)
    {
        if (value < _cache.minValue) _cache.minValue = value;
        if (value > _cache.maxValue) _cache.maxValue = value;
    }

    public virtual void UpdateNoiseGlobalMinMax(int size){
        GenerateNoiseMap(size, 0, 0, true, false);
    }

    private float[,] GenerateNoiseMap(int size, int xOffset = 0, int yOffset = 0, bool doUpdateMinMax = false, bool doNormalize = false)
    {
        float[,] noiseMap = new float[size, size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float noiseValue = GetNoiseValue(x + xOffset, y + yOffset, size);
                if(doUpdateMinMax)UpdateCacheValues(noiseValue);
                if(doNormalize)noiseValue = Mathf.InverseLerp(_cache.minValue, _cache.maxValue, noiseValue);
                noiseMap[x, y] = noiseValue;
            }
        }
        return noiseMap;
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
