using System.Collections.Generic;
using UnityEngine;

public class HexGridManager : MonoBehaviour 
{
    public int size = 64;
    public int nbHeightSteps = 32;
    public int chunkSize = 8;
    
    public GameObject cellPrefab;
    public FeatureGenerator featureGenerator;

    public HeightMapSettings HeightMapSettings;
    public HeightMapSettings MoistureMapSettings;
    public TemperatureMapSettings TemperatureMapSettings;
    public List<DataMapSettings> allFeatureMapSettings;

    private CellFactory cellFactory;
    public DataMapManager dataMapManager;
    public HexMapData hexMapData;
    public ChunkHandler chunkHandler;

    private BiomeHandling biomeHandling;
    public Biome defaultBiome;  // Drag and drop in the inspector
    public Biome[] allBiomes;   // Drag and drop in the inspector


    void Awake() 
    {
        InitializeGridManager();
    }


    public void InitializeGridManager() 
    {
        // Ensure data maps are initialized
        InitializeDataMaps();

        cellFactory = new CellFactory(cellPrefab, transform);

        cellFactory.CreateAllCells(dataMapManager, biomeHandling, hexMapData);

        chunkHandler = new ChunkHandler(hexMapData, chunkSize, transform);
        chunkHandler.GenerateAllChunks();


        featureGenerator = new FeatureGenerator(dataMapManager.featureMaps, hexMapData);
        featureGenerator.GenerateFeatures();
    }


    public void InitializeDataMaps() 
    {
        // generate the DataMaps for Features


        dataMapManager = new DataMapManager(HeightMapSettings, MoistureMapSettings, TemperatureMapSettings, allFeatureMapSettings, size);
        biomeHandling = new BiomeHandling(allBiomes, defaultBiome);
        hexMapData = new HexMapData(size, nbHeightSteps, chunkSize);

        dataMapManager.GenerateDataMaps();
    }
}
