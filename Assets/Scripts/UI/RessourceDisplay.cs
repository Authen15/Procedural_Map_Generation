using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Creature))]
public class RessourceDisplay : MonoBehaviour
{
    public GameObject HealthDisplay;
    public Image HealthBarForeground;
    public Creature Creature;

    private Camera _mainCamera;
    [SerializeField]
    private IEnumerator _fillCoroutine;

    private void Start()
    {
        _mainCamera = Camera.main;
        HealthDisplay.SetActive(false);
        
        Creature = GetComponent<Creature>();
    }

    private void Update()
    {
        HealthDisplay.transform.LookAt(_mainCamera.transform);
    }

    public void UpdateHealthBar()
    {
        if (_fillCoroutine != null)
            StopCoroutine(_fillCoroutine);

        float targetFill = Creature.CurrentHealth / Creature.Stats.MaxHealthPoint.Value;
        _fillCoroutine = UIAnimationUtils.AnimateFillAmount(HealthBarForeground, targetFill);
        StartCoroutine(_fillCoroutine);

        if (targetFill >= 1f)
        {
            HealthDisplay.SetActive(false);
        }
        else
        {
            HealthDisplay.SetActive(true);
        }
    }

    void OnDestroy()
    {
         if (_fillCoroutine != null)
            StopCoroutine(_fillCoroutine);
    }
}
