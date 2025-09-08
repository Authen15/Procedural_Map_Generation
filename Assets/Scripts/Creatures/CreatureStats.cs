using UnityEngine;

public class CreatureStats : MonoBehaviour
{
    [Header("Health")]
    public Stat MaxHealthPoint = new Stat();
    public Stat HealthRegeneration = new Stat();

    [Header("Damage Types")]
    public Stat NeutralDamage = new Stat();
    public Stat AshenDamage = new Stat();
    public Stat EmberDamage = new Stat();
    public Stat FrostDamage = new Stat();
    public Stat VerdantDamage = new Stat();

    [Header("Offensive")]
    public Stat AttackSpeed = new Stat();
    public Stat AoE = new Stat();
    public Stat ExecutionThreshold = new Stat();
    public Stat ArmorPenetration = new Stat();
    public Stat AuraDamage = new Stat();

    [Header("Defensive")]
    public Stat Armor = new Stat();

    [Header("On-Hit Effects")]
    public Stat DoT = new Stat();
    public Stat Slow = new Stat();
    public Stat RootChance = new Stat();

    [Header("Sustain")]
    public Stat Lifesteal = new Stat();

    [Header("Mobility")]
    public Stat MovementSpeed = new Stat();


    public Stat GetStat(StatType type)
    {
        return type switch
        {
            StatType.NeutralDamage => NeutralDamage,
            StatType.AshenDamage => AshenDamage,
            StatType.EmberDamage => EmberDamage,
            StatType.FrostDamage => FrostDamage,
            StatType.VerdantDamage => VerdantDamage,
            StatType.ArmorPenetration => ArmorPenetration,
            StatType.DoT => DoT,
            StatType.AuraDamage => AuraDamage,
            StatType.AoE => AoE,
            StatType.RootChance => RootChance,
            StatType.Slow => Slow,
            StatType.ExecutionThreshold => ExecutionThreshold,
            StatType.AttackSpeed => AttackSpeed,
            StatType.MovementSpeed => MovementSpeed,
            StatType.Armor => Armor,
            StatType.Lifesteal => Lifesteal,
            StatType.MaxHealthPoint => MaxHealthPoint,
            StatType.HealthRegeneration => HealthRegeneration,
            _ => throw new System.ArgumentException($"StatType {type} not handled"),
        };
    }
}
