using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RootEffect")]
public class RootEffectDefinition : EffectDefinition
{
    public float Duration = 2f;

    public override RuntimeEffect CreateEffect(Creature source) => new RootEffect(this, source);
}

