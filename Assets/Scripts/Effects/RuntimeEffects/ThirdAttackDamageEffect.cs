using UnityEngine;

public class ThirdAttackDamageEffect : RuntimeEffect
{
    float _duration;
    float _damageOnThirdHit;

    float _lastHitTime;
    int _counter;

    float _remainingTime => _lastHitTime + _duration - Time.time;

    public ThirdAttackDamageEffect(ThirdAttackDamageEffectDefinition definition, Creature source)
    {
        _duration = definition.Duration;
        _damageOnThirdHit = definition.DamageOnThirdHit;

        _lastHitTime = 0;
        _counter = 0;
    }

    public override void Apply(Creature self)
    {
        _lastHitTime = Time.time;

        _counter++;

        if (_counter == 3)
        {
            _counter = 0;
            self.TakeDamage(_damageOnThirdHit);
        }
    }

    public override bool Tick(Creature self)
    {
        if (_remainingTime < 0)
        {
            _counter = 0;
            return false;
        }
        return true;
    }
}

