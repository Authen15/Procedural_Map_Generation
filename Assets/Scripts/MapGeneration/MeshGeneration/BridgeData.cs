using System.Collections.Generic;
using System.Linq;
using static BridgeUtils;

public static class BridgeData
{
	public static int[] BridgeCellNeighbours;
	public static AxialCoordinates[] BridgeCellCoords;
	public static AxialCoordinates[,] BridgesAnchorCellCoord; // island's cell where the bridge origins from

	public static void GenerateData()
	{

		BridgeCellNeighbours = new int[NbCellInBridge * 6];
		BridgesAnchorCellCoord = new AxialCoordinates[6, BridgeWidth];
		Dictionary<AxialCoordinates, int> BridgeCellIndexDict = new Dictionary<AxialCoordinates, int>();  // only used to populate neighbours arrays

		FetchBridgeAnchorCells();
		GenerateBridgeGridPositions(BridgeCellIndexDict);

		HexGridUtils.PopulateNeighboursArray(ref BridgeCellNeighbours, BridgeCellIndexDict);

		BridgeCellCoords = BridgeCellIndexDict.Keys.ToArray();
	}

	private static void FetchBridgeAnchorCells()
	{
		int anchorMidCellIndexOffset = HexGridUtils.GetNbCellInHexGrid(HexMetrics.IslandRadius - 1) // index of the first cell in the last ring
											+ HexMetrics.IslandRadius / 2; // offset to get the middle cell from the bot Right side

		for (int i = 0; i < 6; i++)
		{ // iterate through the 6 sides
			for (int j = 0; j < BridgeWidth; j++)
			{ // iterate through the cells of the bridge anchored to the island
				if (HexMetrics.IslandRadius % 2 == 0)
				{ // if radius is even, the last ring side have an odd number of cells, the center is a unique cell
					int anchorMidCellIndex = anchorMidCellIndexOffset + i * HexMetrics.IslandRadius; // the offset to get other mid cells is the previous + radius
					AxialCoordinates cellCoord = IslandData.CellCoords[anchorMidCellIndex + j - BridgeWidth / 2]; // we offset by half the bridgesize to get the cell at the left and at the right of the mid cell
					BridgesAnchorCellCoord[i, j] = cellCoord;

				}
				else
				{ //TODO, handle case when island radius is odd and the center is composed of 2 cells

				}
			}
		}
	}

	private static void GenerateBridgeGridPositions(Dictionary<AxialCoordinates, int> BridgeCellIndexDict)
	{
		int cellIndex = 0;
		for (int z = 0; z < HexMetrics.DistanceBetweenIslands; z++)
		{
			// z/2 give an horizontal offset compared to the anchor cell
			// then start just above the anchor cell (-1)
			// and each iteration go up one unit (-z)
			AxialCoordinates midWidthCellPos = new AxialCoordinates(z / 2, -1 - z);
			int widthOffset = BridgeWidth / 2; // place cells from left to right of the middle cell
			for (int x = 0; x < BridgeWidth; x++)
			{
				bool isOddRow = z % 2 == 1;
				if (x == 0 && isOddRow) continue; // skip the first cell in odd rows
				AxialCoordinates cellPos = midWidthCellPos + HexGridUtils.GetDir[(int)HexGridUtils.Dir.Right] * (x - widthOffset);
				BridgeCellIndexDict[cellPos] = cellIndex;

				cellIndex++;
			}
		}
	}

}