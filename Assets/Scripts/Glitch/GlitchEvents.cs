using System;
using UnityEngine;

public static class GlitchEvents
{
    public static event Action<GlitchType, float> Triggered;

    static float activeUntil;
    static GlitchType activeType;

    public static bool IsActive => Time.time < activeUntil;
    public static GlitchType ActiveType => activeType;

    public static void Raise(GlitchType type, float duration)
    {
        activeType = type;
        if (duration > 0f) activeUntil = Time.time + duration;
        Triggered?.Invoke(type, duration);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Triggered = null;
        activeUntil = 0f;
    }
}
