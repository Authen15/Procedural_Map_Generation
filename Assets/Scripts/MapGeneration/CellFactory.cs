using UnityEngine;

public class CellFactory 
{
    private GameObject cellPrefab;
    private Transform parentTransform;
    private CellBlur cellBlur;

    public CellFactory(GameObject cellPrefab, Transform parentTransform)
    {
        this.cellPrefab = cellPrefab;
        this.parentTransform = parentTransform;
    }

    public void CreateAllCells(DataMapManager dataMapManager, BiomeHandling biomeHandling, HexMapData hexMapData) 
    {
        for (int z = 0; z < hexMapData.size; z++) 
        {
            for (int x = 0; x < hexMapData.size; x++) 
            {
                float height = dataMapManager.RoundToNearestHeightStep(dataMapManager.heightMap.values[z, x], hexMapData.nbHeightSteps);
                float moisture = dataMapManager.RoundToNearestHeightStep(dataMapManager.moistureMap.values[z, x], hexMapData.nbHeightSteps);
                float temperature = dataMapManager.RoundToNearestHeightStep(dataMapManager.temperatureMap.values[z, x], hexMapData.nbHeightSteps);

                GameObject cellObj = InstantiateCell(x, height, z);
                if (cellObj != null) 
                {
                    
                    HexCell cell = cellObj.AddComponent<HexCell>();
                    cell.Initialize(x, z);

                    cell.CellObject = cellObj;

                    cell.Biome = biomeHandling.GetBiome(height, moisture, temperature);
                
                    MeshRenderer meshRenderer = cellObj.GetComponent<MeshRenderer>();

                    BiomeLevel level = biomeHandling.GetBiomeLevel(cell.Biome, height);
                    cell.BiomeLevel = level;
                     
                    meshRenderer.material = level.material;
                    hexMapData.SetCell(x, z, cell);
                }
            }
        }
        // Blur the cells
        cellBlur = new CellBlur(hexMapData);
        cellBlur.BlurAllCells(hexMapData, 2);
    }

       


    private GameObject InstantiateCell(int x, float y, int z) 
    {
        Vector3 position = CalculatePosition(x, y, z);
        // Vector3 scale = new Vector3(1, y * HexMetrics.heightMultiplier, 1);  //TODO Check if it's better to modify scale


        GameObject cell = Object.Instantiate(cellPrefab);
        cell.transform.SetParent(parentTransform, false);
        cell.transform.localPosition = position;
        // cell.transform.localScale = scale;

        return cell;
    }

    private Vector3 CalculatePosition(int x, float y, int z) 
    {
        float posX = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
        float posY = y * HexMetrics.heightMultiplier / 2; 
        float posZ = z * (HexMetrics.outerRadius * 1.5f);

        return new Vector3(posX, posY, posZ);
    }
}
