using UnityEngine;

public class CreatureStats : MonoBehaviour
{
    [Header("Damage Stats")]
    public Stat NeutralDamage = new Stat();
    public Stat AshenDamage = new Stat();
    public Stat EmberDamage = new Stat();
    public Stat FrostDamage = new Stat();
    public Stat VerdantDamage = new Stat();

    [Header("Misc Stats")]
    public Stat ArmorPenetration = new Stat();
    public Stat DoT = new Stat();
    public Stat AuraDamage = new Stat();
    public Stat AoE = new Stat();
    public Stat RootChance = new Stat();
    public Stat Slow = new Stat();
    public Stat ExecutionThreshold = new Stat();

    public Stat AttackSpeed = new Stat();
    public Stat MovementSpeed = new Stat();

    [Header("Sustain Stats")]
    public Stat Armor = new Stat();
    public Stat Lifesteal = new Stat();
    public Stat MaxHealthPoint = new Stat();
    public Stat HealthRegeneration = new Stat();

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
