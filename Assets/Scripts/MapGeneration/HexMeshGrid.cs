using UnityEngine;

public class HexMeshGrid : MonoBehaviour
{
	private static HexMeshGrid _instance;
	public static HexMeshGrid Instance{
		get{
			if (_instance == null){
				Debug.LogWarning("HexMeshGrid is null");
			}
			return _instance;
		}
	}


	public int GlobalSeed = 0;

	private HexIsland _hexMesh;
	
	void Awake(){
		_instance = this;
	}
	
	

	void Start()
	{
		_hexMesh = GetComponentInChildren<HexIsland>();

		float startTime = Time.realtimeSinceStartup;
		GenerateMap();
		Debug.Log("Generating time " + (Time.realtimeSinceStartup - startTime).ToString(".0###########"));
	}

	void GenerateMap()
	{
		Vector2[] chunksPositions = HexGridUtils.MapChunksPositions;

		foreach (Vector2 chunkCoordinates in chunksPositions )
		{
			// convert to vector3 for world position
			Vector3 chunkHexPos = HexGridUtils.HexChunkToWorld(new Vector2((int)chunkCoordinates.x, (int)chunkCoordinates.y));

			HexIsland chunk = Instantiate(_hexMesh, chunkHexPos, Quaternion.identity, transform);

			chunk.Initialize((int)chunkCoordinates.x, (int)chunkCoordinates.y);
			chunk.GenerateMesh();
		}
	}
}
