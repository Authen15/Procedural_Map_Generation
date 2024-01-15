using System.Collections.Generic;
using UnityEngine;

public class ChunkDisplay
{
    private HexMapData hexMapData;
    Chunk[,] visibleChunks;

    public ChunkDisplay(HexMapData hexMapData, int rangeChunksToDisplay)
    {
        this.hexMapData = hexMapData;
        visibleChunks = new Chunk[rangeChunksToDisplay * 2 + 1, rangeChunksToDisplay * 2 + 1];
        SetAllChunksInactive(hexMapData.chunks);
    }

    public void UpdateVisibleChunks(Transform viewer, Chunk currentChunk, int rangeChunksToDisplay) {
        ClearVisibleChunks();
        AddVisibleChunk(viewer, currentChunk, rangeChunksToDisplay);
    }

    public void AddVisibleChunk(Transform viewer, Chunk currentChunk, int rangeChunksToDisplay){
        Vector2 viewerPosition = new Vector2(viewer.position.x, viewer.position.z);

        for(int i = -rangeChunksToDisplay; i <= rangeChunksToDisplay; i++){
            for(int j = -rangeChunksToDisplay; j <= rangeChunksToDisplay; j++){
                int ChunkToCheckCoord_X = currentChunk.x + i;
                int ChunkToCheckCoord_Z = currentChunk.z + j;
                if(ChunkToCheckCoord_X >= 0 && ChunkToCheckCoord_X < hexMapData.chunks.GetLength(0) && ChunkToCheckCoord_Z >= 0 && ChunkToCheckCoord_Z < hexMapData.chunks.GetLength(1)){
                    Chunk chunk = hexMapData.chunks[ChunkToCheckCoord_X, ChunkToCheckCoord_Z];
                    chunk.gameObject.SetActive(true);

                    visibleChunks[i + rangeChunksToDisplay,j + rangeChunksToDisplay] = chunk;
                }
            }
        }
    }

    public void ClearVisibleChunks(){
        foreach(Chunk chunk in visibleChunks){
            if(chunk != null){
                chunk.gameObject.SetActive(false);
            }
        }
    }


    private void SetAllChunksInactive(Chunk[,] chunks){
        for(int i = 0; i < chunks.GetLength(0); i++){
            for(int j = 0; j < chunks.GetLength(1); j++){
                chunks[i,j].gameObject.SetActive(false);
            }
        }
    }


    
    
    
    

}
