using UnityEngine;
using UnityEngine.UI;

public class RessourceDisplay : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBarForeground;
    public Stats stats;

    private float targetFillAmount;
    [SerializeField]
    private float fillSpeed = 2f;

    private void Start()
    {
        healthBarForeground = GameObject.Find("healthBarForeground").GetComponent<Image>();
        healthBarBackground = GameObject.Find("healthBarBackground").GetComponent<Image>();
        stats = GetComponent<Stats>();

        targetFillAmount = healthBarForeground.fillAmount;
    }

    private void Update()
    {
        healthBarForeground.fillAmount = Mathf.MoveTowards(healthBarForeground.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);

        if (healthBarForeground.fillAmount >= 1f)
        {
            healthBarBackground.enabled = false;
            healthBarForeground.enabled = false;
        }
        else
        {
            healthBarBackground.enabled = true;
            healthBarForeground.enabled = true;
        }
    }

    public void UpdateHealthBar()
    {
        
        float healthPercentage = (float)stats.currentHealth / stats.maxHealth;
        targetFillAmount = healthPercentage;
    }
}
