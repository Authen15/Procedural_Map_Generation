using System.Collections.Generic;
using UnityEngine;

public static class HexGridUtils {
    public enum Dir { 
        BottomLeft = 0, 
        Left = 1, 
        TopLeft = 2, 
        TopRight = 3, 
        Right = 4, 
        BottomRight = 5
    };

    public static Vector3[] GetDir = {
        new Vector3(0, 0, -1), // bottom left
        new Vector3(-1, 0, 0), 	// left
        new Vector3(-1, 0, 1), 	// top left
        new Vector3(0, 0, 1), 	// top right
        new Vector3(1, 0, 0), 	// right
        new Vector3(1, 0, -1) 	// bottom right
    };
    

    public static Dictionary<Vector3, int> CellIndexDict; // contaiu the index of a cell in an array using it's coordinates in the grid
    
    public static Vector3[] ChunkCellsPositions;

    public static Vector3[] MapChunksPositions;

    static HexGridUtils(){
        CellIndexDict = new Dictionary<Vector3, int>();
        ChunkCellsPositions = GenerateGridPositions(HexMetrics.ChunkSize, true);
        MapChunksPositions = GenerateGridPositions(HexMetrics.MapSize);

    }

	private static Vector3[] GenerateGridPositions(int size, bool populateDict = false){
		int radius = size / 2;

		//Total cell number is can be calcultated using the radius
		//first the center cell + 6 cells around it
		//then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
		// SUM(1->R : R) = R(R+1)/2
		int cellsNumber = 1 + 6 * (radius * (radius + 1) / 2);
		Vector3[] positions = new Vector3[cellsNumber];
		
		// Add center cell
        Vector3 centerCell = new Vector3(radius, 0, radius);
		positions[0] = centerCell;
        if (populateDict) CellIndexDict.Add(centerCell, 0);

		// Iterate through rings
        int index = 1;
		for (int r = 1; r <= radius; r++) {
			// Start at (r, 0) for the current ring
			Vector3 current = new Vector3(radius + r, 0, radius);

			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
					current += GetDir[d];
					positions[index] = current;
                    if (populateDict) CellIndexDict.Add(current, index);
                    index++;
				}
			}
		}
		return positions;
	}

    public static Vector3 HexToWorld(Vector3 cellCoordinates)
	{
		float x = cellCoordinates.x;
		float y = cellCoordinates.y;
		float z = cellCoordinates.z;
		Vector3 position;

		position.x = x * (HexMetrics.InnerRadius * 2f) + z * HexMetrics.InnerRadius;
		position.y = y * HexMetrics.HeightMultiplier;
		// position.y = 0;
		position.z = z * (HexMetrics.OuterRadius * 1.5f);
		return position;
	}

	public static Vector3 HexChunkToWorld(Vector3 chunkCoordinates)
	{
		float x = chunkCoordinates.x;
        float y = chunkCoordinates.y;
		float z = chunkCoordinates.z;
		Vector3 position;

		position.x = x * (HexMetrics.ChunkOuterRadius * 1.5f);
		position.y = y;
		position.z = z * (HexMetrics.ChunkInnerRadius * 2f) + x * HexMetrics.ChunkInnerRadius;

		return position;
	}
}