using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/BiomeLevel")]
public class BiomeLevel : ScriptableObject
{
    
    public Color color;
    public float minHeight; // The minimum noise value for this level
    public float maxHeight; // The maximum noise value for this level

    public bool isWater;
}
