using UnityEngine;

public class GlitchTester : MonoBehaviour
{
    public float duration = 5f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) GlitchEvents.Raise(GlitchType.GravityFlip, duration);
        if (Input.GetKeyDown(KeyCode.Alpha2)) GlitchEvents.Raise(GlitchType.EnemyDuplicate, duration);
        if (Input.GetKeyDown(KeyCode.Alpha3)) GlitchEvents.Raise(GlitchType.WallDisappear, duration);
        if (Input.GetKeyDown(KeyCode.Alpha4)) GlitchEvents.Raise(GlitchType.TimeStutter, duration);
        if (Input.GetKeyDown(KeyCode.Alpha5)) GlitchEvents.Raise(GlitchType.PlayerDuplicate, duration);
    }
}
