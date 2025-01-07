using UnityEngine;
using UnityEngine.UI;

public class RessourceDisplay : MonoBehaviour
{
    public GameObject healthDisplay;
    public Image healthBarForeground;
    public Stats stats;

    private Camera mainCamera;
    private float targetFillAmount;
    [SerializeField]
    private float fillSpeed = 2f;

    private void Start()
    {
        mainCamera = Camera.main;
        targetFillAmount = healthBarForeground.fillAmount;
    }

    private void Update()
    {
        healthBarForeground.fillAmount = Mathf.MoveTowards(healthBarForeground.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);

        if (healthBarForeground.fillAmount >= 1f)
        {
            healthDisplay.SetActive(false);
        }
        else
        {
            healthDisplay.SetActive(true);
        }

        healthDisplay.transform.LookAt(mainCamera.transform);
    }

    public void UpdateHealthBar()
    {
        
        float healthPercentage = (float)stats.currentHealth / stats.maxHealth;
        targetFillAmount = healthPercentage;
    }
}
