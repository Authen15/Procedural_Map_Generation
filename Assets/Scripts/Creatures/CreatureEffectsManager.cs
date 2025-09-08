using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEffectManager : MonoBehaviour
{

    public List<EffectDefinition> OnHitTargetEffects; // effects that this creature applies to others on hit
    public List<EffectDefinition> OnHitSelfEffects; // effects that this creature applies on itself when hitting others
    public Dictionary<EffectDefinition, RuntimeEffect> CurrentEffects; // effects that are currently affecting the creature

    [SerializeField]
    private EffectDefinition[] InitialEffects; // the creature spawns with those effects

    private Creature _self;


    public void Awake()
    {
        _self = GetComponent<Creature>();
        CurrentEffects = new Dictionary<EffectDefinition, RuntimeEffect>();
    }

    public void Start()
    {
        ApplyInitialEffects();
        StartCoroutine(TickEffects());
    }

    private IEnumerator TickEffects()
    {
        WaitForSeconds waitForTick = new(1);

        while (true)
        {
            var toRemove = new List<EffectDefinition>();
            foreach (var kvp in CurrentEffects)
            {
                if (!kvp.Value.Tick(_self))
                {
                    toRemove.Add(kvp.Key); // using a list as dict can't be modified on iteration
                }
            }

            foreach (var def in toRemove)
                CurrentEffects.Remove(def);

            yield return waitForTick;
        }

    }

    public void ApplyEffects(EffectDefinition[] definitions, Creature source)
    {
        foreach (EffectDefinition definition in definitions)
        {
            if (!CurrentEffects.TryGetValue(definition, out RuntimeEffect effect))
            {
                effect = definition.CreateEffect(source);
                CurrentEffects.Add(definition, effect);
            }

            effect.Apply(_self);
        }
    }

    public void ApplyOnHitEffects(Creature target)
    {
        target.EffectManager.ApplyEffects(OnHitTargetEffects.ToArray(), _self);
        ApplyEffects(OnHitSelfEffects.ToArray(), _self);
    }

    public void ApplyInitialEffects()
    {
        ApplyEffects(InitialEffects, _self);
    }

    public void RegisterOnHitTargetEffects(EffectDefinition effectDefinition)
    {
        if (!OnHitTargetEffects.Contains(effectDefinition))
            OnHitTargetEffects.Add(effectDefinition);
    }

    public void RegisterOnHitSelfEffects(EffectDefinition effectDefinition)
    {
        if (!OnHitSelfEffects.Contains(effectDefinition))
            OnHitSelfEffects.Add(effectDefinition);
    }
}