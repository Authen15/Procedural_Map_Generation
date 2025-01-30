using UnityEngine;
using UnityEngine.InputSystem;

public class Creature : MonoBehaviour
{
    private Stats stats;
    private RessourceDisplay ressourceDisplay;
    private InputAction _debugDamageAction;
    private InputAction _debugHealAction;

    private void Awake()
    {
        stats = GetComponent<Stats>();
        ressourceDisplay = GetComponent<RessourceDisplay>();
        stats.currentHealth = stats.maxHealth;

        _debugDamageAction = InputSystem.actions.FindAction("DebugDamage");
        _debugHealAction = InputSystem.actions.FindAction("DebugHeal");
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

    public void TakeDamage(int damage)
    {
        stats.currentHealth -= damage;
        ressourceDisplay.UpdateHealthBar();
        if (stats.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int heal)
    {
        stats.currentHealth += heal;
        ressourceDisplay.UpdateHealthBar();
        if (stats.currentHealth > stats.maxHealth)
        {
            stats.currentHealth = stats.maxHealth;
        }
        
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    

    
}
