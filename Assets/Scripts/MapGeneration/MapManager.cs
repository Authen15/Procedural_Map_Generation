using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	public static MapManager Instance { get; private set; }

	public Dictionary<AxialCoordinates, Island> IslandDict;

	public int MapSeed = 0;

	private IslandDisplay _islandDisplay;
	private IslandPool _islandPool;

	void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
			return;
		}
		Instance = this;

		_islandPool = GetComponent<IslandPool>();
	}

	void Start()
	{
		IslandDict = new Dictionary<AxialCoordinates, Island>();
		_islandDisplay = new IslandDisplay(this);
	}

	public void DestroyIsland(AxialCoordinates islandCoord)
	{
		IslandDict.Remove(islandCoord);
		_islandPool.DespawnIsland(islandCoord);
	}

	public IEnumerator GenerateIslands(AxialCoordinates[] islandCoords)
	{
		int nbIslands = islandCoords.Length;

		for (int i = 0; i < nbIslands; i++)
		{
			Island island = _islandPool.SpawnIsland(islandCoords[i]);

			IslandDict.Add(islandCoords[i], island);

			yield return null; // generate one per frame
		}

		// updating bridges after islands as bridges need to know their neighbours
		foreach (var island in IslandDict.Values)
		{
			island.UpdateBridges();
			yield return null;
		}
	}

}
