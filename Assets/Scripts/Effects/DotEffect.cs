using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/DotEffect")]
public class DotEffect : Effect
{
    public override IEnumerator Activate(Creature target, CreatureStats attacker)
    {
        float duration = Duration;
        while (duration >= 0)
        {
            yield return new WaitForSeconds(1);
            duration--;
            target.TakeDamage(attacker.DoT.Value / duration);
        }
    }
}

