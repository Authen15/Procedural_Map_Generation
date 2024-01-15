using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/DefaultDataMapSettings")]
public class DataMapSettings : UpdatableData 
{
    public NoiseSettings noiseSettings;

	#if UNITY_EDITOR
    protected override void OnValidate()
    {
        base.OnValidate();
    }
    #endif

}