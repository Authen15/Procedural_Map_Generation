using System.Collections.Generic;
using static HexGridUtils;

public static class BridgeUtils
{
    public static Dictionary<AxialCoordinates, int> BridgeCellsIndexDict;
    public static AxialCoordinates[,] BridgesAnchorCells;
    public static int BridgeSize = 6;

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
        PopulateBridgeCellDict(BridgeSize);
	}
    
    private static void FetchBridgeAnchorCells(){ 
		if (BridgesAnchorCells != null) return; // if already initialized, return
		BridgesAnchorCells = new AxialCoordinates[6, BridgeSize]; // 6 faces of the hexagon

		int anchorMidCellIndexOffset = GetIslandCellsNumber(HexMetrics.IslandRadius - 1) // index of the first cell in the last ring
                                            + HexMetrics.IslandRadius / 2; // offset to get the middle cell from the bot Right side
		
		for (int i = 0; i < 6; i++){ // iterate through the 6 sides
			for (int j = 0; j < BridgeSize; j++){ // iterate through the cells of the bridge anchored to the island
				if (HexMetrics.IslandRadius %2 == 0){ // if radius is even, the last ring side have an odd number of cells, the center is a unique cell
					int anchorMidCellIndex = anchorMidCellIndexOffset + i * HexMetrics.IslandRadius; // the offset to get other mid cells is the previous + radius
					AxialCoordinates cellCoord = IslandCellsPositions[anchorMidCellIndex + j - BridgeSize/2]; // we offset by half the bridgesize to get the cell at the left and at the right of the mid cell
					BridgesAnchorCells[i,j] = cellCoord;
					
				}else
				{ //TODO, handle case when island radius is odd and the center is composed of 2 cells

				}
			}
		}
    }

    private static void PopulateBridgeCellDict(int bridgeSize){
		BridgeCellsIndexDict = new Dictionary<AxialCoordinates, int>();
		int cellIndex = 0;
		for (int z = 0; z < HexMetrics.DistanceBetweenIslands; z++){
			AxialCoordinates midCellPos = new AxialCoordinates(-z/2, z+1); // get the middle cell, -z/2 to avoid the offset, 
			int offset = bridgeSize/2; // we place cells from left to right of the middle cell
			for (int x = 0; x < bridgeSize; x++){
				bool isEvenRow = z % 2 == 0;
				if (x == 0 && isEvenRow) continue; // skip the first cell in even rows
				AxialCoordinates cellPos = midCellPos + GetDir[(int)Dir.Right] * (x - offset);
				BridgeCellsIndexDict.Add(cellPos, cellIndex++);
			}
		}
	}
}