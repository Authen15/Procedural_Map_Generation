using System.Collections.Generic;
using UnityEngine;

public static class HexGridUtils
{
	public enum Dir
	{
		BottomLeft = 0,
		Left = 1,
		TopLeft = 2,
		TopRight = 3,
		Right = 4,
		BottomRight = 5
	};

	public static AxialCoordinates[] GetDir = {
		new AxialCoordinates(0, -1),       // bottom left
        new AxialCoordinates(-1, 0),       // left
        new AxialCoordinates(-1, 1),       // top left
        new AxialCoordinates(0, 1),        // top right
        new AxialCoordinates(1, 0),        // right
        new AxialCoordinates(1, -1)        // bottom right
    };

	public static Dictionary<AxialCoordinates, int> IslandCellIndexDict; // Maps cell coordinates to indices
	public static AxialCoordinates[] IslandCellsPositions;
	public static AxialCoordinates[] MapIslandsPositions;

	static HexGridUtils()
	{
		IslandCellIndexDict = new Dictionary<AxialCoordinates, int>();
		IslandCellsPositions = GenerateGridPositions(HexMetrics.IslandSize, true);
		MapIslandsPositions = GenerateGridPositions(HexMetrics.MapSize);
	}

	private static AxialCoordinates[] GenerateGridPositions(int size, bool populateDict = false)
	{
		int radius = size / 2;
		//Total cell number is calcultated using the radius
		//first the center cell + 6 cells around it
		//then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
		// SUM(1->R : R) = R(R+1)/2
		int cellsNumber = 1 + 6 * (radius * (radius + 1) / 2); // Total number of cells
		AxialCoordinates[] positions = new AxialCoordinates[cellsNumber];

		// Add center cell
		AxialCoordinates centerCell = new AxialCoordinates(0, 0);
		positions[0] = centerCell;
		if (populateDict) IslandCellIndexDict.Add(centerCell, 0);

		// Generate surrounding rings
		int index = 1;
		for (int r = 1; r <= radius; r++)
		{
			// Start at (r, 0) for the current ring
			AxialCoordinates current = new AxialCoordinates(r, 0);
			for (int d = 0; d < 6; d++) // 6 directions
			{
				for (int i = 0; i < r; i++) // Cells along current direction
				{
					if (populateDict) IslandCellIndexDict.Add(current, index);
					positions[index] = current;
					current += GetDir[d];
					index++;
				}
			}
		}
		return positions;
	}

	public static int GetIslandCellsNumber(int size)
	{
		return 1 + 6 * (size * (size + 1) / 2);
	}

	public static List<AxialCoordinates> GetCellNeighboursCoords(AxialCoordinates currentCell, bool includeCenter = false)
	{
		List<AxialCoordinates> neighbours = new List<AxialCoordinates>
        {
            currentCell + GetDir[0],
            currentCell + GetDir[1],
            currentCell + GetDir[2],
            currentCell + GetDir[3],
            currentCell + GetDir[4],
            currentCell + GetDir[5]
        };

		if (includeCenter)
		{
			neighbours.Add(currentCell);
		}

		return neighbours;
	}

	public static Vector3 CellToLocal(AxialCoordinates cellCoordinates, AxialCoordinates islandCoord)
	{
		return CellToWorld(cellCoordinates - islandCoord);
	}

	public static Vector3 CellToWorld(AxialCoordinates cellCoordinates)
	{
		float x = cellCoordinates.S;
		float z = cellCoordinates.R;

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

	public static AxialCoordinates WorldToCell(Vector3 worldPosition)
	{
		float x = worldPosition.x;
		float z = worldPosition.z;

		int zCoord = Mathf.RoundToInt(z / (HexMetrics.OuterRadius * 1.5f));

		int xCoord = Mathf.RoundToInt((x - zCoord * HexMetrics.InnerRadius) / (HexMetrics.InnerRadius * 2f));

		return new AxialCoordinates(xCoord, zCoord);
	}

	public static Vector3 IslandToWorld(AxialCoordinates islandCoordinates)
	{
		float x = islandCoordinates.S;
		float z = islandCoordinates.R;

		Vector3 position;
		position.x = x * (HexMetrics.IslandOuterRadius * 1.5f);
		position.y = 0;
		position.z = z * (HexMetrics.IslandInnerRadius * 2f) + x * HexMetrics.IslandInnerRadius;

		return position;
	}

	public static AxialCoordinates WorldToIsland(Vector3 worldPosition)
	{
		float minSqrDist = float.MaxValue;
		AxialCoordinates closestIslandCoord = new AxialCoordinates(0,0);

		// go through islands positions and get the closest distance
		// this is not optimized but there are not too many islands, so it should work fine 
		// optimal way would be a direct computation from coordinates
		foreach (AxialCoordinates coord in MapIslandsPositions)
        {
			Vector3 islandPos = IslandToWorld(coord);
			float sqrDist = (worldPosition - islandPos).sqrMagnitude;
			
			if (sqrDist < minSqrDist){
				minSqrDist = sqrDist;
				closestIslandCoord = coord;
			}
        }
		return closestIslandCoord;
	}

	public static AxialCoordinates GetRandomPositionOnIsland()
	{
		int randomIndex = Random.Range(0, IslandCellsPositions.Length);
		return IslandCellsPositions[randomIndex];
	}

	public static bool IsIslandOutOfMap(AxialCoordinates coord)
    {
		// is the absolute value of the biggest coordinate superior to the map radius 
        return coord.MaxAbs() > HexMetrics.MapRadius;
    }

	public static bool IsCellOutOfIsland(AxialCoordinates coord)
    {
		// is the absolute value of the biggest coordinate superior to the map radius 
        return coord.MaxAbs() > HexMetrics.IslandRadius;
    }
}
