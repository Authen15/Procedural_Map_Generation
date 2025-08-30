using System;
using UnityEngine;

[Serializable]
public class PlayerAffinity
{
    [HideInInspector] public Action OnAffinityChanged;

    public AffinityDefinition Definition;

    private CreatureStats _stats;

    public float CurrentXP = 0f;
    public int CurrentLevel = 0;

    public PlayerAffinity(AffinityDefinition definition, CreatureStats stats)
    {
        Definition = definition;
        CurrentXP = 0f;
        CurrentLevel = 0;
        _stats = stats;
    }

    public void AddXP(float amount)
    {
        CurrentXP += amount;

        while (CurrentLevel < AffinityDefinition.MAX_LEVEL && CurrentXP >= GetNextLevelXP())
        {
            CurrentXP -= GetNextLevelXP();
            LevelUp();
        }

        // Level down if XP drops below 0
        while (CurrentLevel > 0 && CurrentXP < 0f)
        {
            CurrentXP += GetNextLevelXP();
            LevelDown();
        }

        // Notify any listeners that affinity changed
        OnAffinityChanged?.Invoke();
    }

    void LevelUp()
    {
        CurrentLevel++;
        ApplyModifiers();
    }

    void LevelDown()
    {
        CurrentLevel--;
        RemoveModifiers();
    }

    public float GetNextLevelXP()
    {
        return 100f * Mathf.Pow(1.1f, CurrentLevel);
    }

    void ApplyModifiers()
    {
        if (_stats == null) return;

        // Applied only once when the affinity is unlocked
        if (CurrentLevel == AffinityDefinition.UNLOCK_LEVEL)
            ApplyOnceModifiers(Definition.OnceModifiers);

        Definition.ScalingModifier.Apply(_stats);

        // First evolution effects if unlocked
        if (CurrentLevel >= FirstEvolutionDefinition.UNLOCK_LEVEL)
        {
            // Applied only once when the evolution is unlocked
            if (CurrentLevel == FirstEvolutionDefinition.UNLOCK_LEVEL)
                ApplyOnceModifiers(Definition.FirstEvolutionDefinition.OnceModifiers);

            Definition.FirstEvolutionDefinition.ScalingModifier.Apply(_stats);
        }
    }

    void ApplyOnceModifiers(StatModifier[] modifiers)
    {
        foreach (StatModifier modifier in modifiers)
        {
            modifier.Apply(_stats);
        }
    }

    void RemoveModifiers()
    {
        if (_stats == null) return;

        // Removed only if the affinity have just been lost
        if (CurrentLevel < AffinityDefinition.UNLOCK_LEVEL - 1)
            RemoveOnceModifiers(Definition.OnceModifiers);

        Definition.ScalingModifier.Remove(_stats);

        //Removed only if the evolution have just been lost
        if (CurrentLevel == FirstEvolutionDefinition.UNLOCK_LEVEL - 1)
            ApplyOnceModifiers(Definition.FirstEvolutionDefinition.OnceModifiers);

        if (CurrentLevel >= FirstEvolutionDefinition.UNLOCK_LEVEL)
            Definition.FirstEvolutionDefinition.ScalingModifier.Remove(_stats);
    }

    void RemoveOnceModifiers(StatModifier[] modifiers)
    {
        foreach (StatModifier modifier in modifiers)
        {
            modifier.Remove(_stats);
        }
    }
}
