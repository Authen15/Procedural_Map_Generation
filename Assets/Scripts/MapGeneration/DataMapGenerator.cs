using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DataMapGenerator {

	public static DataMap GenerateHeightDataMap(int size, HeightMapSettings settings, Vector2 sampleCentre) 
    {
		float[,] values = Noise.GenerateNoiseMap(size, settings.noiseSettings, sampleCentre);
		AnimationCurve heightCurve_threadsafe = new AnimationCurve (settings.heightCurve.keys);



		for (int i = 0; i < size; i++) {
			for (int j = 0; j < size; j++) {
				//normalize values
				// values [i, j] = (values [i, j] - settings.noiseSettings.minValue) / (settings.noiseSettings.maxValue - settings.noiseSettings.minValue);
				values [i, j] += settings.baseHeight;
				values [i, j] = Mathf.InverseLerp (settings.noiseSettings.minValue, settings.noiseSettings.maxValue, values [i, j]);
				//apply height curve
				values [i, j] = heightCurve_threadsafe.Evaluate (values [i, j]);
			}
		}

		return new DataMap(values);
	}

	public static DataMap GenerateDataMap(int size, DataMapSettings settings, Vector2 sampleCentre) 
    {
		float[,] values = Noise.GenerateNoiseMap(size, settings.noiseSettings, sampleCentre);
		return new DataMap(values);
    }


	public static DataMap GenerateTemperatureDataMap(int size, DataMap heightMap, TemperatureMapSettings settings, Vector2 sampleCentre)
	{
		float[,] values = new float[size, size];
		float minValue = float.MaxValue;
		float maxValue = float.MinValue;

		// Compute Raw Values
		for (int y = 0; y < size; y++)
		{
			float latitudeEffect = Mathf.Abs((float)y / size - 0.5f) * settings.latitudeSensitivity;  // Affected by latitudeSensitivity
			for (int x = 0; x < size; x++)
			{
				// Base Latitude Adjustment
				values[x, y] = (1 - latitudeEffect) + settings.equatorBias; // Adjust for equatorBias

				// Altitude Adjustment
				float elevationEffect = heightMap.values[x, y];
				values[x, y] -= elevationEffect * settings.elevationSensitivity;  // Affected by elevationSensitivity

				// Tracking min and max values for normalization later
				minValue = Mathf.Min(minValue, values[x, y]);
				maxValue = Mathf.Max(maxValue, values[x, y]);
			}
		}

		// Normalize Values
		for (int y = 0; y < size; y++)
		{
			for (int x = 0; x < size; x++)
			{
				values[x, y] = (values[x, y] - minValue) / (maxValue - minValue);
				values[x, y] += settings.baseTemperature;
				values[x, y] = Mathf.Clamp(values[x, y], 0, 1);  // Ensure values stay within [0,1]
			}
		}

		return new DataMap(values);
	}
}

