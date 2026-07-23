using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public float duration = 0.05f;

    Light flashLight;
    Renderer flashRenderer;
    float offAt;
    bool on;

    void Awake()
    {
        flashLight = GetComponentInChildren<Light>(true);
        flashRenderer = GetComponentInChildren<Renderer>(true);
        Toggle(false);
    }

    void Update()
    {
        if (on && Time.time >= offAt) Toggle(false);
    }

    public void Flash()
    {
        offAt = Time.time + duration;
        Toggle(true);
    }

    void Toggle(bool state)
    {
        on = state;
        if (flashLight != null) flashLight.enabled = state;
        if (flashRenderer != null) flashRenderer.enabled = state;
    }
}
