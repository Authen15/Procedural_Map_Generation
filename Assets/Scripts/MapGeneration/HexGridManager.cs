using System.Collections.Generic;
using UnityEngine;




public class HexGridManager : MonoBehaviour 
{
    public int mapSize = 64;
    public int chunkSize = 8;
    
    public GameObject cellPrefab;
    public FeatureGenerator featureGenerator;

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

        //must generate chunks before creating cells
        chunkHandler = new ChunkHandler(mapSize, chunkSize, transform, terrainMaterial, this);
        // chunkHandler.GenerateAllChunks();

        cellFactory = new CellFactory(cellPrefab, noiseManager, biomeHandler);
        // cellFactory.GenerateAllChunksCells();



        featureGenerator = new FeatureGenerator(noiseManager);
        // featureGenerator.GenerateFeatures();
    }


    public void InitializeDataMaps() 
    {
        // generate the DataMaps for Features

        Texture2D atlas = TextureGenerator.GenerateAtlasTexture(allBiomes, 64, 64);
        Texture2D blurredAtlas = TextureGenerator.ApplyGaussianBlur(atlas, 2);
        blurredAtlas = TextureGenerator.ApplyGaussianBlur(blurredAtlas, 3);
        // atlas = blurredAtlas;
        TextureGenerator.WriteTexture(atlas, "Assets/Textures/Atlas.jpeg");
        terrainMaterial.SetTexture("_Atlas", blurredAtlas);
        TextureGenerator.WriteTexture(blurredAtlas, "Assets/Textures/BlurredAtlas.jpeg");


        
        Debug.Log("Atlas generated");
        noiseManager = new NoiseManager(HeightMapSettings, MoistureMapSettings, TemperatureMapSettings, allFeatureMapSettings, mapSize);
        biomeHandler = new BiomeHandler(allBiomes, defaultBiome);
    }
}
