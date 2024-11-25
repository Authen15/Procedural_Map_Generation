using UnityEngine;
using static HexGridUtils;

public class CreatureSpawnManager : MonoBehaviour
{
    public void Start(){
        PlayerEventManager.Instance.OnIslandChanged.AddListener(OnIslandChanged);
    }

    void OnIslandChanged(AxialCoordinates currentIslandCoord)
    {
        PopulateNeighboursIslands(currentIslandCoord);
    }
    

    private void PopulateNeighboursIslands(AxialCoordinates currentIslandCoord)
    {
        TryPopulateIsland(currentIslandCoord);

        AxialCoordinates[] neighborCoords = GetCellNeighboursCoords(currentIslandCoord);
        foreach (AxialCoordinates neighborCoord in neighborCoords)
        {
            TryPopulateIsland(neighborCoord);
        }
    }

    private void TryPopulateIsland(AxialCoordinates islandCoord){
        if (MapManager.Instance.IslandDict.TryGetValue(islandCoord, out Island island) && island.isInitialised)
        {
            island.PopulateIslandCreatures();
        }
        Debug.LogWarning("Failed to populate creatures on Island " + islandCoord);
   
    }
}