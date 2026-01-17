using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class IslandDisplay
{
    private List<AxialCoordinates> _lastVisibleIslands = new List<AxialCoordinates>();

    // private int _rangeChunksToDisplay = 1;

    private MapManager _mapManager;

    public IslandDisplay(MapManager mapManager)
    {
        _mapManager = mapManager;
        GenerateNeededIslands(new List<AxialCoordinates>{new AxialCoordinates(0,0)}); // Generate the first island on startup

        PlayerEventManager.Instance.OnIslandChanged.AddListener(UpdateVisibleChunks);
    }

    public void UpdateVisibleChunks(AxialCoordinates currentIslandCoord) {
        if(currentIslandCoord == null){
            Debug.Log("Current chunk position is null");
            return;
        }

        List<AxialCoordinates> visibleIslandsCoords = HexGridUtils.GetCellNeighboursCoords(currentIslandCoord, includeCenter: true); // todo, add another argument for the range
        
        // inverse loop so no indices can be skipped
        for (int i = visibleIslandsCoords.Count - 1; i >= 0; i--)
        {
            var coord = visibleIslandsCoords[i];
            if (HexGridUtils.IsIslandOutOfMap(coord))
            {
                visibleIslandsCoords.RemoveAt(i);
            }
        }

        ClearNotVisibleIslands(visibleIslandsCoords);
        GenerateNeededIslands(visibleIslandsCoords);
    }

    public void GenerateNeededIslands(List<AxialCoordinates> visibleIslandsCoords){
        var islandsToGenerate = new List<AxialCoordinates>();

        foreach(AxialCoordinates islandCoord in visibleIslandsCoords){
            if(!_lastVisibleIslands.Contains(islandCoord)){
                islandsToGenerate.Add(islandCoord);
                _lastVisibleIslands.Add(islandCoord);
            }
        }
        if (islandsToGenerate.Count() > 0) _mapManager.StartCoroutine(_mapManager.GenerateIslands(islandsToGenerate.ToArray()));
    }
        

    public void ClearNotVisibleIslands(List<AxialCoordinates> visibleIslandsCoords){
        var islandsToRemove = new List<AxialCoordinates>();

        foreach (AxialCoordinates islandCoord in _lastVisibleIslands)
        {
            if (!visibleIslandsCoords.Contains(islandCoord))
            {
                islandsToRemove.Add(islandCoord);
            }
        }

        foreach (AxialCoordinates coord in islandsToRemove)
        {
            _lastVisibleIslands.Remove(coord);
            _mapManager.DestroyIsland(coord);
        }
    }
}
