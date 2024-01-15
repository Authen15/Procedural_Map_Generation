using UnityEngine;

public static class NoiseUtils {
    
    public static float RoundToNearestHeightStep(float value, int steps) 
    {
        float stepValue = 1f / steps;
        return Mathf.Round(value * steps) * stepValue;
    }
}