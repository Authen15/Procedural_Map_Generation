using UnityEngine;

public class AoeEffect : RuntimeEffect
{
    LayerMask layerMask;
    float _value;

    public AoeEffect(AoeEffectDefinition definition, Creature source)
    {
        layerMask = definition.LayerMask;
    }

    public override void Apply(Creature self)
    {
        Collider[] colliders = Physics.OverlapSphere(self.transform.position, 5, layerMask);
        foreach (Collider collider in colliders)
        {
            Creature inRangeTarget = collider.gameObject.GetComponent<Creature>();

            if (inRangeTarget != null || inRangeTarget != self)
                inRangeTarget.TakeDamage(_value);
        }
    }

    public override bool Tick(Creature self)
    {
        return false;
    }
}

