using System;
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

    public static AxialCoordinates[] GetDir = {
        new AxialCoordinates(0, -1), 		// bottom left
        new AxialCoordinates(-1, 0), 		// left
        new AxialCoordinates(-1, 1), 		// top left
        new AxialCoordinates(0, 1), 		// top right
        new AxialCoordinates(1, 0), 		// right
        new AxialCoordinates(1, -1) 		// bottom right
    };

	public struct AxialCoordinates {
		public int x;
		public int z;

		public AxialCoordinates(int x, int z){
			this.x = x;
			this.z = z;
		}

		public static AxialCoordinates operator +(AxialCoordinates a, AxialCoordinates b)
		{
			return new AxialCoordinates
			{
				x = a.x + b.x,
				z = a.z + b.z
			};
		}

		public static AxialCoordinates operator *(AxialCoordinates a, int b)
		{
			return new AxialCoordinates
			{
				x = a.x * b,
				z = a.z * b
			};
		}
	}

	// public struct OffsetCoordinates { // event row based 
	// 	public int x;
	// 	public int z;

	// 	public OffsetCoordinates(int x, int z){
	// 		this.x = x;
	// 		this.z = z;
	// 	}
	// }

	// public static OffsetCoordinates AxialToOffset(AxialCoordinates aCoord){
	// 	OffsetCoordinates oCoord = new OffsetCoordinates();
	// 	oCoord.x = aCoord.x + (int)(0.5f * aCoord.z - aCoord.z / 2); // adding the offset if the row is even
    // 	oCoord.z = aCoord.z;
    // 	return oCoord;
	// }
    

	// public static AxialCoordinates OffsetToAxial(OffsetCoordinates oCoord){
	// 	AxialCoordinates ACoord = new AxialCoordinates();
	// 	ACoord.x = oCoord.x - (int)(0.5f * oCoord.z - oCoord.z / 2);
	// 	ACoord.z = oCoord.z;
	// 	return ACoord;
	// }
		
    

    public static Dictionary<AxialCoordinates, int> IslandCellIndexDict; // contains the index of a cell in an array using it's coordinates in the grid
    
    public static AxialCoordinates[] IslandCellsPositions;

    public static AxialCoordinates[] MapIslandsPositions;

    static HexGridUtils(){
        IslandCellIndexDict = new Dictionary<AxialCoordinates, int>();
        IslandCellsPositions = GenerateGridPositions(HexMetrics.IslandSize, true);
        MapIslandsPositions = GenerateGridPositions(HexMetrics.MapSize);
    }

	private static AxialCoordinates[] GenerateGridPositions(int size, bool populateDict = false){
		int radius = size / 2;

		//Total cell number is can be calcultated using the radius
		//first the center cell + 6 cells around it
		//then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
		// SUM(1->R : R) = R(R+1)/2
		int cellsNumber = 1 + 6 * (radius * (radius + 1) / 2);
		AxialCoordinates[] positions = new AxialCoordinates[cellsNumber];
		
		// Add center cell
        AxialCoordinates centerCell = new AxialCoordinates(0, 0);
		positions[0] = centerCell;
        if (populateDict) IslandCellIndexDict.Add(centerCell, 0);

		// Iterate through rings
        int index = 1;
		for (int r = 1; r <= radius; r++) {
			// Start at (r, 0) for the current ring
			AxialCoordinates current = new AxialCoordinates(r, 0);

			for (int d = 0; d < 6; d++) {  // 6 directions
				for (int i = 0; i < r; i++) {  // Number of cells in the current direction
                    if (populateDict) IslandCellIndexDict.Add(current, index);
					positions[index] = current;
					current += GetDir[d];
                    index++;
				}
			}
		}
		return positions;
	}

	public static int GetIslandCellsNumber(int size){
		return 1 + 6 * (size * (size + 1) / 2);
	}

    public static Vector3 CellToWorld(AxialCoordinates cellCoordinates)
	{
		float x = cellCoordinates.x;
		float z = cellCoordinates.z;
		Vector3 position;

		position.x = x * (HexMetrics.InnerRadius * 2f) + z * HexMetrics.InnerRadius;
		position.y = 0;
		position.z = z * (HexMetrics.OuterRadius * 1.5f);
		return position;
	}

	public static Vector3 CellToWorld(AxialCoordinates cellCoordinates, float height)
	{
		Vector3 position = CellToWorld(cellCoordinates);
		position.y = height * HexMetrics.HeightMultiplier;

		return position;
	}

	// return the cell coordinate in the island from a world position 
	public static AxialCoordinates WorldToCell(Vector3 worldPosition)
	{
		float x = worldPosition.x;
		float z = worldPosition.z;
		AxialCoordinates position = new AxialCoordinates();

		position.x = (int)((x - z * HexMetrics.InnerRadius) / (HexMetrics.InnerRadius * 2f));
		position.z = (int)(z / (HexMetrics.OuterRadius * 1.5f));
		return position;
	}

	public static Vector2 CellToUV(Vector2 cellCoordinates) // must be axial coordinates but we keep vector2 to be able to use float
	{
		float outerRadius = HexMetrics.OuterRadius;
		float x = cellCoordinates.x;
		float y = cellCoordinates.y;
		Vector2 position;

		position.x = x * (outerRadius * 2f) + y * outerRadius;
		position.y = y * (outerRadius * 2f);
		return position;
	}

	public static Vector3 IslandToWorld(AxialCoordinates islandCoordinates)
	{
		float x = islandCoordinates.x;
		float z = islandCoordinates.z;
		Vector3 position;

		position.x = x * (HexMetrics.IslandOuterRadius * 1.5f);
		position.y = 0;
		position.z = z * (HexMetrics.IslandInnerRadius * 2f) + x * HexMetrics.IslandInnerRadius;

		return position;
	}
}