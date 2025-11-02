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

	public static AxialCoordinates[] GetCellNeighboursCoords(AxialCoordinates currentCell)
	{
		AxialCoordinates[] neighbours = new AxialCoordinates[6];
		for (int j = 0; j < 6; j++)
		{
			neighbours[j] = currentCell + GetDir[j];
		}
		return neighbours;
	}

	public static Vector3 CellToLocal(AxialCoordinates cellCoordinates, AxialCoordinates islandCoord)
	{
		return CellToWorld(cellCoordinates - islandCoord);
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

	public static AxialCoordinates WorldToCell(Vector3 worldPosition)
	{
		float x = worldPosition.x;
		float z = worldPosition.z;

		// Calculate z (row), rounding to the nearest cell
		int zCoord = Mathf.RoundToInt(z / (HexMetrics.OuterRadius * 1.5f));

		// Calculate x (column), considering z's contribution
		int xCoord = Mathf.RoundToInt((x - zCoord * HexMetrics.InnerRadius) / (HexMetrics.InnerRadius * 2f));

		return new AxialCoordinates(xCoord, zCoord);
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

	public static AxialCoordinates WorldToIsland(Vector3 worldPosition)
	{
		float x = worldPosition.x;
		float z = worldPosition.z;

		// Calculate x (column), rounding to the nearest island coordinate
		int xCoord = Mathf.RoundToInt(x / (HexMetrics.IslandOuterRadius * 1.5f));

		// Calculate z (row), considering x's contribution
		int zCoord = Mathf.RoundToInt((z - xCoord * HexMetrics.IslandInnerRadius) / (HexMetrics.IslandInnerRadius * 2f));

		return new AxialCoordinates(xCoord, zCoord);
	}

	public static AxialCoordinates GetRandomPositionOnIsland()
	{
		int randomIndex = Random.Range(0, IslandCellsPositions.Length);
		return IslandCellsPositions[randomIndex];
	}
}
