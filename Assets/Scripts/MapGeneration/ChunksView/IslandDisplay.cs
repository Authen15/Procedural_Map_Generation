using System.Collections.Generic;
using System.Linq;
using static IslandData;

public class IslandDisplay
{
    private List<int> _lastVisibleIslands = new List<int>();

    // private int _rangeChunksToDisplay = 1;

    private MapManager _mapManager;

    public IslandDisplay(MapManager mapManager)
    {
        _mapManager = mapManager;
        GenerateNeededIslands(new List<int> { 0 }); // Generate the first island on startup

        PlayerEventManager.Instance.OnIslandChanged.AddListener(UpdateVisibleChunks);
    }

    public void UpdateVisibleChunks(int currentInsland)
    {
        int[] islandNeighbours = HexGridUtils.GetCellAndNeighbours(CellNeighbours, currentInsland); // todo, add another argument for the range

        List<int> visibleIslands = new List<int>();
        for (int i = 0; i < islandNeighbours.Length; i++)
        {
            if (islandNeighbours[i] < 0 || islandNeighbours[i] >= HexMetrics.NbCellInMap) continue; // if there is no neighbour, or the neighbour is out of the map, skip it
            visibleIslands.Add(islandNeighbours[i]);
        }

        // inverse loop so no indices can be skipped
        for (int i = visibleIslands.Count - 1; i >= 0; i--)
        {
            var index = visibleIslands[i];
            if (HexGridUtils.IsIslandOutOfMap(index))
            {
                visibleIslands.RemoveAt(i);
            }
        }

        ClearNotVisibleIslands(visibleIslands);
        GenerateNeededIslands(visibleIslands);
    }

    public void GenerateNeededIslands(List<int> visibleIslands)
    {
        var islandsToGenerate = new List<int>();

        foreach (int island in visibleIslands)
        {
            if (!_lastVisibleIslands.Contains(island))
            {
                islandsToGenerate.Add(island);
                _lastVisibleIslands.Add(island);
            }
        }
        if (islandsToGenerate.Any()) _mapManager.GenerateIslands(islandsToGenerate.ToArray());
    }


    public void ClearNotVisibleIslands(List<int> visibleIslands)
    {
        var visibleSet = new HashSet<int>(visibleIslands);
        var islandsToRemove = _lastVisibleIslands.Where(island => !visibleSet.Contains(island)).ToList();

        _lastVisibleIslands.RemoveAll(island => !visibleSet.Contains(island));
        _mapManager.DestroyIslands(islandsToRemove.ToArray());
    }
}
