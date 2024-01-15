using System.IO.Compression;
using UnityEngine;
using UnityEngine.Diagnostics;

public class CellFactory 
{
    private GameObject cellPrefab;
    // private CellBlur cellBlur;

    private NoiseManager noiseManager;
    private BiomeHandler biomeHandler;

    public CellFactory(GameObject cellPrefab, NoiseManager noiseManager, BiomeHandler biomeHandler)
    {
        this.noiseManager = noiseManager;
        this.biomeHandler = biomeHandler;
        this.cellPrefab = cellPrefab;
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
                int absoluteX = chunk.x * chunk.size + i;
                int absoluteZ = chunk.z * chunk.size + j;

                float height = NoiseUtils.RoundToNearestHeightStep(noiseManager.GetHeightNoiseValue(absoluteX, absoluteZ), HexMetrics.nbHeightSteps);
                float moisture = NoiseUtils.RoundToNearestHeightStep(noiseManager.GetMoistureNoiseValue(absoluteX, absoluteZ), HexMetrics.nbHeightSteps);
                float temperature = NoiseUtils.RoundToNearestHeightStep(noiseManager.GetTemperatureNoiseValue(absoluteX, absoluteZ), HexMetrics.nbHeightSteps);

                GameObject cellObj = InstantiateCell(new Vector3(i, height, j), chunk);
                if (cellObj != null)
                {
                    HexCell cell = cellObj.AddComponent<HexCell>();
                    cell.Initialize(absoluteX, absoluteZ);

                    cell.CellObject = cellObj;
                    cell.Biome = biomeHandler.GetBiome(moisture, temperature);

                    AdjustUVsForCell(cellObj, temperature, moisture);

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


        GameObject cell = Object.Instantiate(cellPrefab);
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
