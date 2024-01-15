using System.Collections.Generic;
using UnityEngine;

public class HexMapData 
{
    public int size;
    public int nbHeightSteps;
    public Chunk[,] chunks;

    public HexCell[,] cells;

    public HexMapData(int size, int nbHeightSteps, int chunkSize)
    {
        this.size = size;
        this.nbHeightSteps = nbHeightSteps;

        cells = new HexCell[size, size];
        chunks = new Chunk[size/chunkSize, size/chunkSize];

    }

    public void SetCell(int x, int z, HexCell cell)
    {
        cells[x, z] = cell;
    }

}
