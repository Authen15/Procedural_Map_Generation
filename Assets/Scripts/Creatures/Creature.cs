using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CreatureStats)), RequireComponent(typeof(CreatureEffectManager))]
public abstract class Creature : MonoBehaviour
{
    public CreatureStats Stats;
    public float CurrentHealth;

    public CreatureEffectManager EffectManager;

    public virtual void Awake()
    {
        Stats = GetComponent<CreatureStats>();
        CurrentHealth = Stats.MaxHealthPoint.Value;
        EffectManager = GetComponent<CreatureEffectManager>();
    }

    public void OnHit(Creature attacker)
    {
        float damage = 0;
        damage += GetNeutralDamage(attacker);
        damage += GetAffinityDamage(attacker);

        TakeDamage(damage);
        Execute(attacker);
    }

    private float GetAffinityDamage(Creature attacker)
    {
        float affinityDamage = 0;

        PlayerAffinityManager affinityManager = attacker.GetComponent<PlayerAffinityManager>();

        affinityDamage += attacker.Stats.AshenDamage.Value * affinityManager.GetDamageMultiplier(AffinityType.Ashen);
        affinityDamage += attacker.Stats.EmberDamage.Value * affinityManager.GetDamageMultiplier(AffinityType.Ember);
        affinityDamage += attacker.Stats.FrostDamage.Value * affinityManager.GetDamageMultiplier(AffinityType.Frost);
        affinityDamage += attacker.Stats.VerdantDamage.Value * affinityManager.GetDamageMultiplier(AffinityType.Verdant);

        return affinityDamage;
    }

    private float GetNeutralDamage(Creature attacker)
    {
        float effectiveArmor = Stats.Armor.Value * (1 - attacker.Stats.ArmorPenetration.Value);
        float damageMultiplier = 50 / (50 + effectiveArmor);

        return damageMultiplier * attacker.Stats.NeutralDamage.Value;
    }

    private void Execute(Creature attacker)
    {
        float healthPercent = CurrentHealth / Stats.MaxHealthPoint.Value;
        if (attacker.Stats.ExecutionThreshold.Value > healthPercent)
        {
            Die();
        }
    }

    public virtual void TakeDamage(float damage)
    {
        CurrentHealth -= damage;

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
