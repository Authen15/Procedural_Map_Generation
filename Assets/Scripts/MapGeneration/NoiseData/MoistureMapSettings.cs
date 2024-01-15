using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/MoistureMapSettings")]
public class MoistureMapSettings : DataMapSettings 
{
    [Space(10)]
    public float baseMoisture;

	#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endif

}