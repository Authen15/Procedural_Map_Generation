using UnityEngine;
using System.Collections;

public class DataMap {

	public float[,] values;
    public int size;

    public float minValue = 0;
    public float maxValue = 0;

    public DataMap(float[,] values) {
        this.values = values;
        this.size = values.GetLength(0);
    }
}
