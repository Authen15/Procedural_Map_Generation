using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Viewer : MonoBehaviour
{
    public int rangeChunksToDisplay = 2;
    public ChunkDisplay chunkDisplay;
    public HexGridManager hexGridManager;
    public ChunkHandler chunkHandler;

    public float updateThreholdDistance = 2;
    private Vector3 lastPosition;



    //TODO genrate viewer from hexgridmanager to pass in hexgridmanager
    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        hexGridManager = GameObject.Find("Grid").GetComponent<HexGridManager>();
        chunkDisplay = new ChunkDisplay(hexGridManager);

        lastPosition = transform.position;

        chunkHandler = hexGridManager.chunkHandler;

        chunkDisplay.UpdateVisibleChunks(transform, rangeChunksToDisplay); // generate map on first frame
        
    }

    void Update()
    {

        // Debug.Log(HexMetrics.WorldPositionToCellPosition(transform.position));

        if(Vector3.Distance(transform.position, lastPosition) >= updateThreholdDistance){
            chunkDisplay.UpdateVisibleChunks(transform, rangeChunksToDisplay);
            lastPosition = transform.position;
        }

    }

}
