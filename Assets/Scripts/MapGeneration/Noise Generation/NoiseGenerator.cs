using Biome;
using UnityEngine;

public static class NoiseGenerator
{
    // protected NoiseCache _cache;

    // public DataMap DataMap;

    // private static float GetNoiseValue(NoiseSettings settings, int x, int y, int size)
    // {
    //     float amplitude = 1;
    //     float frequency = 1;
    //     float noiseValue = 0;
    //     float halfWidth = size / 2f;
    //     float halfHeight = size / 2f;

    //     for (int i = 0; i < settings.octaves; i++)
    //     {
    //         float sampleX = (x - halfWidth + settings.OffsetX) / settings.scale * frequency;
    //         float sampleY = (y - halfHeight + settings.OffsetY) / settings.scale * frequency;
    //         // float sampleX = (x - halfWidth) / settings.scale * frequency;
    //         // float sampleY = (y - halfHeight) / settings.scale * frequency;
    //         float perlinValue = Mathf.PerlinNoise(sampleX, sampleY);
    //         noiseValue += perlinValue * amplitude;
    //         amplitude *= settings.persistance;
    //         frequency *= settings.lacunarity;
    //     }

    //     return noiseValue;
    // }

    // private static float[,] GenerateNoiseMap(NoiseSettings settings, int size)
    // {
    //     float[,] noiseMap = new float[size, size];
    //     for (int y = 0; y < size; y++)
    //     {
    //         for (int x = 0; x < size; x++)
    //         {
    //             float noiseValue = GetNoiseValue(settings, x, y, size);
    //             noiseMap[x, y] = noiseValue;
    //         }
    //     }
    //     return noiseMap;
    // }

    public static float[,] GenerateHeightMap(HeightMapSettings heightMapSettings, int size, int customSeed = 0){
        
        System.Random prng = new System.Random (customSeed + MapManager.Instance.MapSeed);
		float offsetX = prng.Next (-100000, 100000);
		float offsetY = prng.Next (-100000, 100000);

        float[,] noiseMap = GenerateNoiseMap(size, heightMapSettings.NoiseSettings, new Vector2(offsetX, offsetY));
        AnimationCurve heightCurve_threadsafe = new AnimationCurve (heightMapSettings.HeightCurve.keys);
        
        float[,] fallOffMap = null; //TODO handle falloffmap in a better way
        if (heightMapSettings.UseFallOff)
            fallOffMap = FallOffMapGenerator.GenerateIslandFalloffMap(heightMapSettings);

        for(int y = 0; y < size; y++){
            for(int x = 0; x < size; x++){
                float rawNoise = noiseMap[x, y];
                rawNoise = NoiseUtils.CDF(rawNoise, 0.5f, 0.2f);

                rawNoise = Mathf.Clamp(rawNoise, 0, 1);


                float finalValue = heightCurve_threadsafe.Evaluate(rawNoise); // height curve bound must be < 0 and > 1

                if (heightMapSettings.UseFallOff){
                    finalValue -= fallOffMap[x,y];
                }

                finalValue = Mathf.Clamp(finalValue, 0, 1);

                // UpdateCacheValues(finalValue);
                NoiseUtils.RoundToNearestHeightStep(finalValue, HexMetrics.NbHeightSteps);

                noiseMap[x, y] = finalValue;
            }
        }
        return noiseMap;
    }

    private static float[,] GenerateNoiseMap(int size, NoiseSettings settings, Vector2 sampleCentre) {
		float[,] noiseMap = new float[size,size];

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
				noiseMap [x, y] = noiseHeight;
			}
		}

        for (int y = 0; y < size; y++) {
            for (int x = 0; x < size; x++) {
                noiseMap [x, y] = Mathf.InverseLerp (minLocalNoiseHeight, maxLocalNoiseHeight, noiseMap [x, y]);
            }
	}

		return noiseMap;
	}

}





// public class NoiseCache
// {
//     public Vector2[] OctaveOffsets { get; private set; }
//     public NoiseSettings Settings { get; private set; }
//     public float minValue = float.MaxValue;
//     public float maxValue = float.MinValue;

//     // public NoiseCache(DataMapSettings dataMapSettings)
//     // {
//     //     // Debug.Log(dataMapSettings.noiseSettings.scale);
//     //     Settings = dataMapSettings.noiseSettings;
//     //     OctaveOffsets = new Vector2[Settings.octaves];
//     //     System.Random prng = new System.Random(Settings.seed);
//     //     float amplitude = 1;

//     //     for (int i = 0; i < Settings.octaves; i++)
//     //     {
//     //         float offsetX = prng.Next(-100000, 100000);
//     //         float offsetY = prng.Next(-100000, 100000);
//     //         OctaveOffsets[i] = new Vector2(offsetX, offsetY);
//     //         amplitude *= Settings.persistance;  
//     //     }
//     // }

// }

// [System.Serializable]
// public class NoiseSettings
// {
//     public float scale = 50;
//     public int octaves = 6;
//     [Range(0,1)]
//     public float persistance = .6f;
//     [Range(0,1)]
//     public float lacunarity = 2;
//     public int seed;
//     [HideInInspector]
//     public float OffsetX;
//     [HideInInspector]
//     public float OffsetY;

//     public NoiseSettings(float scale, int octaves, float persistance, float lacunarity, int seed)
//     {
//         // Validate and set
//         this.scale = Mathf.Max(scale, 0.01f);
//         this.octaves = Mathf.Max(octaves, 1);
//         this.lacunarity = Mathf.Max(lacunarity, 1);
//         this.persistance = Mathf.Clamp01(persistance);
//         this.seed = seed;

//         System.Random prng = new System.Random(seed);
//         OffsetX = prng.Next(-100000, 100000);
//         OffsetY = prng.Next(-100000, 100000);
//     }
// }

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
