using System.Collections.Generic;
using UnityEngine;
using Biome;

public class HexMeshGrid : MonoBehaviour
{
	private int _mapSize = HexMetrics.MapSize; // size of the map in chunks

	private int _chunkSize = HexMetrics.ChunkSize;

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

		// HexChunk chunk = Instantiate(_hexMesh, new Vector3(0,0,0), Quaternion.identity, transform);
		// chunk.Initialize(0, 0, _chunkSize, _mapSize, heightMap.Values);
		// chunk.GenerateMesh();


		for (int chunkZ = 0; chunkZ < _mapSize; chunkZ++)
		{
			for (int chunkX = 0; chunkX < _mapSize; chunkX++)
			{
				// Vector3[] chunkCellsPositions = new Vector3[_chunkSize * _chunkSize];
				// Vector3 chunWorldPos = new Vector3(chunkWorldPosX, 0, chunkWorldPosZ);
				Vector3 chunkHexPos = HexMetrics.HexChunkToWorld(new HexChunkCoordinates(chunkX,chunkZ));

				HexChunk chunk = Instantiate(_hexMesh, chunkHexPos, Quaternion.identity, transform);
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
				ApplyMaterial(chunk);
			}
		}
	}

	void ApplyMaterial(HexChunk chunkMesh)
	{
		chunkMesh.ApplyMaterial(_terrainMaterial);
	}
}
