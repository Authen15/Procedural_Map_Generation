using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public int rangeChunksToDisplay = 2;
    public ChunkDisplay chunkDisplay;
    public HexGridManager hexGridManager;
    public ChunkHandler chunkHandler;

    public float updateThreholdDistance = 5;
    private Vector3 lastPosition;



    //TODO genrate viewer from hexgridmanager to pass in hexgridmanager
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        hexGridManager = GameObject.Find("Grid").GetComponent<HexGridManager>();
        chunkDisplay = new ChunkDisplay(hexGridManager.hexMapData, rangeChunksToDisplay);

        lastPosition = transform.position;

        chunkHandler = hexGridManager.chunkHandler;
    }

    void Update()
    {
        
        if(Vector3.Distance(transform.position, lastPosition) >= updateThreholdDistance){
            Chunk chunk = chunkHandler.GetChunkFromPosition(transform.position);
            if(chunk != null){
                chunkDisplay.UpdateVisibleChunks(transform, chunk, rangeChunksToDisplay);
                lastPosition = transform.position;
            }
        }
    }

}
