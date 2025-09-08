using UnityEngine;

public class DotEffect : RuntimeEffect
{
    float _duration;
    float _startTime;

    Stat _doT;
    float _damagePerTick => _doT.Value / _duration;

    float _remainingTime => _startTime + _duration - Time.time;

    public DotEffect(DotEffectDefinition definition, Creature source)
    {
        _duration = definition.Duration;
        _doT = source.Stats.DoT;
    }

    public override void Apply(Creature self)
    {
        _startTime = Time.time;
    }

    public override bool Tick(Creature self)
    {
        if (_remainingTime < 0)
            return false;

        self.TakeDamage(_damagePerTick);
        return true;
    }
}

