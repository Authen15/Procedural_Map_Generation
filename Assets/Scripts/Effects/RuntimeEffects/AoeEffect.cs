using UnityEngine;

public class AoeEffect : RuntimeEffect
{
    LayerMask _layerMask;
    float _radius;

    Stat _aoE;

    public AoeEffect(AoeEffectDefinition definition, Creature source)
    {
        _layerMask = definition.LayerMask;
        _radius = definition.Radius;

        _aoE = source.Stats.AoE;
    }

    public override void Apply(Creature self)
    {
        Collider[] colliders = Physics.OverlapSphere(self.transform.position, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            Creature inRangeTarget = collider.gameObject.GetComponent<Creature>();

            if (inRangeTarget != null || inRangeTarget != self)
                inRangeTarget.TakeDamage(_aoE.Value);
        }
    }

    public override bool Tick(Creature self)
    {
        return false;
    }
}

