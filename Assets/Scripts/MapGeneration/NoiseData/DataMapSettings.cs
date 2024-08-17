using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/DefaultDataMapSettings")]
public class DataMapSettings : ScriptableObject 
{
    public NoiseSettings noiseSettings;
    
	#if UNITY_EDITOR

	protected virtual void OnValidate() {
		noiseSettings.ValidateValues ();
	}

	#endif
}