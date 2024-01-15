using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public int x;
    public int z;
    public int size;
    public HexCell[,] cells;

    public void Initialize(int x, int z, int size)
    {
        this.x = x;
        this.z = z;
        this.size = size;
        cells = new HexCell[size, size];
    }

    public void SetCell(int x, int z, HexCell cell)
    {
        cells[x, z] = cell;
    }
}
