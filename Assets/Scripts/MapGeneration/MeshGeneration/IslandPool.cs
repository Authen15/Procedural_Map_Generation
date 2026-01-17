using System.Collections.Generic;
using UnityEngine;

public class IslandPool : MonoBehaviour
{
    [SerializeField] private GameObject _islandPrefab;
    
    private int _poolSize => HexGridUtils.GetIslandCellsNumber(1);
    private Queue<Island> _availableIslands = new Queue<Island>();
    private Dictionary<AxialCoordinates, Island> _activeIslands = new Dictionary<AxialCoordinates, Island>();
    
    private void Awake()
    {
        for (int i = 0; i < _poolSize; i++)
        {
            Island island = Instantiate(_islandPrefab).GetComponent<Island>();
            island.gameObject.SetActive(false);
            _availableIslands.Enqueue(island);
        }
    }
    
    public Island SpawnIsland(AxialCoordinates coord)
    {
        if (_activeIslands.ContainsKey(coord))
            return _activeIslands[coord];
        
        if (_availableIslands.Count == 0)
        {
            Debug.LogWarning("Pool is empty, creating a new Island");
            Island newIsland = Instantiate(_islandPrefab).GetComponent<Island>();
            _availableIslands.Enqueue(newIsland);
        }
        
        Island island = _availableIslands.Dequeue();
        island.gameObject.SetActive(true);
        island.Initialize(coord);
        _activeIslands[coord] = island;
        return island;
    }
    
    public void DespawnIsland(AxialCoordinates coord)
    {
        if (!_activeIslands.TryGetValue(coord, out Island island))
            return;
        
        island.Cleanup();
        island.gameObject.SetActive(false);
        
        _activeIslands.Remove(coord);
        _availableIslands.Enqueue(island);
    }
}