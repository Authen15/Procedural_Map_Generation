using UnityEngine;

public class DotEffect : RuntimeEffect
{
    private float _duration;
    private float _startTime;

    private Stat _doT;
    private float _damagePerTick => _doT.Value / _duration;

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

        _duration--;
        self.TakeDamage(_damagePerTick);
        return true;
    }
}

