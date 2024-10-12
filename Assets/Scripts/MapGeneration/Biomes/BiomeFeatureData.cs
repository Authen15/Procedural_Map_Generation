using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeFeatureData
{
    public FeaturePack featurePack;
    public Vector3 scaleMultiplier = Vector3.one;
    [Range(0f, 1f)]
    public float probability = 1; // from 0 to 1
    public float featureThresholdMin = 0.0f;
    public float featureThresholdMax = 1.0f;
    public ShaderParameters shaderParameters = new ShaderParameters();
    public bool hasSnow = false;
    public bool isAffectedByBiomeColor = false; 
}