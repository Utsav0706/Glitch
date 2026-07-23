using UnityEngine;

public class GlitchTester : MonoBehaviour
{
    public float duration = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GlitchEvents.Raise(GlitchType.GravityFlip, duration);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GlitchEvents.Raise(GlitchType.WallDisappear, duration);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GlitchEvents.Raise(GlitchType.Blackout, duration);
        if (Input.GetKeyDown(KeyCode.Alpha4)) GlitchEvents.Raise(GlitchType.TimeDilation, duration);
    }
}
