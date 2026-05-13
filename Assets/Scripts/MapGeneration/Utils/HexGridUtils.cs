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
		new AxialCoordinates(-1, 1),       // bottom left
        new AxialCoordinates(-1, 0),       // left
        new AxialCoordinates(0, -1),       // top left
        new AxialCoordinates(1, -1),        // top right
        new AxialCoordinates(1, 0),        // right
        new AxialCoordinates(0, 1)        // bottom right
    };

	public static void PopulateNeighboursArray(ref int[] neighbourArray, Dictionary<AxialCoordinates, int> indexDict)
	{

		foreach (AxialCoordinates coord in indexDict.Keys)
		{
			int cellIndex = indexDict[coord];
			int neighbourIndexOffset = cellIndex * 6;
			AxialCoordinates[] neighbours = GetCellNeighboursCoords(coord);

			for (int i = 0; i < 6; i++)
			{
				if (indexDict.TryGetValue(neighbours[i], out int value))
				{
					neighbourArray[neighbourIndexOffset + i] = value;

				}
				else
				{
					neighbourArray[neighbourIndexOffset + i] = -1;
				}
			}
		}
	}

	// Total cell number is calcultated using the radius
	// first the center cell 1 + 6 cells around it
	// then 12(R=2 * 6) cells to make another ring around, then 18(R=3 * 6) ...
	// SUM(1->R : R) = R(R+1)/2
	// so 1+6*R(R+1)/2
	public static int GetNbCellInHexGrid(int radius)
	{
		return 1 + 3 * (radius * (radius + 1));
	}

	public static AxialCoordinates[] GetCellNeighboursCoords(AxialCoordinates currentCell)
	{
		AxialCoordinates[] neighbours =
		{
			currentCell + GetDir[0],
			currentCell + GetDir[1],
			currentCell + GetDir[2],
			currentCell + GetDir[3],
			currentCell + GetDir[4],
			currentCell + GetDir[5]
		};

		return neighbours;
	}

	public static int[] GetCellNeighbours(in int[] CellNeighbours, int index)
	{
		int neighbourOffset = index * 6;

		int[] neighbours =
		{
			CellNeighbours[neighbourOffset],
			CellNeighbours[neighbourOffset+1],
			CellNeighbours[neighbourOffset+2],
			CellNeighbours[neighbourOffset+3],
			CellNeighbours[neighbourOffset+4],
			CellNeighbours[neighbourOffset+5],
		};

		return neighbours;
	}

	public static int[] GetCellAndNeighbours(in int[] CellNeighbours, int index)
	{
		int neighbourOffset = index * 6;

		int[] neighbours =
		{
			index,
			CellNeighbours[neighbourOffset],
			CellNeighbours[neighbourOffset+1],
			CellNeighbours[neighbourOffset+2],
			CellNeighbours[neighbourOffset+3],
			CellNeighbours[neighbourOffset+4],
			CellNeighbours[neighbourOffset+5],
		};

		return neighbours;
	}

	public static Vector3 CellToLocal(AxialCoordinates cellCoordinates, AxialCoordinates islandCoord)
	{
		return CellToWorld(cellCoordinates - islandCoord);
	}

	public static Vector3 CellToWorld(AxialCoordinates cellCoordinates)
	{
		float x = cellCoordinates.Q;
		float z = cellCoordinates.R;

		Vector3 position;
		position.x = x * (HexMetrics.InnerRadius * 2f) + z * HexMetrics.InnerRadius;
		position.y = 0;
		position.z = -z * (HexMetrics.OuterRadius * 1.5f);

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
		float x = islandCoordinates.Q;
		float z = islandCoordinates.R;

		Vector3 position;
		position.x = x * (HexMetrics.IslandOuterRadius * 1.5f);
		position.y = 0;
		position.z = -(z * (HexMetrics.IslandInnerRadius * 2f) + x * HexMetrics.IslandInnerRadius);

		return position;
	}

	public static Vector3 IslandToWorld(int index)
	{
		return IslandToWorld(IslandData.CellCoords[index]);
	}

	public static int WorldToIsland(Vector3 worldPosition)
	{
		float minSqrDist = float.MaxValue;
		int closestIsland = 0;

		// go through islands positions and get the closest distance
		// this is not optimized but there are not too many islands, so it should work fine 
		// optimal way would be a direct computation from coordinates
		for (int i = 0; i < HexMetrics.NbCellInMap; i++)
		{
			Vector3 islandPos = IslandToWorld(IslandData.CellCoords[i]);
			float sqrDist = (worldPosition - islandPos).sqrMagnitude;

			if (sqrDist < minSqrDist)
			{
				minSqrDist = sqrDist;
				closestIsland = i;
			}
		}
		return closestIsland;
	}

	public static AxialCoordinates GetRandomPositionOnIsland()
	{
		int randomIndex = Random.Range(0, IslandData.CellCoords.Length);
		return IslandData.CellCoords[randomIndex];
	}

	public static bool IsIslandOutOfMap(int index)
	{
		return index > HexMetrics.NbCellInMap;
	}

	public static bool IsIslandOutOfMap(AxialCoordinates coord)
	{
		return coord.RingNumber() > HexMetrics.MapRadius;
	}

	public static bool IsCellOutOfIsland(AxialCoordinates coord)
	{
		return coord.RingNumber() > HexMetrics.IslandRadius;
	}
}
