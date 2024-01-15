using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler
{
    public int mapSize;
    public int chunkSize;
    private Transform parent;
    public int chunkGridLength;
    private Material terrainMaterial;
    HexGridManager _HexGridManager;


    public ChunkHandler(int mapSize, int chunkSize, Transform parent, Material terrainMaterial, HexGridManager hexGridManager)
    {
        this.mapSize = mapSize;
        this.chunkSize = chunkSize;
        this.parent = parent;
        chunkGridLength = mapSize / chunkSize;
        this.terrainMaterial = terrainMaterial;
        _HexGridManager = hexGridManager;
    }
    
    // public void GenerateAllChunks()
    // {
    //     for (int X = 0; X < mapSize / chunkSize; X++)
    //     {
    //         for (int Z = 0; Z < mapSize / chunkSize; Z++)
    //         {
    //             GenerateChunkFromChunkPosition(X, Z);
    //         }
    //     }
    // }

    public Chunk GenerateChunkFromChunkPosition(Vector2Int chunkPosition){        

        GameObject chunkObject = new GameObject("Chunk " + chunkPosition.x + ", " + chunkPosition.y);
        chunkObject.transform.SetParent(parent);

        Vector2 chunkWorldPos = WorldPosFromChunkGrid(chunkPosition);
        Vector3 position = new Vector3(chunkWorldPos.x, 0, chunkWorldPos.y);
        // Debug.Log("Generating chunk: " + chunkPosition.x + ", " + chunkPosition.y + " at position: " + position);

        chunkObject.transform.localPosition = position;
        
        chunkObject.tag = "Chunk";

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(chunkPosition.x, chunkPosition.y, chunkSize);
        PopulateChunk(chunk);
        CombineMeshes(chunk);

        return chunk;
    }

    private void CombineMeshes(Chunk chunk){
        // Debug.Log("Combining meshes for chunk: " + chunk.x + ", " + chunk.z);
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Debug.Log("meshFilters.Length: " + meshFilters.Length);
        int i = 0;
        while (i < meshFilters.Length)
        {
            if(meshFilters[i].gameObject == chunk.gameObject){
                i++;
                continue;
            }
            combine[i].mesh = meshFilters[i].sharedMesh;
            // combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix * chunk.transform.worldToLocalMatrix;
            meshFilters[i].gameObject.SetActive(false); //start with chunks disabled
            GameObject.Destroy(meshFilters[i].gameObject);
            i++;
        }
        chunk.gameObject.AddComponent<MeshFilter>();
        chunk.gameObject.AddComponent<MeshRenderer>();
        chunk.gameObject.GetComponent<MeshFilter>().mesh = new Mesh();
        chunk.gameObject.GetComponent<MeshFilter>().mesh.CombineMeshes(combine);
        chunk.gameObject.GetComponent<MeshRenderer>().material = terrainMaterial;


        // Add or get the MeshCollider and assign the combined mesh to it
        MeshCollider meshCollider = chunk.gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null) {
            meshCollider = chunk.gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = chunk.gameObject.GetComponent<MeshFilter>().mesh;

        chunk.gameObject.SetActive(true);
    }

    private void PopulateChunk(Chunk chunk){
        _HexGridManager.cellFactory.GenerateChunkCells(chunk);
        _HexGridManager.featureGenerator.GenerateChunkFeatures(chunk);
    }

    public Vector2Int? GetChunkPositionFromWorldPosition(Vector3 worldPosition)
    {
        // Transform to local position
        Vector3 localPosition = parent.InverseTransformPoint(worldPosition);

        // Find the column and row of the cell 
        Vector3Int cellPosition = HexMetrics.WorldPositionToCellPosition(localPosition);

        // Find which chunk the cell belongs to
        int chunkCol = cellPosition.x / chunkSize;
        int chunkRow = cellPosition.z / chunkSize;

        // Return the chunk. 
        if (chunkCol >= 0 && chunkCol < chunkGridLength && chunkRow >= 0 && chunkRow < chunkGridLength)
        {
            return new Vector2Int(chunkCol, chunkRow);
        }
        Debug.Log(chunkCol + ", " + chunkRow + "this chunk is out of bounds of the map, the player should not be here");
        return null;
    }

    public List<Vector2Int> GetNeighboursChunksPositionFromChunkPosition(Vector2Int chunkPosition, int rangeChunksToDisplay){
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for(int i = -rangeChunksToDisplay; i <= rangeChunksToDisplay; i++)
        {
            for(int j = -rangeChunksToDisplay; j <= rangeChunksToDisplay; j++)
            {
                int ChunkToCheckCoord_X = chunkPosition.x + i;
                int ChunkToCheckCoord_Z = chunkPosition.y + j; // in the chunk grid, the z axis is the y axis in the world
                
                if(ChunkToCheckCoord_X >= 0 && ChunkToCheckCoord_X < chunkGridLength &&
                   ChunkToCheckCoord_Z >= 0 && ChunkToCheckCoord_Z < chunkGridLength)
                {
                    neighbours.Add(new Vector2Int(ChunkToCheckCoord_X, ChunkToCheckCoord_Z));
                }
            }
        }
        return neighbours;
    }


    private Vector2Int ChunkGridFromWorldPos(Vector2Int position){
        Vector3Int cellPos = HexMetrics.WorldPositionToCellPosition(new Vector3(position.x, 0, position.y));
        return new Vector2Int(cellPos.x / chunkSize, cellPos.z / chunkSize);
    }

    private Vector2 WorldPosFromChunkGrid(Vector2Int position){
        Vector3 chunkWorldPos = HexMetrics.CellPositionToWorldPosition(new Vector3Int(position.x * chunkSize, 0, position.y * chunkSize));
        return new Vector2(chunkWorldPos.x, chunkWorldPos.z);
    }

}
