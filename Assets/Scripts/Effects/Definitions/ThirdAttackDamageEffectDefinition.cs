using UnityEngine;

[CreateAssetMenu(menuName = "Effects/ThirdAttackDamageEffect")]
public class ThirdAttackDamageEffectDefinition : EffectDefinition
{
    public float Duration = 10f;
    public float DamageOnThirdHit = 20;

    public override RuntimeEffect CreateEffect(Creature source) => new ThirdAttackDamageEffect(this, source);
}

