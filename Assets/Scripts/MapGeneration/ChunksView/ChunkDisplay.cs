// using System.Collections.Generic;
// using UnityEngine;

// public class ChunkDisplay
// {
//     private Dictionary<Vector2Int, Chunk> _lastVisibleChunks = new Dictionary<Vector2Int, Chunk>();
//     private List<Vector2Int> _visibleChunksPositions;


//     private HexMeshGrid _hexMeshGrid;
//     private ChunkHandler _ChunkHandler;

//     public ChunkDisplay(HexMeshGrid hexMeshGrid)
//     {
//         _hexMeshGrid = hexMeshGrid;
//         _ChunkHandler = hexMeshGrid.chunkHandler;
//         _visibleChunksPositions = new List<Vector2Int>();
//     }

//     public void UpdateVisibleChunks(Transform viewer, int rangeChunksToDisplay) {
//         Vector2Int? currentChunkPosition = _ChunkHandler.GetChunkPositionFromWorldPosition(viewer.position);
//         if(currentChunkPosition == null){
//             Debug.Log("Current chunk position is null");
//             return;
//         }

//         _visibleChunksPositions = _ChunkHandler.GetNeighboursChunksPositionFromChunkPosition((Vector2Int)currentChunkPosition, rangeChunksToDisplay);

//         GenerateNeededChunks(_visibleChunksPositions);
//         ClearNotVisibleChunks();
//         _visibleChunksPositions.Clear();
//     }

//     public void GenerateNeededChunks(List<Vector2Int> visibleChunksPositions){
//         foreach(Vector2Int chunkPosition in visibleChunksPositions){
//             if(!_lastVisibleChunks.ContainsKey(chunkPosition)){
//                 Chunk chunk = _ChunkHandler.GenerateChunkFromChunkPosition(chunkPosition);
//                 // GenerateChunk(chunk);
//                 _lastVisibleChunks.Add(chunkPosition, chunk);
//             }
//         }
//     }
        

//     public void ClearNotVisibleChunks(){
//         foreach(Vector2Int chunkPosition in _lastVisibleChunks.Keys){
//             if(!_visibleChunksPositions.Contains(chunkPosition)){
//                 GameObject chunkObject = _lastVisibleChunks[chunkPosition].gameObject;
//                 _lastVisibleChunks.Remove(chunkPosition);
//                 Object.Destroy(chunkObject);
//             }
//         }
//     }
// }
