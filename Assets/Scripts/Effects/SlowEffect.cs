using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/SlowEffect")]
public class SlowEffect : Effect
{
    public override IEnumerator Activate(Creature target, CreatureStats attacker)
    {
        //TODO check if this doesn't go in negative
        float amount = attacker.Slow.Value;

        target.Stats.MovementSpeed.AddMultiplierBonus(-amount);
        yield return new WaitForSeconds(Duration);
        target.Stats.MovementSpeed.AddMultiplierBonus(amount);
    }
}

