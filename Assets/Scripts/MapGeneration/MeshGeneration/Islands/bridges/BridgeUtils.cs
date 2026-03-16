using System.Collections.Generic;
using static HexGridUtils;

public static class BridgeUtils
{
    public static Dictionary<AxialCoordinates, int> BridgeCellsIndexDict;
    public static AxialCoordinates[,] BridgesAnchorCells; // island's cell where the bridge origins from
    public static int BridgeWidth = 6;

    public enum BridgesDir { 
        BottomRight = 0,
        Bottom = 1,
        BottomLeft = 2, 
        TopLeft = 3, 
        Top = 4, 
        TopRight = 5, 
    };

    static BridgeUtils()
	{
        FetchBridgeAnchorCells();
        PopulateBridgeCellDict();
	}
    
    private static void FetchBridgeAnchorCells(){ 
		if (BridgesAnchorCells != null) return; // if already initialized, return
		BridgesAnchorCells = new AxialCoordinates[6, BridgeWidth]; // 6 faces of the hexagon

		int anchorMidCellIndexOffset = GetIslandCellsNumber(HexMetrics.IslandRadius - 1) // index of the first cell in the last ring
                                            + HexMetrics.IslandRadius / 2; // offset to get the middle cell from the bot Right side
		
		for (int i = 0; i < 6; i++){ // iterate through the 6 sides
			for (int j = 0; j < BridgeWidth; j++){ // iterate through the cells of the bridge anchored to the island
				if (HexMetrics.IslandRadius %2 == 0){ // if radius is even, the last ring side have an odd number of cells, the center is a unique cell
					int anchorMidCellIndex = anchorMidCellIndexOffset + i * HexMetrics.IslandRadius; // the offset to get other mid cells is the previous + radius
					AxialCoordinates cellCoord = IslandCellsPositions[anchorMidCellIndex + j - BridgeWidth/2]; // we offset by half the bridgesize to get the cell at the left and at the right of the mid cell
					BridgesAnchorCells[i,j] = cellCoord;
					
				}else
				{ //TODO, handle case when island radius is odd and the center is composed of 2 cells

				}
			}
		}
    }

	private static void PopulateBridgeCellDict()
	{
		BridgeCellsIndexDict = new Dictionary<AxialCoordinates, int>();
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
				AxialCoordinates cellPos = midWidthCellPos + GetDir[(int)Dir.Right] * (x - widthOffset);
				BridgeCellsIndexDict.Add(cellPos, cellIndex++);
			}
		}
	}

	public static Island GetNeighborIsland(AxialCoordinates coord, BridgesDir dir)
	{
		AxialCoordinates islandCoord;
		switch (dir)
		{
			case BridgeUtils.BridgesDir.BottomRight:
				islandCoord = new AxialCoordinates(coord.Q + 1, coord.R);
				break;
			case BridgeUtils.BridgesDir.Bottom:
				islandCoord = new AxialCoordinates(coord.Q, coord.R + 1);
				break;
			case BridgeUtils.BridgesDir.BottomLeft:
				islandCoord = new AxialCoordinates(coord.Q - 1, coord.R + 1);
				break;
			default:
				return null;
		}
		if (!MapManager.Instance.IslandDict.ContainsKey(islandCoord)) return null;
		return MapManager.Instance.IslandDict[islandCoord];
	}
}