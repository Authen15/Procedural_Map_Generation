using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/HeightMapSettings")]
public class HeightMapSettings : DataMapSettings
{
    public float FallOffMapSteepness = 2;
    public float FallOffMapOffset = 10f;
    [Range(0, 2)] public float Multiplier = 1;
    public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Action UpdateIslandMesh;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateIslandMesh?.Invoke();
    }
#endif

    public float GetMaximumHeight()
    {
        return HeightCurve.keys[HeightCurve.length - 1].value;
    }
}