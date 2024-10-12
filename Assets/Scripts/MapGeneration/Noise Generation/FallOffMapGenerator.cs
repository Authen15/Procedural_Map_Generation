using UnityEngine;

public static class FallOffMapGenerator{
    public static float[,] GenerateIslandFalloffMap(HeightMapSettings heightMapSettings)
    {
        float[,] map = new float[HexMetrics.IslandSize, HexMetrics.IslandSize];

        for (int y = 0; y < HexMetrics.IslandSize; y++)
        {
            for (int x = 0; x < HexMetrics.IslandSize; x++)
            {
                float nx = x / (float)HexMetrics.IslandSize * 2 - 1;
                float ny = y / (float)HexMetrics.IslandSize * 2 - 1;

                float value = Mathf.Max(Mathf.Abs(nx), Mathf.Abs(ny));
                map[x, y] = Evaluate(heightMapSettings, value);
            }
        }

        return map;
    }

    private static float Evaluate(HeightMapSettings heightMapSettings, float value)
    {
        float a = heightMapSettings.FallOffMapSteepness;
        float b = heightMapSettings.FallOffMapOffset;

        return Mathf.Pow(value, a) / (Mathf.Pow(value, a) + Mathf.Pow(b - b * value, a));
    }
}

