using UnityEngine;

public class AuraEffect : RuntimeEffect
{
    LayerMask _layerMask;
    float _radius;

    Stat _auraDamage;

    public AuraEffect(AuraEffectDefinition definition, Creature source)
    {
        _layerMask = definition.LayerMask;
        _radius = definition.Radius;

        _auraDamage = source.Stats.AuraDamage;
    }

    public override void Apply(Creature self)
    {
        
    }

    public override bool Tick(Creature self)
    {
        Collider[] colliders = Physics.OverlapSphere(self.transform.position, _radius, _layerMask);
        foreach (Collider collider in colliders)
        {
            Creature inRangeTarget = collider.gameObject.GetComponent<Creature>();

            if (inRangeTarget != null || inRangeTarget != self)
                inRangeTarget.TakeDamage(_auraDamage.Value);
        }
        return true;
    }
}

