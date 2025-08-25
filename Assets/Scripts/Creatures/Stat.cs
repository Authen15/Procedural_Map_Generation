using System;
using UnityEngine.Events;

[Serializable]
public class Stat
{
    public UnityEvent<float> OnValueChanged;

    public float BaseValue = 0f;
    private float _flatBonus = 0f;
    private float _multiplierBonus = 0f;

    public float Value => BaseValue * (1f + _multiplierBonus) + _flatBonus;

    public void AddFlatBonus(float amount)
    {
        _flatBonus += amount;
        OnValueChanged?.Invoke(Value);
    }

    public void AddMultiplierBonus(float amount)
    {
        _multiplierBonus += amount;
        OnValueChanged?.Invoke(Value);
    }
}
