using Unity.VisualScripting;
using UnityEngine;

public class MoistureNoiseGenerator : NoiseGenerator
{
    private float baseMoisture;

    public MoistureNoiseGenerator(MoistureMapSettings settings) : base(settings)
    {
        this.baseMoisture = settings.baseMoisture;
    }


    
    public override float GetNormalizedNoiseValue(int x, int y, int size){
        float normalizedValue = Mathf.InverseLerp(base.cache.minValue, base.cache.maxValue, GetNoiseValue(x, y, size));

        if(normalizedValue > 1) Debug.Log("Noise value > 1 in MoistureNoiseGenerator");
        if(normalizedValue < 0) Debug.Log("Noise value < 0 in MoistureNoiseGenerator");

        return normalizedValue;
    }

    public override float GetNoiseValue(int x, int y, int size)
    {
        
        float rawNoise = base.GetNoiseValue(x, y, size);
        float finalValue =  rawNoise + baseMoisture;

        return finalValue;
    }

    public NoiseCache GetCache(){
        return base.cache;
    }
}
