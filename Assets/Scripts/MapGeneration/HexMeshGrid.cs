using UnityEngine;
using Biome;

public class HexMeshGrid : MonoBehaviour
{
	private HexChunk _hexMesh;

	[SerializeField]
	private HeightMapSettings _heightMapSettings;
	// [SerializeField]
	// private MoistureMapSettings _moistureMapSettings;
	// [SerializeField]
	// private TemperatureMapSettings _temperatureMapSettings;
	
	[SerializeField]
	private Material _terrainMaterial;

	[SerializeField]
	private BiomeManager _biomeManager;

	private NoiseManager _noiseManager;

	[SerializeField]
	private Camera TMP_CAMERA;


	void Start()
	{
		Vector3 camPos = HexGridUtils.HexChunkToWorld(new Vector3(HexMetrics.MapSize/2, 500, HexMetrics.MapSize/2));
		TMP_CAMERA.transform.position = camPos;


		_hexMesh = GetComponentInChildren<HexChunk>();
		// _noiseManager = new NoiseManager(_heightMapSettings, _moistureMapSettings, _temperatureMapSettings, _biomeManager);

		// DataMap heightMap = _noiseManager.GetFullHeightDataMap();
		// DataMap moistureMap = _noiseManager.GetFullMoistureDataMap();
		// DataMap temperatureMap = _noiseManager.GetFullTemperatureDataMap();
		// DataMap featureMap = _noiseManager.GetFullFeatureDataMap(featureMap, _grassMapSettings);

		// Texture2D heightMapTexture = TextureGenerator.TextureFromNoiseMap(heightMap.Values);
		// heightMapTexture.filterMode = FilterMode.Trilinear;
		// Texture2D moistureMapTexture = TextureGenerator.TextureFromNoiseMap(moistureMap.Values);
		// moistureMapTexture.filterMode = FilterMode.Trilinear;
		// Texture2D temperatureMapTexture = TextureGenerator.TextureFromNoiseMap(temperatureMap.Values);
		// temperatureMapTexture.filterMode = FilterMode.Trilinear;

		// SetMaterialTextures(heightMapTexture, moistureMapTexture, temperatureMapTexture);
		// SetMaterialProperties();

		GenerateMap();
	}

	// void SetMaterialTextures(Texture2D heightMapTexture, Texture2D moistureMapTexture, Texture2D temperatureMapTexture)
	// {
	// 	_terrainMaterial.SetTexture("_HeightMap", heightMapTexture);
	// 	_terrainMaterial.SetTexture("_MoistureMap", moistureMapTexture);
	// 	_terrainMaterial.SetTexture("_TemperatureMap", temperatureMapTexture);
	// }

	// void SetMaterialProperties()
	// {
	// 	_terrainMaterial.SetFloat("_MapSize", _mapSize);
	// 	_terrainMaterial.SetFloat("_InnerRadius", HexMetrics.InnerRadius);
	// 	_terrainMaterial.SetFloat("_OuterRadius", HexMetrics.OuterRadius);
	// 	_terrainMaterial.SetFloat("_MinHeight", 0);
	// 	_terrainMaterial.SetFloat("_MaxHeight", HexMetrics.HeightMultiplier);
	// }

	void GenerateMap()
	{
		// Vector3[] chunksPositions = HexGridUtils.GenerateCellsPositions(_mapSize);
		// Vector3[] chunksPositions = HexGridUtils.GetMapChunksPositions();
		Vector3[] chunksPositions = HexGridUtils.MapChunksPositions;

		foreach (Vector3 chunkCoordinates in chunksPositions )
		{
			
			Vector3 chunkHexPos = HexGridUtils.HexChunkToWorld(new Vector3((int)chunkCoordinates.x, 0, (int)chunkCoordinates.z));

			HexChunk chunk = Instantiate(_hexMesh, chunkHexPos, Quaternion.identity, transform);

			chunk.Initialize((int)chunkCoordinates.x, (int)chunkCoordinates.z, HexMetrics.ChunkSize, _heightMapSettings);
			chunk.GenerateMesh();
			ApplyMaterial(chunk);
		}
	}

	void ApplyMaterial(HexChunk chunkMesh)
	{
		chunkMesh.ApplyMaterial(_terrainMaterial);
	}
}
