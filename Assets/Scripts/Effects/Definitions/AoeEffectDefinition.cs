using UnityEngine;

[CreateAssetMenu(menuName = "Effects/AoeEffect")]
public class AoeEffectDefinition : EffectDefinition
{
    public LayerMask LayerMask;
    public float Radius = 5f;

    public override RuntimeEffect CreateEffect(Creature source) => new AoeEffect(this, source);
    
}

