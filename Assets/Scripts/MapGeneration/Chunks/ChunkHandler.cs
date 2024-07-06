using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class ChunkHandler
{
    private int _mapSize;
    private int _chunkSize;
    private Transform _parent;
    private int _chunkGridLength;
    private Material _terrainMaterial;
    private HexGridManager _hexGridManager;
    // private Mesh _originalHexagonMesh;
    // private NoiseManager noiseManager;


    public ChunkHandler(int mapSize, int chunkSize, Transform parent, Material terrainMaterial, HexGridManager hexGridManager)
    {
        this._mapSize = mapSize;
        this._chunkSize = chunkSize;
        this._parent = parent;
        this._chunkGridLength = mapSize / chunkSize;
        this._terrainMaterial = terrainMaterial;
        this._hexGridManager = hexGridManager;
        // this._originalHexagonMesh = originalHexagonMesh;
        // this.noiseManager = noiseManager;
    }
    
    public void GenerateAllChunks()
    {
        for (int X = 0; X < _mapSize / _chunkSize; X++)
        {
            for (int Z = 0; Z < _mapSize / _chunkSize; Z++)
            {
                GenerateChunkFromChunkPosition(new Vector2Int(X, Z));
            }
        }
    }

    public Chunk GenerateChunkFromChunkPosition(Vector2Int chunkPosition){        

        GameObject chunkObject = new GameObject("Chunk " + chunkPosition.x + ", " + chunkPosition.y);
        chunkObject.transform.SetParent(_parent);

        // Vector2Int centerChunkPosition = chunkPosition;
        // centerChunkPosition.x += chunkSize/2;
        // centerChunkPosition.y += chunkSize/2;
        Vector2 chunkWorldPos = WorldPosFromChunkGrid(chunkPosition);
        Vector3 position = new Vector3(chunkWorldPos.x, HexMetrics.heightMultiplier, chunkWorldPos.y);
        // Debug.Log("Generating chunk: " + chunkPosition.x + ", " + chunkPosition.y + " at position: " + position);

        chunkObject.transform.localPosition = position;
        
        chunkObject.tag = "Chunk";

        Chunk chunk = chunkObject.AddComponent<Chunk>();
        chunk.Initialize(chunkPosition.x, chunkPosition.y, _chunkSize);

        // float[,] heightMap = noiseManager.GetChunkHeightMap(chunk);
        // float[,] moistureMap = noiseManager.GetChunkMoistureMap(chunk);
        // float[,] temperatureMap = noiseManager.GetChunkTemperatureMap(chunk);
        PopulateChunk(chunk);
        CombineMeshes(chunk);

        ApplyMaterialToChunk(chunk);

        return chunk;
    }

    private void ApplyMaterialToChunk(Chunk chunk){
        // _terrainMaterial = new Material(_terrainMaterial);
        // Texture2D heightMapTexture = TextureGenerator.TextureFromNoiseMap(heightMap);
        // Texture2D moistureMapTexture = TextureGenerator.TextureFromNoiseMap(moistureMap);
        // Texture2D temperatureMapTexture = TextureGenerator.TextureFromNoiseMap(temperatureMap);
        // _terrainMaterial.SetTexture("_HeightMap", heightMapTexture);
        // _terrainMaterial.SetTexture("_MoistureMap", moistureMapTexture);
        // _terrainMaterial.SetTexture("_TemperatureMap", temperatureMapTexture);
        MeshRenderer meshRenderer = chunk.gameObject.GetComponent<MeshRenderer>();
        meshRenderer.material = _terrainMaterial;
        // if(chunk.x == 0 && chunk.z == 0){
        //     TextureGenerator.WriteTexture(heightMapTexture, "Assets/Textures/HeightMap.jpeg");
        //     TextureGenerator.WriteTexture(moistureMapTexture, "Assets/Textures/MoistureMap.jpeg");
        //     TextureGenerator.WriteTexture(temperatureMapTexture, "Assets/Textures/TemperatureMap.jpeg");
        // }
    }

    private void CombineMeshes(Chunk chunk){
        // Debug.Log("Combining meshes for chunk: " + chunk.x + ", " + chunk.z);
        MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        // Debug.Log("meshFilters.Length: " + meshFilters.Length);
        int i = 0;
        while (i < meshFilters.Length)
        {
            if(meshFilters[i].gameObject == chunk.gameObject){
                i++;
                continue;
            }
            combine[i].mesh = meshFilters[i].sharedMesh;
            // combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix * chunk.transform.worldToLocalMatrix;
            meshFilters[i].gameObject.SetActive(false); //start with chunks disabled
            GameObject.Destroy(meshFilters[i].gameObject);
            i++;
        }
        chunk.gameObject.AddComponent<MeshFilter>();
        chunk.gameObject.AddComponent<MeshRenderer>();
        MeshFilter meshFilter = chunk.gameObject.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);

        // Add or get the MeshCollider and assign the combined mesh to it
        MeshCollider meshCollider = chunk.gameObject.GetComponent<MeshCollider>();
        if (meshCollider == null) {
            meshCollider = chunk.gameObject.AddComponent<MeshCollider>();
        }
        meshCollider.sharedMesh = chunk.gameObject.GetComponent<MeshFilter>().mesh;

        // Recalculate UVs
        // Vector2[] newUVs = new Vector2[meshFilter.mesh.vertices.Length];
        // Bounds bounds = meshFilter.mesh.bounds;
        // for (int j = 0; j < newUVs.Length; j++)
        // {
        //     Vector3 vertexPosition = meshFilter.mesh.vertices[j];
        //     newUVs[j].x = (vertexPosition.x - bounds.min.x) / (bounds.max.x - bounds.min.x);
        //     newUVs[j].y = (vertexPosition.z - bounds.min.z) / (bounds.max.z - bounds.min.z);
        // }
        // meshFilter.mesh.uv = newUVs;


        chunk.gameObject.SetActive(true);
    }

    private void PopulateChunk(Chunk chunk){
        _hexGridManager.cellFactory.GenerateChunkCells(chunk);
        // _HexGridManager.featureGenerator.GenerateChunkFeatures(chunk);
    }

    public Vector2Int? GetChunkPositionFromWorldPosition(Vector3 worldPosition)
    {
        // Transform to local position
        Vector3 localPosition = _parent.InverseTransformPoint(worldPosition);

        // Find the column and row of the cell 
        Vector3Int cellPosition = HexMetrics.WorldPositionToCellPosition(localPosition);

        // Find which chunk the cell belongs to
        int chunkCol = cellPosition.x / _chunkSize;
        int chunkRow = cellPosition.z / _chunkSize;

        // Return the chunk. 
        if (chunkCol >= 0 && chunkCol < _chunkGridLength && chunkRow >= 0 && chunkRow < _chunkGridLength)
        {
            return new Vector2Int(chunkCol, chunkRow);
        }
        Debug.Log(chunkCol + ", " + chunkRow + "this chunk is out of bounds of the map, the player should not be here");
        return null;
    }

    public List<Vector2Int> GetNeighboursChunksPositionFromChunkPosition(Vector2Int chunkPosition, int rangeChunksToDisplay){
        List<Vector2Int> neighbours = new List<Vector2Int>();

        for(int i = -rangeChunksToDisplay; i <= rangeChunksToDisplay; i++)
        {
            for(int j = -rangeChunksToDisplay; j <= rangeChunksToDisplay; j++)
            {
                int ChunkToCheckCoord_X = chunkPosition.x + i;
                int ChunkToCheckCoord_Z = chunkPosition.y + j; // in the chunk grid, the z axis is the y axis in the world
                
                if(ChunkToCheckCoord_X >= 0 && ChunkToCheckCoord_X < _chunkGridLength &&
                   ChunkToCheckCoord_Z >= 0 && ChunkToCheckCoord_Z < _chunkGridLength)
                {
                    neighbours.Add(new Vector2Int(ChunkToCheckCoord_X, ChunkToCheckCoord_Z));
                }
            }
        }
        return neighbours;
    }


    private Vector2Int ChunkGridFromWorldPos(Vector2Int position){
        Vector3Int cellPos = HexMetrics.WorldPositionToCellPosition(new Vector3(position.x, 0, position.y));
        return new Vector2Int(cellPos.x / _chunkSize, cellPos.z / _chunkSize);
    }

    private Vector2 WorldPosFromChunkGrid(Vector2Int position){
        Vector3 chunkWorldPos = HexMetrics.CellPositionToWorldPosition(new Vector3Int(position.x * _chunkSize, 0, position.y * _chunkSize));
        return new Vector2(chunkWorldPos.x, chunkWorldPos.z);
    }

}
