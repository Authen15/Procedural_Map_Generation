using UnityEngine;

public class SlowEffect : RuntimeEffect
{
    float _duration;
    Stat _slowStat;

    float _appliedAmount;
    float _startTime;

    float _remainingTime => _startTime + _duration - Time.time;

    public SlowEffect(SlowEffectDefinition definition, Creature source)
    {
        _slowStat = source.Stats.Slow;
        _duration = definition.Duration;
    }

    public override bool Tick(Creature self)
    {
        if (_remainingTime < 0)
        {
            Reset(self);
            return false;
        } 
        return true;
    }


    public override void Apply(Creature self)
    {
        Reset(self);
        Slow(self);
    }

    private void Slow(Creature self)
    {
        _startTime = Time.time;

        _appliedAmount = _slowStat.Value;
        _appliedAmount = Mathf.Clamp01(_appliedAmount);

        self.Stats.MovementSpeed.AddMultiplierBonus(-_appliedAmount);
    }

    private void Reset(Creature self)
    {
        if (_appliedAmount > 0)
            self.Stats.MovementSpeed.AddMultiplierBonus(_appliedAmount);

        _appliedAmount = 0;
    }
}

