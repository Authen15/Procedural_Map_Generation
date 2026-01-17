using UnityEngine;

public static class NoiseGenerator
{
    public static void PrintNoiseDistribution(float[] noiseMap, int size, int nbStep)
    {
        float stepSize = 1f / nbStep;
        int[] stepsValue = new int[nbStep+1];

        int length = size * size;

        for (int x = 0; x < length; x++)
        {
            int index = (int)(noiseMap[x] / stepSize);
            if (index >= nbStep+1) Debug.Log($"index = {index}");
            stepsValue[index]++;
        }

        for (int i = 0; i < nbStep+1; i++)
        {
            Debug.Log($"step : {stepSize*i} to {(i+1)*stepSize} = {stepsValue[i]}");
        }
    }

    public static float[] GenerateHeightMap(HeightMapSettings heightMapSettings, int size, int customSeed = 0)
    {

        System.Random prng = new System.Random(customSeed + MapManager.Instance.MapSeed);
        float offsetX = prng.Next(-100000, 100000);
        float offsetY = prng.Next(-100000, 100000);

        float[] noiseMap = GenerateNoiseMap(size, heightMapSettings.NoiseSettings, new Vector2(offsetX, offsetY));
        AnimationCurve heightCurve = heightMapSettings.HeightCurve;

        float maxHeight = heightMapSettings.GetMaximumHeight();
        float multiplier = heightMapSettings.Multiplier;
        float invMaxHeight = 1f / maxHeight;
        // float[,] fallOffMap = null; //TODO handle falloffmap in a better way
        // if (heightMapSettings.UseFallOff)
        //     fallOffMap = FallOffMapGenerator.GenerateIslandFalloffMap(heightMapSettings);

        int length = size * size;
        for (int i = 0; i < length; i++)
        {
            float rawNoise = noiseMap[i];
            rawNoise = NoiseUtils.CDF(rawNoise, 0.5f, 0.2f); // to uniformize the noise

            rawNoise *= multiplier;

            rawNoise = heightCurve.Evaluate(rawNoise);

            // remap between 0 and 1
            rawNoise *= invMaxHeight;

            // if (heightMapSettings.UseFallOff)
            // {
            //     finalValue -= fallOffMap[x, y];
            // }
            rawNoise = NoiseUtils.RoundToNearestHeightStep(rawNoise);

            noiseMap[i] = rawNoise;
        }
        return noiseMap;
    }

    private static float[] GenerateNoiseMap(int size, NoiseSettings settings, Vector2 sampleCentre) {
		int length = size*size;
        float[] noiseMap = new float[length];

		System.Random prng = new System.Random (settings.seed);
		Vector2[] octaveOffsets = new Vector2[settings.octaves];

		float amplitude = 1;

		for (int i = 0; i < settings.octaves; i++) {
			float offsetX = prng.Next (-100000, 100000) + settings.offset.x + sampleCentre.x;
			float offsetY = prng.Next (-100000, 100000) - settings.offset.y - sampleCentre.y;
			octaveOffsets [i] = new Vector2 (offsetX, offsetY);

			amplitude *= settings.persistance;
		}

		float maxLocalNoiseHeight = float.MinValue;
		float minLocalNoiseHeight = float.MaxValue;

		float halfMapSize = size / 2f;


		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {

				amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < settings.octaves; i++) {
					float sampleX = (x-halfMapSize + octaveOffsets[i].x) / settings.scale * frequency;
					float sampleY = (y-halfMapSize + octaveOffsets[i].y) / settings.scale * frequency;

					float perlinValue = Mathf.PerlinNoise (sampleX, sampleY);
					noiseHeight += perlinValue * amplitude;

					amplitude *= settings.persistance;
					frequency *= settings.lacunarity;
				}

				if (noiseHeight > maxLocalNoiseHeight) {
					maxLocalNoiseHeight = noiseHeight;
				} 
				if (noiseHeight < minLocalNoiseHeight) {
					minLocalNoiseHeight = noiseHeight;
				}
				noiseMap [y * size + x] = noiseHeight;
			}
		}

        for (int i = 0; i < length; i++) {
                noiseMap [i] = Mathf.InverseLerp(minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [i]);
	    }

		return noiseMap;
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
