using UnityEngine;

public class HeightNoiseGenerator : NoiseGenerator
{
    private AnimationCurve heightCurve;
    private float baseHeight;

    public HeightNoiseGenerator(HeightMapSettings settings) : base(settings)
    {
        this.heightCurve = settings.heightCurve;
        this.baseHeight = settings.baseHeight;
    }

    public override float GetNormalizedNoiseValue(int x, int y, int size){
        float normalizedValue = Mathf.InverseLerp(base.cache.minValue, base.cache.maxValue, GetNoiseValue(x, y, size));

        if(normalizedValue > 1) Debug.Log("Noise value > 1 in MoistureNoiseGenerator");
        if(normalizedValue < 0) Debug.Log("Noise value < 0 in MoistureNoiseGenerator");

        AnimationCurve heightCurve_threadsafe = new AnimationCurve (heightCurve.keys);
        normalizedValue = heightCurve_threadsafe.Evaluate(normalizedValue); // height curve bound must be < 0 and > 1

        return normalizedValue;
    }


    public override float GetNoiseValue(int x, int y, int size)
    {
        float rawNoise = base.GetNoiseValue(x, y, size);
        float finalValue = rawNoise + baseHeight;

        return finalValue;
    }

    public NoiseCache GetCache(){
        return base.cache;
    }

}
