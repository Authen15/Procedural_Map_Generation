using Unity.Mathematics;
using UnityEngine;

public class CellFactory 
{
    private GameObject _cellPrefab;
    // private CellBlur cellBlur;

    private BiomeHandler _biomeHandler;
    private float[,] _heightMap;
    private float[,] _moistureMap;
    private float[,] _temperatureMap;

    public CellFactory(GameObject cellPrefab, BiomeHandler biomeHandler, float[,] heightMap, float[,] moistureMap, float[,] temperatureMap)
    {
        this._biomeHandler = biomeHandler;
        this._cellPrefab = cellPrefab;
        this._heightMap = heightMap;
        this._moistureMap = moistureMap;
        this._temperatureMap = temperatureMap;
    }

    public void GenerateChunkCells(Chunk chunk){
        if(chunk == null){
            Debug.Log("Chunk is null");
            return;
        }

        for (int i = 0; i < chunk.size; i++)
        {
            for (int j = 0; j < chunk.size; j++)
            {
                int offsetX = chunk.x * chunk.size;
                int offsetY = chunk.z * chunk.size;
                float height = NoiseUtils.RoundToNearestHeightStep(_heightMap[i + offsetX, j + offsetY], HexMetrics.nbHeightSteps);
                float moisture = NoiseUtils.RoundToNearestHeightStep(_moistureMap[i + offsetX, j + offsetY], HexMetrics.nbHeightSteps);
                float temperature = NoiseUtils.RoundToNearestHeightStep(_temperatureMap[i + offsetX, j + offsetY], HexMetrics.nbHeightSteps);
                // float height = NoiseUtils.RoundToNearestHeightStep(heightMap[i,j], HexMetrics.nbHeightSteps);
                // float moisture = NoiseUtils.RoundToNearestHeightStep(moistureMap[i,j], HexMetrics.nbHeightSteps);
                // float temperature = NoiseUtils.RoundToNearestHeightStep(temperatureMap[i,j], HexMetrics.nbHeightSteps);
                
                // Debug.Log("Height: " + height + " Moisture: " + moisture + " Temperature: " + temperature);
                GameObject cellObj = InstantiateCell(new Vector3(i, height, j), chunk);
                if (cellObj != null)
                {
                    HexCell cell = cellObj.AddComponent<HexCell>();
                    cell.Initialize(i, j);

                    cell.CellObject = cellObj;
                    cell.Biome = _biomeHandler.GetBiome(moisture, temperature);

                    // AdjustUVsForCell(cellObj, temperature, moisture);

                    // MeshRenderer meshRenderer = cellObj.GetComponent<MeshRenderer>();
                    // BiomeLevel level = biomeHandler.GetBiomeLevel(cell.Biome, height);
                    // cell.BiomeLevel = level;

                    // meshRenderer.material = terainMaterial;

                    chunk.cells[i, j] = cell;
                }
            }
        }
    }

    
	// public void BlurCells(HexMapData hexMapData)
    // {
	// 	cellBlur = new CellBlur(hexMapData);
	// 	cellBlur.BlurAllCells(hexMapData, 2);
	// }

    private GameObject InstantiateCell(Vector3 position, Chunk chunk) 
    {
        Vector3 cellPos = HexMetrics.CellPositionToWorldPosition(position);
        // Vector3 scale = new Vector3(1, y * HexMetrics.heightMultiplier, 1);  //TODO Check if it's better to modify scale


        GameObject cell = Object.Instantiate(_cellPrefab);
        cell.transform.SetParent(chunk.gameObject.transform, false);
        cell.transform.localPosition = cellPos;
        // cell.transform.localScale = scale;

        // Debug.Log(position);

        return cell;
    }

    private void AdjustUVsForCell(GameObject cellObj, float temperature, float humidity)
    {
        MeshFilter meshFilter = cellObj.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Vector2[] uvs = meshFilter.mesh.uv;

            for (int i = 0; i < uvs.Length; i++)
            {
                uvs[i] = new Vector2(temperature, humidity);
            }

            meshFilter.mesh.uv = uvs;
        }
    }


    private void Debugging(NoiseManager noiseManager){
        noiseManager.Debugging();
    }
}
