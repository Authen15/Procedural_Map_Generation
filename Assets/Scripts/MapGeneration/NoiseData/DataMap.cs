using UnityEngine;
using System.Collections;

public class DataMap {

    public bool IsGenerated;
	public float[,] Values = new float[HexMetrics.MapSize,HexMetrics.MapSize];
}
