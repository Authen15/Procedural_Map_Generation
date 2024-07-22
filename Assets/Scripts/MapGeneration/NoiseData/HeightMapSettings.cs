using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/HeightMapSettings")]
public class HeightMapSettings : DataMapSettings 
{
    public float BaseHeight;

    public float FallOffMapSteepness = 3;
    public float FallOffMapOffset = 2.2f;
    public AnimationCurve HeightCurve = AnimationCurve.Linear(0, 0, 1, 1);
}