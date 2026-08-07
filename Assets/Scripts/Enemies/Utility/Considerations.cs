using UnityEngine;

public static class Considerations
{
    public static float Linear(float x) => Mathf.Clamp01(x);

    public static float Inverse(float x) => 1f - Mathf.Clamp01(x);

    public static float Curve(float x, float exponent) => Mathf.Pow(Mathf.Clamp01(x), Mathf.Max(0.0001f, exponent));

    public static float Band(float x, float lo, float hi) => (x >= lo && x <= hi) ? 1f : 0f;

    public static float Logistic(float x, float mid, float steepness) => 1f / (1f + Mathf.Exp(-steepness * (x - mid)));

    public static float Closeness(float distance, float maxDistance)
    {
        if (maxDistance <= 0f) return 0f;
        return Inverse(distance / maxDistance);
    }

    public static float InRange(float distance, float idealRange, float falloff)
    {
        float d = Mathf.Abs(distance - idealRange) / Mathf.Max(0.0001f, falloff);
        return Inverse(d);
    }

    public static float Hurt(float healthNormalized) => Inverse(healthNormalized);

    public static float Healthy(float healthNormalized) => Linear(healthNormalized);

    public static float GlitchBonus(bool glitchActive, float weight) => glitchActive ? Mathf.Max(0f, weight) : 0f;

    public static float Exposure(bool exposedToThreat, float distance, float maxDistance)
    {
        return exposedToThreat ? Closeness(distance, maxDistance) : 0f;
    }

    public static float TargetCertainty(int copies)
    {
        return copies <= 1 ? 1f : 1f / copies;
    }

    public static float Product(params float[] values)
    {
        float result = 1f;
        for (int i = 0; i < values.Length; i++) result *= Mathf.Clamp01(values[i]);
        return result;
    }
}
