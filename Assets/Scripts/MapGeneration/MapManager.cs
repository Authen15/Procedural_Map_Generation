using UnityEngine;

public class MapManager : MonoBehaviour
{
	private static MapManager _instance;
	public static MapManager Instance{
		get{
			if (_instance == null){
				Debug.LogWarning("HexMeshGrid is null");
			}
			return _instance;
		}
	}


	public int MapSeed = 0;

	public Island IslandPrefab;
	
	void Awake(){
		_instance = this;
	}
	
	void Start()
	{
		float startTime = Time.realtimeSinceStartup;
		GenerateMap();
		Debug.Log("Generating time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
	}

	void GenerateMap()
	{
		Vector2[] islandsPositions = HexGridUtils.MapIslandsPositions;

		foreach (Vector2 islandCoordinates in islandsPositions )
		{
			// convert to vector3 for world position
			Vector3 islandPos = HexGridUtils.IslandToWorld(new Vector2((int)islandCoordinates.x, (int)islandCoordinates.y));

			Island island = Instantiate(IslandPrefab, islandPos, Quaternion.identity, transform);

			island.Initialize((int)islandCoordinates.x, (int)islandCoordinates.y);
		}
	}
}
