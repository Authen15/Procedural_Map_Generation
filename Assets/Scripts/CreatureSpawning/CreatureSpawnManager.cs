using UnityEngine;
using static HexGridUtils;

public class CreatureSpawnManager : MonoBehaviour
{
    private AxialCoordinates _previousIslandCoord;

    public void Start(){
        PlayerEventManager.Instance.OnIslandChanged.AddListener(OnIslandChanged);
    }

    void OnIslandChanged(AxialCoordinates currentIslandCoord)
    {
        if (_previousIslandCoord != null)
        {
            SetActiveNeighboursIslandsSpawner(_previousIslandCoord, false);
        }
        SetActiveNeighboursIslandsSpawner(currentIslandCoord, true);
    }

    private void SetActiveNeighboursIslandsSpawner(AxialCoordinates currentIslandCoord, bool active)
    {
        TrySetActiveIsland(currentIslandCoord, active);

        AxialCoordinates[] neighborCoords = GetCellNeighboursCoords(currentIslandCoord);
        foreach (AxialCoordinates neighborCoord in neighborCoords)
        {
            TrySetActiveIsland(neighborCoord, active);
        }
    }

    private void TrySetActiveIsland(AxialCoordinates islandCoord, bool active){
        if (MapManager.Instance.IslandDict.TryGetValue(islandCoord, out Island island) && island.isInitialised)
        {
            island.IslandSpawner.active = active;
        }
        Debug.LogWarning("Failed to set axtive spawner on Island " + islandCoord);
   
    }
}