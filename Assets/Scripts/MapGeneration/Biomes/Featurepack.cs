using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Terrain/FeaturePack")]
public class FeaturePack : ScriptableObject
{
    public List<GameObject> prefabs;
    public Material material;
    public DataMapSettings noiseMapSettings;
    // public ShaderParameters shaderParameters = new ShaderParameters();
}


[System.Serializable]
public class ShaderParameters
{
    //Should be between 0 and 2
    public float SnowOpacity = 1;

    //Should be between 0 and 1
    public float Height = 0;

    public Color BaseColor;
    public Color SnowColor = Color.white;
}
