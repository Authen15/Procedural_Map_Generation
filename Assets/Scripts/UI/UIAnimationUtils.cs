using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class UIAnimationUtils
{
    public static IEnumerator AnimateFillAmount(Image image, float target, float duration = 0.3f)
    {
        float elapsed = 0f;
        float start = image.fillAmount;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, elapsed / duration);
            image.fillAmount = Mathf.Lerp(start, target, t);
            yield return null;
        }

        image.fillAmount = target;
    }
}