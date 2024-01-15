using UnityEngine;

public class HexCell : MonoBehaviour 
{
    public int X { get; set; }
    public int Z { get; set; }

    public GameObject CellObject { get; set; }
    public Biome Biome { get; set; }
    public BiomeLevel BiomeLevel { get; set; }
    public Chunk Chunk { get; set; }

    public void Initialize(int x, int z) 
    {
        X = x;
        Z = z;
    }

    public Transform GetPlaceHolderTransform()
    {
        return CellObject.transform.Find("PlaceHolder");
    }
}