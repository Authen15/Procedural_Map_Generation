using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CreatureStats)), RequireComponent(typeof(CreatureEffectsManager))]
public abstract class Creature : MonoBehaviour
{
    public CreatureStats Stats;
    public float CurrentHealth;

    private CreatureEffectsManager _effectsManager;

    public virtual void Awake()
    {
        Stats = GetComponent<CreatureStats>();
        CurrentHealth = Stats.MaxHealthPoint.Value;
        _effectsManager = GetComponent<CreatureEffectsManager>();
    }

    // When the creature is beeing Hit, take damages and check for any effects 
    public void OnHit(CreatureStats attacker)
    {
        _effectsManager.ApplyOnHitEffects(attacker);
        OnHitDamage(attacker);
    }

    protected virtual void OnHitDamage(CreatureStats attacker)
    {
        // TODO take in care the resistance of the creature

        // TODO, apply damage depending on affinity resistance/weakness

        
        float totalDamage = attacker.NeutralDamage.Value + attacker.AshenDamage.Value + attacker.EmberDamage.Value + attacker.FrostDamage.Value + attacker.VerdantDamage.Value;

        TakeDamage(totalDamage);
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

        // TODO add execution threshold
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float heal)
    {
        CurrentHealth += heal;

        if (CurrentHealth > Stats.MaxHealthPoint.Value)
        {
            CurrentHealth = Stats.MaxHealthPoint.Value;
        }
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }

    public IEnumerator Regeneration()
    {
        Heal(Stats.HealthRegeneration.Value);
        yield return new WaitForSeconds(1);
    }
}
