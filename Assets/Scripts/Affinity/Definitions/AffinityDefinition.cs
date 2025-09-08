using UnityEngine;

[CreateAssetMenu(menuName = "Affinity/Definition")]
public class AffinityDefinition : ScriptableObject
{
    public const int MAX_LEVEL = 6;
    public const int UNLOCK_LEVEL = 1;

    public string Name;

    public AffinityType AffinityType;
    public AffinityDefinition Opposite;

    public FirstEvolutionDefinition FirstEvolutionDefinition;
    public SecondEvolutionDefinition SecondEvolutionDefinition;


    [TextArea] public string Description;

    public StatModifier[] OnceModifiers; // added once
    public StatModifier ScalingModifier; // added at each level up
}