using UnityEngine;

[CreateAssetMenu(menuName = "DataMapSettings/DefaultDataMapSettings")]
public class DataMapSettings : ScriptableObject 
{
    public NoiseSettings NoiseSettings;
    
	#if UNITY_EDITOR

	protected virtual void OnValidate() {
		NoiseSettings.ValidateValues ();
	}

	#endif
}