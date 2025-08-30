using UnityEngine;

[CreateAssetMenu(menuName = "Affinity/Stat/StatModifier")]
public class StatModifier : ScriptableObject
{
    [Header("Target Stat")]
    public StatType TargetStat;

    [Header("Bonuses")]
    public float FlatBonus;
    public float MultiplierBonus = 0f;

    public void Apply(CreatureStats stats)
    {
        var stat = stats.GetStat(TargetStat);

        if (FlatBonus != 0)
            stat.AddFlatBonus(FlatBonus);

        if (MultiplierBonus != 0)
            stat.AddMultiplierBonus(MultiplierBonus);
    }

    public void Remove(CreatureStats stats)
    {
        var stat = stats.GetStat(TargetStat);

        if (FlatBonus != 0)
            stat.AddFlatBonus(-FlatBonus);

        if (MultiplierBonus != 0)
            stat.AddMultiplierBonus(-MultiplierBonus);
    }
}

