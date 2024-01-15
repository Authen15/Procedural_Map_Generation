using UnityEngine;
using System.Collections;

public class DataMap {

	public float[,] values;
    public int size;

    public float minValue = -1.0f;
    public float maxValue = 1.0f;

    public DataMap(float[,] values) {
        this.values = values;
        this.size = values.GetLength(0);
    }
}
