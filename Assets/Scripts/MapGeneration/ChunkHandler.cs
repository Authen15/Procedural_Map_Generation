using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler
{
    public int chunkSize;
    public HexMapData hexMapData;
    private Transform parent;


    public ChunkHandler(HexMapData hexMapData, int chunkSize, Transform parent)
    {
        this.hexMapData = hexMapData;
        this.chunkSize = chunkSize;
        this.parent = parent;
    }
    
    public void GenerateAllChunks()
    {
        for (int x = 0; x < hexMapData.size; x += chunkSize)
        {
            for (int z = 0; z < hexMapData.size; z += chunkSize)
            {
                GenerateChunk(x, z);
            }
        }
    }

    private void GenerateChunk(int x, int z){
        GameObject chunkObject = new GameObject("Chunk " + x/chunkSize + ", " + z/chunkSize);
        chunkObject.transform.SetParent(parent);
        Vector3 position = new Vector3((2*x + chunkSize - 1) * HexMetrics.innerRadius, 0, (z * 1.5f + 1) * HexMetrics.outerRadius + (chunkSize - 3) * HexMetrics.innerRadius);
        chunkObject.transform.localPosition = position;
        
        chunkObject.tag = "Chunk";


        for (int i = 0; i < chunkSize; i++)
        {
            for (int j = 0; j < chunkSize; j++)
            {
                HexCell cellToAdd = hexMapData.cells[x + i, z + j];
                cellToAdd.CellObject.transform.SetParent(chunkObject.transform);
            }
        }
        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(x / chunkSize, z / chunkSize, chunkSize);
        hexMapData.chunks[x / chunkSize, z / chunkSize] = chunk;
    }

    public Chunk GetChunkFromPosition(Vector3 worldPosition)
    {
        // Transform to local position
        Vector3 localPosition = parent.InverseTransformPoint(worldPosition);

        // Find the column and row of the cell
        int col = Mathf.FloorToInt(localPosition.x / (HexMetrics.innerRadius * 2));
        int row = Mathf.FloorToInt(localPosition.z / (HexMetrics.outerRadius * 1.5f));

        // Find which chunk the cell belongs to
        int chunkCol = col / chunkSize;
        int chunkRow = row / chunkSize;

        // Return the chunk. 
        if (chunkCol >= 0 && chunkCol < hexMapData.size / chunkSize && chunkRow >= 0 && chunkRow < hexMapData.size / chunkSize)
        {
            return hexMapData.chunks[chunkCol, chunkRow];
        }
        return null; // Return null if the position is out of bounds
    }



}
