using UnityEngine;
using UnityEngine.UI;

public class RessourceDisplay : MonoBehaviour
{
    public Image healthBarBackground;
    public Image healthBarForeground; // L'image qui se remplit pour représenter la vie actuelle
    public Stats stats; // Référence au script "Stats" qui contient les variables de vie

    private float targetFillAmount; // Valeur de remplissage cible de la barre de vie
    [SerializeField]
    private float fillSpeed = 2f; // Vitesse de remplissage de la barre de vie (ajustez selon vos besoins)

    private void Start()
    {
        healthBarForeground = GameObject.Find("healthBarForeground").GetComponent<Image>();
        healthBarBackground = GameObject.Find("healthBarBackground").GetComponent<Image>();
        stats = GetComponent<Stats>();

        targetFillAmount = healthBarForeground.fillAmount;
    }

    private void Update()
    {
        // Interpoler en douceur vers la valeur cible de remplissage de la barre de vie
        healthBarForeground.fillAmount = Mathf.MoveTowards(healthBarForeground.fillAmount, targetFillAmount, fillSpeed * Time.deltaTime);

        if (healthBarForeground.fillAmount >= 1f)
        {
            healthBarBackground.enabled = false; // Désactiver l'affichage de l'image
            healthBarForeground.enabled = false; // Désactiver l'affichage de l'image
        }
        else
        {
            healthBarBackground.enabled = true; // Activer l'affichage de l'image
            healthBarForeground.enabled = true; // Activer l'affichage de l'image
        }
    }

    public void UpdateHealthBar()
    {
        // Calculer le pourcentage de vie actuelle
        float healthPercentage = (float)stats.currentHealth / stats.maxHealth;

        // Mettre à jour la valeur cible de remplissage de la barre de vie
        targetFillAmount = healthPercentage;
    }
}
