using UnityEngine;

[CreateAssetMenu(menuName = "Effects/HealthRegeneration")]
public class HealthRegenerationEffectDefinition : EffectDefinition
{
    public override RuntimeEffect CreateEffect(Creature source) => new HealthRegenerationEffect(this, source);
}

