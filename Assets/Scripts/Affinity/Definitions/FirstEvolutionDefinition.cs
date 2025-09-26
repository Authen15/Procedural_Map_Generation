using UnityEngine;

[CreateAssetMenu(menuName = "Affinity/FirstEvolution")]
public class FirstEvolutionDefinition : ScriptableObject
{
    public const int UNLOCK_LEVEL = 3;
    public string Name;

    [TextArea] public string Description;

    public StatModifier[] OnceModifiers; // added once
    public StatModifier ScalingModifier; // added at each level up
}
