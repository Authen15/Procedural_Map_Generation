using UnityEngine;

public abstract class EffectDefinition : ScriptableObject
{
    public abstract RuntimeEffect CreateEffect(Creature source);
}
