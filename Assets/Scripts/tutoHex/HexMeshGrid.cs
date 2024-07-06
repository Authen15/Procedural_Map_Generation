using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexGridTuto : MonoBehaviour
{
	[SerializeField]
	private int _mapSize = HexMetrics.MapSize; // size of the map in chunks
	[SerializeField]
	private int _chunkSize = 32;

	private HexMesh _hexMesh;

	[SerializeField]
	private HeightMapSettings _heightMapSettings;
	[SerializeField]
	private MoistureMapSettings _moistureMapSettings;
	[SerializeField]
	private TemperatureMapSettings _temperatureMapSettings;
	[SerializeField]
	// private DataMapSettings _grassMapSettings;
	// private List<DataMapSettings> _featureMapSettings;
	// [SerializeField]
	private Material _terrainMaterial;
	// [SerializeField]
	// private Material _grassMaterial;

	private NoiseManager _noiseManager;
	// private BiomeHandler _biomeHandler;
	// public Biome defaultBiome;
	// public Biome[] allBiomes;

	void Start()
	{
		_hexMesh = GetComponentInChildren<HexMesh>();
		_noiseManager = new NoiseManager(_heightMapSettings, _moistureMapSettings, _temperatureMapSettings, null, _mapSize);

		DataMap heightMap = new DataMap(new float[_mapSize, _mapSize]);
		DataMap moistureMap = new DataMap(new float[_mapSize, _mapSize]);
		DataMap temperatureMap = new DataMap(new float[_mapSize, _mapSize]);
		DataMap featureMap = new DataMap(new float[_mapSize, _mapSize]);


		_noiseManager.GetFullHeightDataMap(heightMap);
		_noiseManager.GetFullMoistureDataMap(moistureMap);
		_noiseManager.GetFullTemperatureDataMap(temperatureMap);
		// _noiseManager.GetFullFeatureDataMap(featureMap, _grassMapSettings);

		Texture2D heightMapTexture = TextureGenerator.TextureFromNoiseMap(heightMap.values);
		heightMapTexture.filterMode = FilterMode.Trilinear;
		Texture2D moistureMapTexture = TextureGenerator.TextureFromNoiseMap(moistureMap.values);
		moistureMapTexture.filterMode = FilterMode.Trilinear;
		Texture2D temperatureMapTexture = TextureGenerator.TextureFromNoiseMap(temperatureMap.values);
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
				Vector3[] chunkCellsPositions = new Vector3[_chunkSize * _chunkSize];
				HexMesh chunk = Instantiate(_hexMesh);
				for (int z = 0; z < _chunkSize; z++)
				{
					for (int x = 0; x < _chunkSize; x++)
					{
						int xSample = x + chunkX * _chunkSize;
						int zSample = z + chunkZ * _chunkSize;
						float y = heightMap.values[xSample, zSample];
						chunkCellsPositions[z * _chunkSize + x] = CreateCellPosition(xSample, y, zSample);
					}
				}
				chunk.Triangulate(chunkCellsPositions, _chunkSize, chunkX, chunkZ, _mapSize, heightMap.values);
				ApplyMaterial(chunk, chunkX, chunkZ, heightMap, moistureMap);
			}
		}
	}

	void ApplyMaterial(HexMesh chunkMesh, int chunkX, int chunkZ, DataMap heightMap, DataMap moistureMap)
	{
		float[,] grassVisibilityMap = _noiseManager.GetChunkVisibilityNoiseMap(heightMap, moistureMap, new Vector2Int(chunkX, chunkZ), _chunkSize);
		Texture2D grassVisibilityTexture = TextureGenerator.TextureFromNoiseMap(grassVisibilityMap);
		grassVisibilityTexture.filterMode = FilterMode.Trilinear;
		// chunkMesh.ApplyMaterial(_terrainMaterial, _grassMaterial, grassVisibilityTexture);
		chunkMesh.ApplyMaterial(_terrainMaterial, null, grassVisibilityTexture);
	}

	Vector3 CreateCellPosition(int x, float y, int z)
	{
		Vector3 position;
		position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
		position.y = y * HexMetrics.heightMultiplier;
		position.z = z * (HexMetrics.outerRadius * 1.5f);
		return position;
	}
}
