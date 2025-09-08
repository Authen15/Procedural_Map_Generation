using UnityEngine;

[CreateAssetMenu(menuName = "Effects/DotEffect")]
public class DotEffectDefinition : EffectDefinition
{
    public float Duration = 4f;

    public override RuntimeEffect CreateEffect(Creature source) => new DotEffect(this, source);
}

