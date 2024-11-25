using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static HexGridUtils;

public class IslandSpawner : MonoBehaviour{

    public GameObject CreaturePrefab;

    private const int MAXIMUM_CREATURE_AMOUNT = 64;

    public Island Island;

    private int _creaturesAmount;
    
    public void PopulateIslandCreatures(){
        while (_creaturesAmount < MAXIMUM_CREATURE_AMOUNT) {
            AxialCoordinates randomCoord = GetRandomPositionOnIsland();
            float height = Island.GetCellHeightMapValue(randomCoord);
            Vector3 randomWorldPos = CellToWorld(randomCoord, height);

            GameObject creature = Instantiate(CreaturePrefab, transform);
            creature.transform.localPosition = randomWorldPos;
            creature.name = "creature " + _creaturesAmount;

            OnCreatureCreated();
        }
    }

    public void OnCreatureCreated(){
         _creaturesAmount++;
    }

    public void OnCreatureDeath(){
        _creaturesAmount--;
    }
}