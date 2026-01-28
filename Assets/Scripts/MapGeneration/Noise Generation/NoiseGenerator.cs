using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public static class NoiseGenerator
{
    public static void PrintNoiseDistribution(float[] noiseMap, int nbStep)
    {
        float stepSize = 1f / nbStep;
        int[] stepsValue = new int[nbStep+1];

        int length = noiseMap.Length;
        for (int i = 0; i < length; i++)
        {
            int index = (int)(noiseMap[i] / stepSize);
            if (index >= nbStep + 1 || index < 0)
            {
                Debug.Log($"PrintNoiseDistribution error at index = {index}");
                continue;
            } 
            stepsValue[index]++;
        }

        for (int i = 0; i < nbStep+1; i++)
        {
            Debug.Log($"step : {stepSize*i} to {(i+1)*stepSize} = {stepsValue[i]}");
        }
    }

    public static float[] GenerateHeightMap(HeightMapSettings heightMapSettings, int size, uint customSeed = 0)
    {
        int length = size * size;

        NativeArray<float> nativeNoiseMap = new NativeArray<float>(length, Allocator.Persistent);

        GenerateNoiseMap(heightMapSettings, nativeNoiseMap, size, customSeed);
        ProcessNoiseMap(heightMapSettings, nativeNoiseMap, length);

        float[] noiseMap = nativeNoiseMap.ToArray();
        nativeNoiseMap.Dispose();
        return noiseMap;
    }

    private static void GenerateNoiseMap(HeightMapSettings heightMapSettings, NativeArray<float> nativeNoiseMap, int size, uint customSeed = 0)
    {
        var prng = new Unity.Mathematics.Random(customSeed);

        float2 offset = new float2(
            prng.NextFloat(-100000f, 100000f),
            prng.NextFloat(-100000f, 100000f)
        );

        var job = new NoiseGenerationJob{
            size = size,
            scale = heightMapSettings.NoiseSettings.scale,
            octaves = heightMapSettings.NoiseSettings.octaves,
            persistance = heightMapSettings.NoiseSettings.persistance,
            lacunarity = heightMapSettings.NoiseSettings.lacunarity,
            offset = offset,
            noiseMap = nativeNoiseMap
        };

        JobHandle handle = job.Schedule(size*size, 64);
        handle.Complete();
    }

    //TODO create a job friendly heightcurve and jobify this processing
    private static NativeArray<float> ProcessNoiseMap(HeightMapSettings heightMapSettings, NativeArray<float> nativeNoiseMap, int length)
    {
        AnimationCurve heightCurve = heightMapSettings.HeightCurve;

        float maxHeight = heightMapSettings.GetMaximumHeight();
        float multiplier = heightMapSettings.Multiplier;
        float invMaxHeight = 1f / maxHeight;

        for (int i = 0; i < length; i++)
        {
            float rawNoise = nativeNoiseMap[i];
            rawNoise = NoiseUtils.CDF(rawNoise, 0.5f, 0.2f); // to uniformize the noise

            rawNoise *= multiplier;

            rawNoise = heightCurve.Evaluate(rawNoise);

            // remap between 0 and 1
            rawNoise *= invMaxHeight;

            rawNoise = NoiseUtils.RoundToNearestHeightStep(rawNoise);

            nativeNoiseMap[i] = rawNoise;
        }
        return nativeNoiseMap;
    }
}

[System.Serializable]
public class NoiseSettings {
	public float scale = 50;

	public int octaves = 6;
	[Range(0,1)]
	public float persistance =.6f;
	public float lacunarity = 2;

	public int seed;
	public Vector2 offset;

	public void ValidateValues() {
		scale = Mathf.Max (scale, 0.01f);
		octaves = Mathf.Max (octaves, 1);
		lacunarity = Mathf.Max (lacunarity, 1);
		persistance = Mathf.Clamp01 (persistance);
	}
}
