using UnityEngine;

[CreateAssetMenu(menuName = "Effects/AoeEffect")]
public class AoeEffectDefinition : EffectDefinition
{
    public LayerMask LayerMask;

    public override RuntimeEffect CreateEffect(Creature source) => new AoeEffect(this, source);
    
}

