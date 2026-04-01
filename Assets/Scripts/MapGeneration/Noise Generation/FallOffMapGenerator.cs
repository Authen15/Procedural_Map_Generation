using UnityEngine;
using static IslandData;

public static class FallOffMapGenerator{
    public static float[] GenerateIslandFalloffMap(HeightMapSettings heightMapSettings)
    {
        int mapRadius = HexMetrics.IslandRadius;
        float invMapRadius = 1f / mapRadius; // faster than division
        int size = mapRadius * 2 + 1;
        float[] falloffMap = new float[size * size];
        
        // Iterate through all hexes in the map
        foreach(AxialCoordinates coord in CellCoords)
        {
            int q = coord.Q;
            int r = coord.R;
            
            float distance = coord.Norm();

            // Normalize distance (0 at center, 1 at edge)
            float normalizedDistance = distance * invMapRadius;
            
            // Apply falloff curve
            float falloffValue = Evaluate(heightMapSettings, normalizedDistance);
            
            int arrayX = q + mapRadius;
            int arrayY = r + mapRadius;
            int index = arrayY * size + arrayX;
            
            falloffMap[index] = falloffValue;
        }
        
        return falloffMap;
    }

    private static float Evaluate(HeightMapSettings heightMapSettings, float value)
    {
        float a = heightMapSettings.FallOffMapSteepness;
        float b = heightMapSettings.FallOffMapOffset;
        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}

