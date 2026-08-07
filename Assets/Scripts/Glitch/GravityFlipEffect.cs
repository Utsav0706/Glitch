using UnityEngine;

[DefaultExecutionOrder(1000)]
public class GravityFlipEffect : MonoBehaviour
{
    public Vector3 indoorCenter = Vector3.zero;
    public Vector2 indoorHalfExtents = new Vector2(30f, 30f);
    public float indoorRoofHeight = 5.5f;
    public float liftImpulse = 5f;
    public float cameraRoll = 180f;
    public float rollSpeed = 360f;

    Vector3 originalGravity;
    bool active;
    float endTime;
    Transform cam;
    float roll;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<GravityFlipEffect>() == null)
            new GameObject("GravityFlipEffect").AddComponent<GravityFlipEffect>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }

    void OnDisable()
    {
        GlitchEvents.Triggered -= OnGlitch;
        if (active) End();
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (type != GlitchType.GravityFlip || active) return;

        Transform player = FindPlayer();
        if (player == null || !Indoors(player.position)) return;

        Begin(duration);
    }

    void Begin(float duration)
    {
        active = true;
        endTime = Time.time + duration;

        originalGravity = Physics.gravity;
        Physics.gravity = new Vector3(originalGravity.x, -originalGravity.y, originalGravity.z);

        foreach (Rigidbody rb in FindObjectsByType<Rigidbody>(FindObjectsSortMode.None))
        {
            if (rb.isKinematic || !Indoors(rb.position)) continue;
            rb.AddForce(Vector3.up * liftImpulse, ForceMode.VelocityChange);
        }
    }

    void End()
    {
        active = false;
        Physics.gravity = originalGravity;
    }

    void Update()
    {
        if (active && Time.time >= endTime) End();
    }

    void LateUpdate()
    {
        if (cam == null && Camera.main != null) cam = Camera.main.transform;

        float target = active ? cameraRoll : 0f;
        roll = Mathf.MoveTowards(roll, target, rollSpeed * Time.deltaTime);

        if (cam != null && Mathf.Abs(roll) > 0.01f)
            cam.rotation = cam.rotation * Quaternion.Euler(0f, 0f, roll);
    }

    bool Indoors(Vector3 p)
    {
        return Mathf.Abs(p.x - indoorCenter.x) <= indoorHalfExtents.x
            && Mathf.Abs(p.z - indoorCenter.z) <= indoorHalfExtents.y
            && p.y <= indoorCenter.y + indoorRoofHeight;
    }

    Transform FindPlayer()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        return go != null ? go.transform : null;
    }
}
