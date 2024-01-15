using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TextureGenerator {


	public static Texture2D TextureFromColourMap(Color[] colourMap, int size) {
		Texture2D texture = new Texture2D (size, size);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.SetPixels (colourMap);
		texture.Apply ();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(DataMap heightMap) {
		int size = heightMap.size;

		Color[] colourMap = new Color[size * size];
		for (int y = 0; y < size; y++) {
			for (int x = 0; x < size; x++) {
				colourMap [y * size + x] = Color.Lerp (Color.black, Color.white, Mathf.InverseLerp(heightMap.minValue,heightMap.maxValue,heightMap.values [x, y]));
			}
		}

		return TextureFromColourMap (colourMap, size);
	}

    public static void WriteTexture(Texture2D texture, string pathToFile){
        // byte[] bytes = texture.EncodeToPNG();
		byte[] bytes = texture.EncodeToJPG();
        System.IO.File.WriteAllBytes(pathToFile, bytes);


    }


	public static Texture2D GenerateAtlasTexture(Biome[] biomes, int atlasWidth, int atlasHeight)
	{
		Texture2D atlas = new Texture2D(atlasWidth, atlasHeight);

		for (int y = 0; y < atlasHeight; y++)
		{
			for (int x = 0; x < atlasWidth; x++)
			{
				float temperature = (float)x / (atlasWidth - 1);
				float humidity = (float)y / (atlasHeight - 1);

				Color pixelColor = Color.clear; // Set a default transparent color
				foreach (Biome biome in biomes)
				{
					if(temperature > 1 || temperature < 0)
								Debug.Log("heightValue: " + temperature);
					if(humidity > 1 || humidity < 0)
								Debug.Log("heightValue: " + humidity);
					if (biome.minTemperature <= temperature && biome.maxTemperature >= temperature &&
						biome.minMoisture <= humidity && biome.maxMoisture >= humidity)
					{
						// Determine height value based on position in the texture
						float heightValue = Mathf.Lerp(0, 1, (float)y / (atlasHeight - 1));

						// Determine the level based on heightValue
						foreach (BiomeLevel level in biome.levels)
						{
							if(heightValue > 1 || heightValue < 0)
								Debug.Log("heightValue: " + heightValue);
							if (heightValue >= level.minHeight && heightValue <= level.maxHeight)
							{
								pixelColor = level.color;
								break;
							}
						}
						break; // Once we find a matching biome, we break out of the loop
					}
				}
				atlas.SetPixel(x, y, pixelColor);
			}
		}

		atlas.Apply();
		return atlas;
	}


	public static Texture2D ApplyGaussianBlur(Texture2D source, int radius)
	{
		Texture2D blurred = new Texture2D(source.width, source.height);
		int ks = radius * 2 + 1;
		float[] kernel = GaussianKernel(ks);

		for (int y = 0; y < source.height; y++)
		{
			for (int x = 0; x < source.width; x++)
			{
				Color c = ApplyBlurForPixel(source, x, y, kernel, radius);
				blurred.SetPixel(x, y, c);
			}
		}

		blurred.Apply();
		return blurred;
	}

	private static Color ApplyBlurForPixel(Texture2D source, int x, int y, float[] kernel, int radius)
	{
		Color c = Color.clear;
		float totalWeight = 0;

		for (int ky = -radius; ky <= radius; ky++)
		{
			for (int kx = -radius; kx <= radius; kx++)
			{
				int px = Mathf.Clamp(x + kx, 0, source.width - 1);
				int py = Mathf.Clamp(y + ky, 0, source.height - 1);
				c += source.GetPixel(px, py) * kernel[ky + radius] * kernel[kx + radius];
				totalWeight += kernel[ky + radius] * kernel[kx + radius];
			}
		}

		return c / totalWeight;
	}

	private static float[] GaussianKernel(int length)
	{
		float[] kernel = new float[length];
		float sigma = length / 6.0f;
		float mean = length / 2.0f;
		float sum = 0.0f;
		for (int x = 0; x < length; x++)
		{
			kernel[x] = Mathf.Exp(-0.5f * (x - mean) * (x - mean) / (sigma * sigma)) / (sigma * Mathf.Sqrt(2 * Mathf.PI));
			sum += kernel[x];
		}
		for (int x = 0; x < length; x++)
		{
			kernel[x] /= sum;
		}
		return kernel;
	}


}
