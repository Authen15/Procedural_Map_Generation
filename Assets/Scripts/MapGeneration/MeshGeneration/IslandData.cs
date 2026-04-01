using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IslandData
{
	public static int[] CellNeighbours;
	public static AxialCoordinates[] CellCoords;

	public static void GenerateData()
	{
		int gridRadius = Mathf.Max(HexMetrics.IslandRadius, HexMetrics.MapRadius);
		CellNeighbours = new int[HexGridUtils.GetNbCellInHexGrid(gridRadius) * 6];


		Dictionary<AxialCoordinates, int> IslandCellIndexDict = new Dictionary<AxialCoordinates, int>(); // only used to populate neighbours arrays
		GenerateGlobalGridPositions(IslandCellIndexDict, gridRadius);
		HexGridUtils.PopulateNeighboursArray(ref CellNeighbours, IslandCellIndexDict);

		CellCoords = IslandCellIndexDict.Keys.ToArray();
	}


	// the grid is (0,0) at center and (size,size) at bottom right corner
	private static void GenerateGlobalGridPositions(Dictionary<AxialCoordinates, int> dict, int nbCells)
	{
		AxialCoordinates centerCell = new AxialCoordinates(0, 0);
		dict.Add(centerCell, 0);

		// Generate surrounding rings
		int index = 1;
		for (int r = 1; r <= nbCells; r++)
		{
			// Start at (r, 0) for the current ring
			AxialCoordinates current = new AxialCoordinates(r, 0);
			for (int d = 0; d < 6; d++) // 6 directions
			{
				for (int i = 0; i < r; i++) // Cells along current direction
				{
					dict.Add(current, index);
					current += HexGridUtils.GetDir[d];
					index++;
				}
			}
		}
	}
}