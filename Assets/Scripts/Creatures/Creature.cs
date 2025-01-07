using UnityEngine;

public class Creature : MonoBehaviour
{

    private Stats stats;
    private RessourceDisplay ressourceDisplay;

    private void Awake()
    {
        stats = GetComponent<Stats>();
        ressourceDisplay = GetComponent<RessourceDisplay>();
        stats.currentHealth = stats.maxHealth;
    } 

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(10);
        }
        if (Input.GetKeyDown(KeyCode.H))
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
