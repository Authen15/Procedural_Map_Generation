using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CreatureStats))]
public class PlayerAffinityManager : MonoBehaviour
{
    public static PlayerAffinityManager Instance { get; private set; }

    [Header("Affinity Definitions")]
    public List<AffinityDefinition> AffinityDefinitions;

    private Dictionary<AffinityType, PlayerAffinity> Affinities;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        InitAffinities();
    }

    void InitAffinities()
    {
        Affinities = new Dictionary<AffinityType, PlayerAffinity>();
        CreatureStats playerStats = GetComponent<CreatureStats>();

        foreach (var definition in AffinityDefinitions)
        {
            Affinities[definition.AffinityType] = new PlayerAffinity(definition, playerStats);
        }
    }

    // Call when an enemy is killed to distribute XP
    public void OnCreatureKilled(EnemyCreature enemy)
    {
        AddAffinityXP(AffinityType.Ember, enemy.EmberXPAmount);
        AddAffinityXP(AffinityType.Frost, enemy.FrostXPAmount);
        AddAffinityXP(AffinityType.Ashen, enemy.AshenXPAmount);
        AddAffinityXP(AffinityType.Verdant, enemy.VerdantXPAmount);
    }

    // add XP to an affinity and subtract from its opposite
    private void AddAffinityXP(AffinityType type, float amount)
    {
        if (amount <= 0f) return;

        if (Affinities.TryGetValue(type, out var affinity))
        {
            affinity.AddXP(amount);

            // Subtract same amount from opposite affinity
            var def = affinity.Definition;
            if (def.Opposite != null)
            {
                var oppType = def.Opposite.AffinityType;
                if (Affinities.TryGetValue(oppType, out var oppAffinity))
                {
                    oppAffinity.AddXP(-amount);
                }
            }
        }
    }

    public PlayerAffinity GetAffinity(AffinityType type)
    {
        return Affinities[type];
    }
}
