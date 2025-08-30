using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Effects/RootEffect")]
public class RootEffect : Effect
{
    CreatureAgent _creatureAgent;

    public override IEnumerator Activate(Creature target, CreatureStats attacker)
    {
        if (!_creatureAgent) // get the creatureAgent and cache it
            _creatureAgent = target.GetComponent<CreatureAgent>();

        if (_creatureAgent == null)
        {
            Debug.LogError("CreatureAgent in null in RootEffect");
            yield break;
        }

        _creatureAgent.SetImmobilizeState(true);
        yield return new WaitForSeconds(Duration);
        _creatureAgent.SetImmobilizeState(false);
    }
}

