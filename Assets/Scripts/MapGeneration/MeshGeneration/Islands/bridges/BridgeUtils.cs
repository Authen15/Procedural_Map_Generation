using System.Collections.Generic;
using System.Linq;

public static class BridgeUtils
{
	public const int BridgeWidth = 6;
	public const int NbCellInBridge = HexMetrics.DistanceBetweenIslands * BridgeWidth
									- HexMetrics.DistanceBetweenIslands / 2; // every first cell in odd row are skipped

	public enum BridgesDir
	{
		BottomRight = 0,
		Bottom = 1,
		BottomLeft = 2,
		TopLeft = 3,
		Top = 4,
		TopRight = 5,
	};

	public static Island GetNeighbourIsland(int index, BridgesDir dir)
	{
		int[] neighbours = HexGridUtils.GetCellNeighbours(IslandData.CellNeighbours, index);
		int neighboursIndex = neighbours[((int)dir + 4) % 6]; // the direction is reversed because we want to get the current island from the neighbour's perspective
		return (neighboursIndex >= 0 && neighboursIndex < HexMetrics.NbCellInMap) ? MapManager.Instance.IslandViews[neighboursIndex] : null;
	}
}