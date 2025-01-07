using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using static HexGridUtils;

public class IslandSpawner : MonoBehaviour{

    public GameObject CreaturePrefab;
    public Island Island;
    public GameObject Player;

    public bool active;

    private const int MAXIMUM_CREATURE_AMOUNT = 48;
    private const float MIN_SPAWN_DURATION = 2f;
    private const float MAX_SPAWN_DURATION = 6f;
    private const float MIN_PLAYER_DISTANCE = 50f; // Minimum distance from player to spawn a creature
    private const int MAX_RANDOM_SPAWN_ITERATIONS = 10; // Maximum number of attempts to spawn a creature


    private int _creaturesAmount;
    private float _spawnDuration;

    public void Start(){
        Player = GameObject.FindGameObjectWithTag("Player");
    }
    
    private void PopulateIslandCreatures()
    {
        if (!TryGetValidSpawnPosition(out Vector3 spawnPosition))
        {
            return;
        }

        GameObject creature = Instantiate(CreaturePrefab, spawnPosition, quaternion.identity);
        creature.transform.parent = transform;
        creature.name = $"creature {_creaturesAmount}";

        OnCreatureCreated();
    }

    private bool TryGetValidSpawnPosition(out Vector3 spawnPosition)
    {
        for (int attemptCount = 0; attemptCount < MAX_RANDOM_SPAWN_ITERATIONS; attemptCount++)
        {
            spawnPosition = GetRandomSpawnPosition();
            if (CanSpawnCreatureAtPosition(spawnPosition))
            {
                return true;
            }
        }

        spawnPosition = Vector3.zero;
        return false;
    }

    private Vector3 GetRandomSpawnPosition(){
        AxialCoordinates randomCoord = GetRandomPositionOnIsland();
        float height = Island.GetCellHeightMapValue(randomCoord);
        return CellToWorld(randomCoord, height);
    }

    private bool CanSpawnCreatureAtPosition(Vector3 position){
        return Vector3.Distance(Player.transform.position, position) > MIN_PLAYER_DISTANCE;
    }

    public void Update(){
        if (active && _creaturesAmount < MAXIMUM_CREATURE_AMOUNT){
            _spawnDuration -= Time.deltaTime;
            if (_spawnDuration <= 0){
                PopulateIslandCreatures();
                _spawnDuration = UnityEngine.Random.Range(MIN_SPAWN_DURATION, MAX_SPAWN_DURATION);
            }
        }
    }

    public void OnCreatureCreated(){
        _creaturesAmount++;
    }

    public void OnCreatureDeath(){
        _creaturesAmount--;
    }

    public void OnDrawGizmos(){
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(Player.transform.position, MIN_PLAYER_DISTANCE);
    }
}