using System;
using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/HeightMapSettings")]
public class HeightMapSettings : DataMapSettings 
{
    public bool UseFallOff;
    public float FallOffMapSteepness = 3;
    public float FallOffMapOffset = 2.2f;
    public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);

    public Action UpdateIslandMesh;

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
        UpdateIslandMesh?.Invoke();
    }
#endif
}