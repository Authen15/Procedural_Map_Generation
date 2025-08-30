using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureEffectsManager : MonoBehaviour
{
    private Creature _creature;

    [SerializeField]
    private Effect _slowEffect;
    [SerializeField]
    private Effect _rootEffect;
    [SerializeField]
    private Effect _doTEffect;

    private Dictionary<Effect, Coroutine> _activeCoroutines = new();

    public void Awake()
    {
        _creature = GetComponent<Creature>();
    }

    public void ApplyOnHitEffects(CreatureStats attacker)
    {
        if (Random.value <= attacker.RootChance.Value)
            ApplyEffect(_rootEffect, attacker);

        ApplyEffect(_slowEffect, attacker);
        ApplyEffect(_doTEffect, attacker);
    }

    void ApplyEffect(Effect effect, CreatureStats attacker)
    {
        if (effect == null) return;

        // Handle cooldown
        if (effect.IsOnCooldown)
            return;

        
        if (_activeCoroutines.TryGetValue(effect, out var existing))
        {
            _creature.StopCoroutine(existing);
            _activeCoroutines.Remove(effect);
        }

        Coroutine routine = _creature.StartCoroutine(RunEffect(effect, attacker));
        _activeCoroutines[effect] = routine;
    }

    private IEnumerator RunEffect(Effect effect, CreatureStats attacker)
    {
        yield return effect.Activate(_creature, attacker);
        _activeCoroutines.Remove(effect);
    }
}