using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

[BurstCompile]
struct NoiseGenerationJob : IJobParallelFor
{
    public int size;
    public float scale;
    public int octaves;
    public float persistance;
    public float lacunarity;
    public float2 offset;

    public NativeArray<float> noiseMap;

    public void Execute(int index)
    {
        int x = index % size;
        int y = index / size;

		float halfMapSize = size * 0.5f; // so incresing / decreasing noise size will create a (de)zoom effect starting from the center
        float invScale = 1f / scale;

        float amplitude = 1;
        float frequency = 1;
        float noiseHeight = 0;

        for (int i = 0; i < octaves; i++) {
            float sampleX = (x-halfMapSize) * invScale * frequency;
            float sampleY = (y-halfMapSize) * invScale * frequency;

            float n = noise.cnoise(new float2(sampleX + offset.x, sampleY + offset.y));

            noiseHeight += n * amplitude;

            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Normalize
        noiseHeight = noiseHeight * 0.5f + 0.5f; // remap to [0, 1]

        noiseMap[index] = math.saturate(noiseHeight); // could be above 1 since we use multiple octaves
    }
}