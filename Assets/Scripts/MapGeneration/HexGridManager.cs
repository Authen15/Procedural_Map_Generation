using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;




public class HexGridManager : MonoBehaviour 
{
    public int mapSize = 64;
    public int chunkSize = 8;
    
    public GameObject cellPrefab;
    // public FeatureGenerator featureGenerator;

    public HeightMapSettings HeightMapSettings;
    public MoistureMapSettings MoistureMapSettings;
    public TemperatureMapSettings TemperatureMapSettings;
    public List<DataMapSettings> allFeatureMapSettings;

    public CellFactory cellFactory;
    public NoiseManager noiseManager;
    public ChunkHandler chunkHandler;

    public BiomeHandler biomeHandler;
    public Biome defaultBiome;  // Drag and drop in the inspector
    public Biome[] allBiomes;   // Drag and drop in the inspector

    public Material terrainMaterial;


    void Awake() 
    {
        InitializeGridManager();
    }


    public void InitializeGridManager()
    {
        // Ensure data maps are initialized
        InitializeDataMaps();

        DataMap heightMap = new DataMap(new float[mapSize, mapSize]);
        DataMap moistureMap = new DataMap(new float[mapSize, mapSize]);
        DataMap temperatureMap = new DataMap(new float[mapSize, mapSize]);

        // DataMap textureHeightDataMap = new DataMap(new float[mapSize*4, mapSize*4]);
        // DataMap textureMoistureDataMap = new DataMap(new float[mapSize*4, mapSize*4]);
        // DataMap textureTemperatureDataMap = new DataMap(new float[mapSize*4, mapSize*4]);


        noiseManager.GetFullHeightDataMap(heightMap);
        noiseManager.GetFullMoistureDataMap(moistureMap);
        noiseManager.GetFullTemperatureDataMap(temperatureMap);

        Texture2D heightMapTexture = TextureGenerator.TextureFromNoiseMap(heightMap.values);
        heightMapTexture.filterMode = FilterMode.Trilinear;
        Texture2D moistureMapTexture = TextureGenerator.TextureFromNoiseMap(moistureMap.values);
        moistureMapTexture.filterMode = FilterMode.Trilinear;
        Texture2D temperatureMapTexture = TextureGenerator.TextureFromNoiseMap(temperatureMap.values);
        temperatureMapTexture.filterMode = FilterMode.Trilinear;

        terrainMaterial.SetTexture("_HeightMap", heightMapTexture);
        terrainMaterial.SetTexture("_MoistureMap", moistureMapTexture);
        terrainMaterial.SetTexture("_TemperatureMap", temperatureMapTexture);

        TextureGenerator.WriteTexture(heightMapTexture, "Assets/Textures/HeightMap.jpeg");

        terrainMaterial.SetFloat("_MapSize", mapSize);
        terrainMaterial.SetFloat("_InnerRadius", HexMetrics.innerRadius);
        terrainMaterial.SetFloat("_OuterRadius", HexMetrics.outerRadius);
        terrainMaterial.SetFloat("_MinHeight", HexMetrics.heightMultiplier);
        terrainMaterial.SetFloat("_MaxHeight", HexMetrics.heightMultiplier * 1.5f);// time 1.5 seems to be a good value


        

        //must generate chunks before creating cells
        chunkHandler = new ChunkHandler(mapSize, chunkSize, transform, terrainMaterial, this);
        // chunkHandler.GenerateAllChunks();

        cellFactory = new CellFactory(cellPrefab, biomeHandler, heightMap.values, moistureMap.values, temperatureMap.values);
        // cellFactory.GenerateAllChunksCells();

        // featureGenerator = new FeatureGenerator(noiseManager);
        // featureGenerator.GenerateFeatures();

        chunkHandler.GenerateAllChunks();


    }


    public void InitializeDataMaps() 
    {
        // generate the DataMaps for Features

        // Texture2D atlas = TextureGenerator.GenerateAtlasTexture(allBiomes, 64, 64);
        // Texture2D blurredAtlas = TextureGenerator.ApplyGaussianBlur(atlas, 2);
        // blurredAtlas = TextureGenerator.ApplyGaussianBlur(blurredAtlas, 3);
        // // atlas = blurredAtlas;
        // TextureGenerator.WriteTexture(atlas, "Assets/Textures/Atlas.jpeg");
        // terrainMaterial.SetTexture("_Atlas", blurredAtlas);
        // TextureGenerator.WriteTexture(blurredAtlas, "Assets/Textures/BlurredAtlas.jpeg");





        
        // Debug.Log("Atlas generated");
        noiseManager = new NoiseManager(HeightMapSettings, MoistureMapSettings, TemperatureMapSettings, allFeatureMapSettings, mapSize);




        
        // terrainMaterial.SetFloat("_TemperatureMax", temperatureMap.maxValue);
        // terrainMaterial.SetFloat("_TemperatureMin", temperatureMap.minValue);
        // terrainMaterial.SetFloat("_MoistureMax", moistureMap.maxValue);
        // terrainMaterial.SetFloat("_MoistureMin", moistureMap.minValue);

        // TextureGenerator.WriteTexture(heightMapTexture, "Assets/Textures/HeightMap.jpeg");
        // TextureGenerator.WriteTexture(moistureMapTexture, "Assets/Textures/MoistureMap.jpeg");
        // TextureGenerator.WriteTexture(temperatureMapTexture, "Assets/Textures/TemperatureMap.jpeg");







        // noiseManager.Debugging();
        biomeHandler = new BiomeHandler(allBiomes, defaultBiome);
    }
}
