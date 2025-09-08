using UnityEngine;

[CreateAssetMenu(menuName = "Effects/AttackSpeedPerHitEffect")]
public class AttackSpeedPerHitEffectDefinition : EffectDefinition
{
    public float Duration = 10f;
    public float AttackSpeedPerHit = 0.02f;

    public override RuntimeEffect CreateEffect(Creature source) => new AttackSpeedPerHitEffect(this, source);
}

