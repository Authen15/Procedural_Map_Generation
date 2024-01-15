using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/HeightMapSettings")]
public class HeightMapSettings : DataMapSettings 
{

    [Space(10)]
    public float baseHeight;

    public AnimationCurve heightCurve = AnimationCurve.Linear(0, 0, 1, 1);

	#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endif

}