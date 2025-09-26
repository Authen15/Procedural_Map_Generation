using System;
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
        AddStatChangeListeners();
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

    public void UnRegisterOnHitTargetEffects(EffectDefinition effectDefinition) {
        OnHitTargetEffects.Remove(effectDefinition);
    }

    public void RegisterOnHitSelfEffects(EffectDefinition effectDefinition)
    {
        if (!OnHitSelfEffects.Contains(effectDefinition))
            OnHitSelfEffects.Add(effectDefinition);
    }

    public void UnRegisterOnHitSelfEffects(EffectDefinition effectDefinition) {
        OnHitSelfEffects.Remove(effectDefinition);
    }

    private void AddStatChangeListeners()
    {

        Action<float, EffectDefinition> registerOrUnregister= (f, def) =>
        {
            if (f > 0) RegisterOnHitTargetEffects(def);
            else UnRegisterOnHitTargetEffects(def);
        };


        // On-hit target Effect
        EffectDefinition def = EffectRegistry.GetEffectDefinition<AoeEffectDefinition>();
        _self.Stats.AoE.OnValueChanged.AddListener((float f) => registerOrUnregister(f, def));

        def = EffectRegistry.GetEffectDefinition<DotEffectDefinition>();
        _self.Stats.DoT.OnValueChanged.AddListener((float f) => registerOrUnregister(f, def));

        def = EffectRegistry.GetEffectDefinition<RootEffectDefinition>();
        _self.Stats.RootChance.OnValueChanged.AddListener((float f) => registerOrUnregister(f, def));

        def = EffectRegistry.GetEffectDefinition<SlowEffectDefinition>();
        _self.Stats.Slow.OnValueChanged.AddListener((float f) => registerOrUnregister(f, def));

        // On-hit self effect
        def = EffectRegistry.GetEffectDefinition<AuraEffectDefinition>();
        _self.Stats.AuraDamage.OnValueChanged.AddListener((float f) => registerOrUnregister(f, def));
    }
}