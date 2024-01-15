using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/TemperatureMapSettings")]
public class TemperatureMapSettings : DataMapSettings
{
    [Space(10)]
    [Range(0f, 5f)]
    public float latitudeSensitivity = 1.2f;

    [Range(0f, 5f)]
    public float elevationSensitivity = 0.5f;

    [Range(-1f, 1f)]
    public float baseTemperature = 0.0f;

    [Range(-1f, 1f)]
    public float equatorBias = 0f;
    
    #if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endif

}
