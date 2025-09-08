using UnityEngine;

public class RootEffect : RuntimeEffect
{
    float _duration;
    float _startTime;

    float _remainingTime => _startTime + _duration - Time.time;

    CreatureAgent _creatureAgent;
    Stat _sourceRootChance;

    public RootEffect(RootEffectDefinition definition, Creature source)
    {
        _duration = definition.Duration;
        _sourceRootChance = source.Stats.RootChance;
    }

    public override void Apply(Creature self)
    {
        if (_creatureAgent == null)
            _creatureAgent = self.GetComponent<CreatureAgent>();
            
        if (_sourceRootChance.Value >= Random.value)
        {
            _creatureAgent.SetImmobilizeState(true);
            _startTime = Time.time;
        }
    }

    public override bool Tick(Creature self)
    {
        if (_remainingTime < 0)
        {
            Reset();
            return false;
        }
        return true;
    }
    
    private void Reset()
    {
        _creatureAgent.SetImmobilizeState(false);
    }
}

