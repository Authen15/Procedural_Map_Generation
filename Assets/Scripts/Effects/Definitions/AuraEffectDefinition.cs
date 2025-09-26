using UnityEngine;

[CreateAssetMenu(menuName = "Effects/AuraEffect")]
public class AuraEffectDefinition : EffectDefinition
{
    public LayerMask LayerMask;
    public float Radius = 5f;

    public override RuntimeEffect CreateEffect(Creature source) => new AuraEffect(this, source);
    
}

