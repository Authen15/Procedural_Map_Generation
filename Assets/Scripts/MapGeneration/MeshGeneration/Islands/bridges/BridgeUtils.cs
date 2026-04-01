using System.Collections.Generic;
using System.Linq;

public static class BridgeUtils
{
    public const int BridgeWidth = 6;
	public const int NbCellInBridge = HexMetrics.DistanceBetweenIslands * BridgeWidth
									- HexMetrics.DistanceBetweenIslands / 2; // every first cell in odd row are skipped

	public enum BridgesDir { 
        BottomRight = 0,
        Bottom = 1,
        BottomLeft = 2, 
        TopLeft = 3, 
        Top = 4, 
        TopRight = 5, 
    };

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