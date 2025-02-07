using Unity.Mathematics;
using UnityEngine;
using static HexGridUtils;

public class IslandSpawner : MonoBehaviour{

    public GameObject CreaturePrefab;
    public Island Island;
    public GameObject Player;

    private bool active;

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

    public void SetActive(bool active){
        this.active = active;
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
            spawnPosition = GetRandomSpawnPosition() + transform.position; // add the object position to get the world position
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

    public void OnDrawGizmosSelected(){
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(Player.transform.position, MIN_PLAYER_DISTANCE);
    }
}