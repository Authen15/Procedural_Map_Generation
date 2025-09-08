using UnityEngine;

public class AttackSpeedPerHitEffect : RuntimeEffect
{
    float _attackSpeedPerHit;
    float _duration;

    float _lastHitTime;
    int _counter;

    float _remainingTime => _lastHitTime + _duration - Time.time;

    public AttackSpeedPerHitEffect(AttackSpeedPerHitEffectDefinition definition, Creature source)
    {
        _attackSpeedPerHit = definition.AttackSpeedPerHit;
        _duration = definition.Duration;

        _lastHitTime = 0;
        _counter = 0;
    }

    public override void Apply(Creature self)
    {
        _lastHitTime = Time.time;
        _counter++;

        self.Stats.AttackSpeed.AddMultiplierBonus(_attackSpeedPerHit);
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


    private void Reset(Creature self)
    {
        self.Stats.AttackSpeed.AddMultiplierBonus(-_attackSpeedPerHit * _counter);
        _counter = 0;
    }
}

