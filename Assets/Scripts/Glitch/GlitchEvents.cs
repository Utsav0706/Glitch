using System;
using UnityEngine;

public static class GlitchEvents
{
    public static event Action<GlitchType, float> Triggered;

    public static void Raise(GlitchType type, float duration)
    {
        Triggered?.Invoke(type, duration);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void ResetStatics()
    {
        Triggered = null;
    }
}
