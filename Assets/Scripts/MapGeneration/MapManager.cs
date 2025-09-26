using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
	public static MapManager Instance { get; private set; }

	public Dictionary<AxialCoordinates, Island> IslandDict;

	public int MapSeed = 0;

	public Island IslandPrefab;
	
	void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
	
	void Start()
	{
		IslandDict = new Dictionary<AxialCoordinates, Island>();

		float startTime = Time.realtimeSinceStartup;
		GenerateMap();
		Debug.Log("Generating time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
	}

	void GenerateMap()
	{
		AxialCoordinates[] islandsPositions = HexGridUtils.MapIslandsPositions;

		foreach (AxialCoordinates islandCoordinates in islandsPositions)
		{
			// convert to vector3 for world position
			Vector3 islandPos = HexGridUtils.IslandToWorld(islandCoordinates);

			Island island = Instantiate(IslandPrefab, islandPos, Quaternion.identity, transform);

			IslandDict.Add(islandCoordinates, island);

			island.Initialize(islandCoordinates);
		}

		foreach (Island island in IslandDict.Values){
			island.GenerateIsland();
		}
	}
}
