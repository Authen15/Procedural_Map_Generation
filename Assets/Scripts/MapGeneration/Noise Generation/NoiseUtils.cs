using UnityEngine;

public static class NoiseUtils {
    

    // https://en.wikipedia.org/wiki/Cauchy_distribution
    // cumulative distibution function to Uniformize perlin noise values
    public static float CDF(float x, float x0, float gamma){
        return 1.32f/Mathf.PI * Mathf.Atan((x - x0) / gamma) + 0.5f;
    }
    

    public static float RoundToNearestHeightStep(float value, int steps) 
    {
        float stepValue = 1f / steps;
        return Mathf.Round(value * steps) * stepValue;
    }
}