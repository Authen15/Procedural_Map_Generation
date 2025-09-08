public class HealthRegenerationEffect : RuntimeEffect
{
    Stat _healthRegen;

    public HealthRegenerationEffect(HealthRegenerationEffectDefinition definition, Creature source)
    {
        _healthRegen = source.Stats.HealthRegeneration;
    }

    public override bool Tick(Creature self)
    {
        self.Heal(_healthRegen.Value);
        return true;
    }


    public override void Apply(Creature self)
    {
    }
}

