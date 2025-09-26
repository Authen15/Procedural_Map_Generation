using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/EffectRegistry")]
public class EffectRegistry : ScriptableObject
{
    [Header("Effect Definitions")]
    [SerializeField]
    private List<EffectDefinition> _effectDefinitions = new List<EffectDefinition>(); //TODO add Resources.Load to automatically load effects

    private static EffectRegistry _instance;
    public static EffectRegistry Instance {
        get {
            if (_instance == null)
                _instance = Resources.Load<EffectRegistry>("EffectRegistry");
            return _instance;
        }
    }

    public static T GetEffectDefinition<T>() where T : EffectDefinition
    {
        if (Instance == null || Instance._effectDefinitions == null)
        {
            Debug.LogError("EffectRegistry not initialized!");
            return null;
        }
        foreach (EffectDefinition def in Instance._effectDefinitions)
        {
            if (def is T) return (T)def;
        }
        Debug.LogError($"EffectDefinition {typeof(T)} not found in EffectRegistry");
        return null;
    }
}