using UnityEngine;

[CreateAssetMenu(menuName = "Affinity/SecondEvolution")]
public class SecondEvolutionDefinition : ScriptableObject
{
    public const int UNLOCK_LEVEL = 6;
    public string Name;

    [TextArea] public string Description;
    public EffectDefinition Effect;
}
