using UnityEngine;

public class TemperatureNoiseGenerator : NoiseGenerator
{
    private float latitudeSensitivity;
    private float equatorBias;
    private float elevationSensitivity;
    private float baseTemperature;

    public TemperatureNoiseGenerator(TemperatureMapSettings settings) : base(settings)
    {
        this.latitudeSensitivity = settings.latitudeSensitivity;
        this.equatorBias = settings.equatorBias;
        this.elevationSensitivity = settings.elevationSensitivity;
        this.baseTemperature = settings.baseTemperature;
    }

    public override float GetNormalizedNoiseValue(int x, int y, int size){
        float normalizedValue = Mathf.InverseLerp(base.cache.minValue, base.cache.maxValue, GetNoiseValue(x, y, size));

        if(normalizedValue > 1) Debug.Log("Noise value > 1 in MoistureNoiseGenerator");
        if(normalizedValue < 0) Debug.Log("Noise value < 0 in MoistureNoiseGenerator");

        return normalizedValue;
    }


    public override float GetNoiseValue(int x, int y, int size)
    {
        float rawNoise = base.GetNoiseValue(x, y, size); // represent the height value

        float latitudeEffect = Mathf.Abs((float)y / size - 0.5f) * latitudeSensitivity;
        float latitudeAdjustedNoise = (1 - latitudeEffect) + equatorBias;

        latitudeAdjustedNoise -= rawNoise * elevationSensitivity;

        float finalValue = latitudeAdjustedNoise + rawNoise * baseTemperature;

        return finalValue;
    }

    public NoiseCache GetCache(){
        return base.cache;
    }
}
