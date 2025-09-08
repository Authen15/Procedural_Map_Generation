using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyCreature : Creature
{
    private RessourceDisplay _ressourceDisplay;
    private InputAction _debugDamageAction;
    private InputAction _debugHealAction;
    

    [Header("Experience")]
    public int EmberXPAmount;
    public int FrostXPAmount;
    public int AshenXPAmount;
    public int VerdantXPAmount;

    public override void Awake()
    {
        base.Awake();

        _ressourceDisplay = GetComponent<RessourceDisplay>();

        _debugDamageAction = InputSystem.actions.FindAction("DebugDamage");
        _debugHealAction = InputSystem.actions.FindAction("DebugHeal");

        if (_debugDamageAction != null)
            _debugDamageAction.performed += OnDebugDamage;

        if (_debugHealAction != null)
            _debugHealAction.performed += OnDebugHeal;
    }
    

    private void Update() {
        if (_debugDamageAction.WasPressedThisFrame())
        {
            TakeDamage(10);
        }
        if (_debugHealAction.WasPressedThisFrame())
        {
            Heal(10);
        }
    }

    
    public override void TakeDamage(float damage)
    {
        CurrentHealth -= damage;
        _ressourceDisplay.UpdateHealthBar();
        // TODO add execution threshold
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    protected override void OnHitDamage(CreatureStats attacker)
    {
        base.OnHitDamage(attacker);
    }

    public override void Heal(float heal)
    {
        base.Heal(heal);
        _ressourceDisplay.UpdateHealthBar();
    }

    public override void Die()
    {
        base.Die();
    }

    private void OnEnable()
    {
        _debugDamageAction?.Enable();
        _debugHealAction?.Enable();
    }

    private void OnDisable()
    {
        _debugDamageAction?.Disable();
        _debugHealAction?.Disable();
    }

    private void OnDebugDamage(InputAction.CallbackContext context)
    {
        TakeDamage(10);
    }

    private void OnDebugHeal(InputAction.CallbackContext context)
    {
        Heal(10);
    }
}
