using System.Runtime.CompilerServices;
using UnityEngine;

public static class NoiseUtils {
    

    // https://en.wikipedia.org/wiki/Cauchy_distribution
    // cumulative distibution function to Uniformize perlin noise values
    static readonly float _firstFactor = 1.32f/Mathf.PI;
    public static float CDF(float x, float x0, float gamma){
        return _firstFactor * Mathf.Atan((x - x0) / gamma) + 0.5f;
    }
    

    static readonly int _NbSteps = HexMetrics.NbHeightSteps;
    static readonly float _heightStepValue = 1f / HexMetrics.NbHeightSteps;
    public static float RoundToNearestHeightStep(float value) 
    {
        return Mathf.Round(value * _NbSteps) * _heightStepValue;
    }
}