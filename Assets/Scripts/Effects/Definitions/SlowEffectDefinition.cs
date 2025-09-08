using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SlowEffect")]
public class SlowEffectDefinition : EffectDefinition
{
    public float Duration = 1f;

    public override RuntimeEffect CreateEffect(Creature source) => new SlowEffect(this, source);
}

