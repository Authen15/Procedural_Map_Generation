using UnityEngine;
using UnityEngine.UI;

public class AffinityProgressUI : MonoBehaviour
{
    public Image ProgressBarForeground;
    public AffinityType affinityType;
    public Text LevelText;

    private PlayerAffinity _affinity;
    private Coroutine fillCoroutine;

    public void Start()
    {
        ProgressBarForeground.fillAmount = 0;

        _affinity = PlayerAffinityManager.Instance.GetPlayerAffinityByType(affinityType);

        _affinity.OnAffinityChanged += UpdateUI;

        UpdateUI();
    }

    public void UpdateUI()
    {
        float targetFill = _affinity.CurrentXP / _affinity.GetNextLevelXP();
        if (fillCoroutine != null)
            StopCoroutine(fillCoroutine);

        // Using PlayerAffinityManager.Instance as it won't be unactive for the coroutine
        fillCoroutine = PlayerAffinityManager.Instance.StartCoroutine(UIAnimationUtils.AnimateFillAmount(ProgressBarForeground, targetFill));

        LevelText.text = _affinity.CurrentLevel + "/" + AffinityDefinition.MAX_LEVEL;
    }

    // void OnDestroy()
    // {
    //     if (PlayerAffinityManager.Instance != null)
    //     {
    //         _affinity.OnAffinityChanged -= UpdateUI;
    //     }
    // }
}
