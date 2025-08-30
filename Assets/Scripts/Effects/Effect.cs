using System.Collections;
using UnityEngine;

public abstract class Effect : ScriptableObject
{
    [Header("Cooldown (seconds)")]
    public float Cooldown = 1f;

    [Header("Duration (seconds)")]
    public float Duration = 2f;

    private float _lastUseTime = -Mathf.Infinity;

    public bool IsOnCooldown => Time.time < _lastUseTime + Cooldown;

    public abstract IEnumerator Activate(Creature target, CreatureStats attacker);
}
