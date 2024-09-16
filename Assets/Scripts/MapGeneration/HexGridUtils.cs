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

    public static Vector2[] GetDir = {
        new Vector2(0, -1), 	// bottom left
        new Vector2(-1, 0), 	// left
        new Vector2(-1, 1), 	// top left
        new Vector2(0, 1), 		// top right
        new Vector2(1, 0), 		// right
        new Vector2(1, -1) 		// bottom right
    };
    

    public static Dictionary<Vector3, int> CellIndexDict; // contaiu the index of a cell in an array using it's coordinates in the grid
    
    public static Vector2[] ChunkCellsPositions;

    public static Vector2[] MapChunksPositions;

    static HexGridUtils(){
        CellIndexDict = new Dictionary<Vector3, int>();
        ChunkCellsPositions = GenerateGridPositions(HexMetrics.IslandSize, true);
        MapChunksPositions = GenerateGridPositions(HexMetrics.MapSize);
    }

	private static Vector2[] GenerateGridPositions(int size, bool populateDict = false){
		int radius = size / 2;

		//Total cell number is can be calcultated using the radius
		//first the center cell + 6 cells around it
		//then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
		// SUM(1->R : R) = R(R+1)/2
		int cellsNumber = 1 + 6 * (radius * (radius + 1) / 2);
		Vector2[] positions = new Vector2[cellsNumber];
		
		// Add center cell
        Vector2 centerCell = new Vector2(0, 0);
		positions[0] = centerCell;
        if (populateDict) CellIndexDict.Add(centerCell, 0);

		// Iterate through rings
        int index = 1;
		for (int r = 1; r <= radius; r++) {
			// Start at (r, 0) for the current ring
			Vector2 current = new Vector2(r, 0);

			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
                    if (populateDict) CellIndexDict.Add(current, index);
					positions[index] = current;
					current += GetDir[d];
                    index++;
				}
			}
		}
		return positions;
	}

	public static int GetRingStartIndex(int ringNumber){
		return 1 + 6 * (ringNumber * (ringNumber + 1) / 2);
	}

    public static Vector3 HexToWorld(Vector2 cellCoordinates)
	{
		float x = cellCoordinates.x;
		float z = cellCoordinates.y;
		Vector3 position;

		position.x = x * (HexMetrics.InnerRadius * 2f) + z * HexMetrics.InnerRadius;
		position.y = 0;
		position.z = z * (HexMetrics.OuterRadius * 1.5f);
		return position;
	}

	// public static Vector2 HexToWorld(Vector2 cellCoordinates)
	// {
	// 	float x = cellCoordinates.x;
	// 	float y = cellCoordinates.y;
	// 	Vector2 position;

	// 	position.x = x * (HexMetrics.InnerRadius * 2f) + y * HexMetrics.InnerRadius;
	// 	position.y = y * (HexMetrics.OuterRadius * 1.5f);
	// 	return position;
	// }

	public static Vector2 HexToUV(Vector2 cellCoordinates)
	{
		float outerRadius = HexMetrics.OuterRadius;
		float x = cellCoordinates.x;
		float y = cellCoordinates.y;
		Vector2 position;

		position.x = x * (outerRadius * 2f) + y * outerRadius;
		position.y = y * (outerRadius * 2f);
		return position;
	}

	public static Vector3 HexChunkToWorld(Vector2 chunkCoordinates)
	{
		float x = chunkCoordinates.x;
		float z = chunkCoordinates.y;
		Vector3 position;

		position.x = x * (HexMetrics.ChunkOuterRadius * 1.5f);
		position.y = 0;
		position.z = z * (HexMetrics.ChunkInnerRadius * 2f) + x * HexMetrics.ChunkInnerRadius;

		return position;
	}
}