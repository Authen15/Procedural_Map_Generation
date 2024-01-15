using System.Collections.Generic;
using UnityEngine;

public class ChunkDisplay
{
    private Dictionary<Vector2Int, Chunk> lastVisibleChunks = new Dictionary<Vector2Int, Chunk>();
    private List<Vector2Int> visibleChunksPositions;


    private HexGridManager _HexGridManager;
    private ChunkHandler _ChunkHandler;

    public ChunkDisplay(HexGridManager hexGridManager)
    {
        _HexGridManager = hexGridManager;
        _ChunkHandler = hexGridManager.chunkHandler;
        visibleChunksPositions = new List<Vector2Int>();
    }

    public void UpdateVisibleChunks(Transform viewer, int rangeChunksToDisplay) {
        Vector2Int? currentChunkPosition = _ChunkHandler.GetChunkPositionFromWorldPosition(viewer.position);
        if(currentChunkPosition == null){
            Debug.Log("Current chunk position is null");
            return;
        }

        visibleChunksPositions = _ChunkHandler.GetNeighboursChunksPositionFromChunkPosition((Vector2Int)currentChunkPosition, rangeChunksToDisplay);

        GenerateNeededChunks(visibleChunksPositions);
        ClearNotVisibleChunks();
        visibleChunksPositions.Clear();
    }

    public void GenerateNeededChunks(List<Vector2Int> visibleChunksPositions){
        foreach(Vector2Int chunkPosition in visibleChunksPositions){
            if(!lastVisibleChunks.ContainsKey(chunkPosition)){
                Chunk chunk = _ChunkHandler.GenerateChunkFromChunkPosition(chunkPosition);
                // GenerateChunk(chunk);
                // OptimiseChunkMeshes(chunk);
                lastVisibleChunks.Add(chunkPosition, chunk);
            }
        }
    }
        

    public void ClearNotVisibleChunks(){
        foreach(Vector2Int chunkPosition in lastVisibleChunks.Keys){
            if(!visibleChunksPositions.Contains(chunkPosition)){
                GameObject chunkObject = lastVisibleChunks[chunkPosition].gameObject;
                lastVisibleChunks.Remove(chunkPosition);
                Object.Destroy(chunkObject);
            }
        }
    }





    // private void OptimiseChunkMeshes(Chunk chunk)
    // {
    //     // Combine all the cells' meshes into one mesh
    //     MeshFilter[] meshFilters = GetChunkMeshFilters(chunk);
    //     Debug.Log(meshFilters.Length);
    //     CombineInstance[] combine = new CombineInstance[meshFilters.Length];

    //     int validMeshFiltersCount = 0; // Count of valid mesh filters

    //     for (int i = 0; i < meshFilters.Length; i++)
    //     {
    //         MeshFilter meshFilter = meshFilters[i];
            
    //         if (meshFilter != null && meshFilter.sharedMesh != null)
    //         {
    //             combine[validMeshFiltersCount].mesh = meshFilter.sharedMesh;
    //             combine[validMeshFiltersCount].transform = meshFilter.transform.localToWorldMatrix;
    //             meshFilter.gameObject.SetActive(false);
    //             validMeshFiltersCount++;
    //         }
    //     }

    //     if (validMeshFiltersCount > 0)
    //     {
    //         Mesh mesh = new Mesh();
    //         mesh.CombineMeshes(combine); // Pass true for merging sub-meshes and creating a new set of sub-meshes
    //         chunk.AddComponent<MeshFilter>();
    //         chunk.GetComponent<MeshFilter>().sharedMesh = mesh;
    //         chunk.gameObject.SetActive(true);
    //     }
    //     else
    //     {
    //         Debug.LogWarning("No valid MeshFilters found to combine.");
    //     }
    // }

    // private MeshFilter[] GetChunkMeshFilters(Chunk chunk)
    // {
    //     MeshFilter[] meshFilters = new MeshFilter[chunk.cells.Length];
    //     for (int i = 0, j = 0; i < chunk.cells.GetLength(0) && j < chunk.cells.GetLength(1) ; i++, j++)
    //     {
    //         GameObject cellObj = chunk.cells[i,j].CellObject;
    //         MeshFilter meshFilter = cellObj.GetComponent<MeshFilter>();
    //         meshFilters[i] = meshFilter;
    //     }
    //     return meshFilters;
    // }


}
