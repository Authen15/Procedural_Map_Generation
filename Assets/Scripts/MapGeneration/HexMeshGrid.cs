using System.Collections.Generic;
using UnityEngine;
using Biome;

public class HexMeshGrid : MonoBehaviour
{
	private int _mapSize = HexMetrics.MapSize; // size of the map in chunks
	[SerializeField]
	private int _chunkSize = 32;

	private HexChunk _hexMesh;

	[SerializeField]
	private HeightMapSettings _heightMapSettings;
	[SerializeField]
	private MoistureMapSettings _moistureMapSettings;
	[SerializeField]
	private TemperatureMapSettings _temperatureMapSettings;
	
	[SerializeField]
	private Material _terrainMaterial;

	[SerializeField]
	private BiomeManager _biomeManager;

	private NoiseManager _noiseManager;


	void Start()
	{
		_hexMesh = GetComponentInChildren<HexChunk>();
		_noiseManager = new NoiseManager(_heightMapSettings, _moistureMapSettings, _temperatureMapSettings, _biomeManager);

		DataMap heightMap = _noiseManager.GetFullHeightDataMap();
		DataMap moistureMap = _noiseManager.GetFullMoistureDataMap();
		DataMap temperatureMap = _noiseManager.GetFullTemperatureDataMap();
		// DataMap featureMap = _noiseManager.GetFullFeatureDataMap(featureMap, _grassMapSettings);

		Texture2D heightMapTexture = TextureGenerator.TextureFromNoiseMap(heightMap.Values);
		heightMapTexture.filterMode = FilterMode.Trilinear;
		Texture2D moistureMapTexture = TextureGenerator.TextureFromNoiseMap(moistureMap.Values);
		moistureMapTexture.filterMode = FilterMode.Trilinear;
		Texture2D temperatureMapTexture = TextureGenerator.TextureFromNoiseMap(temperatureMap.Values);
		temperatureMapTexture.filterMode = FilterMode.Trilinear;

		SetMaterialTextures(heightMapTexture, moistureMapTexture, temperatureMapTexture);
		SetMaterialProperties();

		GenerateMap(heightMap, moistureMap);
	}

	void SetMaterialTextures(Texture2D heightMapTexture, Texture2D moistureMapTexture, Texture2D temperatureMapTexture)
	{
		_terrainMaterial.SetTexture("_HeightMap", heightMapTexture);
		_terrainMaterial.SetTexture("_MoistureMap", moistureMapTexture);
		_terrainMaterial.SetTexture("_TemperatureMap", temperatureMapTexture);
	}

	void SetMaterialProperties()
	{
		_terrainMaterial.SetFloat("_MapSize", _mapSize);
		_terrainMaterial.SetFloat("_InnerRadius", HexMetrics.innerRadius);
		_terrainMaterial.SetFloat("_OuterRadius", HexMetrics.outerRadius);
		_terrainMaterial.SetFloat("_MinHeight", 0);
		_terrainMaterial.SetFloat("_MaxHeight", HexMetrics.heightMultiplier);
	}

	void GenerateMap(DataMap heightMap, DataMap moistureMap)
	{
		for (int chunkZ = 0; chunkZ < _mapSize / _chunkSize; chunkZ++)
		{
			for (int chunkX = 0; chunkX < _mapSize / _chunkSize; chunkX++)
			{
				// Vector3[] chunkCellsPositions = new Vector3[_chunkSize * _chunkSize];
				float chunkWorldPosX = chunkX * _chunkSize * HexMetrics.innerRadius * 2;
				float chunkWorldPosZ = chunkZ * _chunkSize *  HexMetrics.outerRadius * 1.5f;
				Vector3 chunWorldPos = new Vector3(chunkWorldPosX, 0, chunkWorldPosZ);


				HexChunk chunk = Instantiate(_hexMesh, chunWorldPos, Quaternion.identity, transform);
				// for (int z = 0; z < _chunkSize; z++)
				// {
				// 	for (int x = 0; x < _chunkSize; x++)
				// 	{
				// 		int xSample = x + chunkX * _chunkSize;
				// 		int zSample = z + chunkZ * _chunkSize;
				// 		float y = heightMap.values[xSample, zSample];
				// 		chunkCellsPositions[z * _chunkSize + x] = HexMetrics.CellGridToWorldPos(xSample, y, zSample);
				// 	}
				// }
				chunk.Initialize(chunkX, chunkZ, _chunkSize, _mapSize, heightMap.Values);
				chunk.GenerateMesh();
				ApplyMaterial(chunk, chunkX, chunkZ, heightMap, moistureMap);
			}
		}
	}

	void ApplyMaterial(HexChunk chunkMesh, int chunkX, int chunkZ, DataMap heightMap, DataMap moistureMap)
	{
		float[,] grassVisibilityMap = _noiseManager.GetChunkVisibilityNoiseMap(heightMap, moistureMap, new Vector2Int(chunkX, chunkZ), _chunkSize);
		Texture2D grassVisibilityTexture = TextureGenerator.TextureFromNoiseMap(grassVisibilityMap);
		grassVisibilityTexture.filterMode = FilterMode.Trilinear;
		// chunkMesh.ApplyMaterial(_terrainMaterial, _grassMaterial, grassVisibilityTexture);
		chunkMesh.ApplyMaterial(_terrainMaterial, null, grassVisibilityTexture);
	}
}
