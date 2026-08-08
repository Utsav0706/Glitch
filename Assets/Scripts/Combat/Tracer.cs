using System.Collections.Generic;
using UnityEngine;

public class Tracer : MonoBehaviour
{
    public float speed = 140f;
    public float maxLife = 1.5f;

    static readonly Dictionary<Color, Material> coreMats = new Dictionary<Color, Material>();
    static Material trailMat;

    Vector3 target;
    float dieAt;
    Renderer core;
    bool arrived;

    public static void Spawn(Vector3 from, Vector3 to, Color color)
    {
        color.a = 1f;

        GameObject go = new GameObject("Tracer");
        go.transform.position = from;

        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Collider col = sphere.GetComponent<Collider>();
        if (col != null) Destroy(col);
        sphere.transform.SetParent(go.transform, false);
        sphere.transform.localScale = Vector3.one * 0.2f;
        Renderer sr = sphere.GetComponent<Renderer>();
        sr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        sr.sharedMaterial = CoreMaterial(color);

        TrailRenderer trail = go.AddComponent<TrailRenderer>();
        trail.time = 0.09f;
        trail.startWidth = 0.18f;
        trail.endWidth = 0.02f;
        trail.numCapVertices = 3;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.material = TrailMaterial();
        trail.startColor = color;
        trail.endColor = new Color(color.r, color.g, color.b, 0f);

        Tracer t = go.AddComponent<Tracer>();
        t.target = to;
        t.dieAt = Time.time + t.maxLife;
        t.core = sr;
    }

    void Update()
    {
        if (!arrived)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if ((transform.position - target).sqrMagnitude < 0.02f)
            {
                arrived = true;
                if (core != null) core.enabled = false;
                Destroy(gameObject, 0.12f);
            }
        }

        if (Time.time >= dieAt) Destroy(gameObject);
    }

    static Material CoreMaterial(Color color)
    {
        if (coreMats.TryGetValue(color, out Material cached) && cached != null) return cached;

        Shader sh = Shader.Find("Universal Render Pipeline/Unlit");
        if (sh == null) sh = Shader.Find("Unlit/Color");
        if (sh == null) sh = Shader.Find("Sprites/Default");

        Material m = new Material(sh);
        m.color = color;
        if (m.HasProperty("_BaseColor")) m.SetColor("_BaseColor", color);
        if (m.HasProperty("_EmissionColor"))
        {
            m.EnableKeyword("_EMISSION");
            m.SetColor("_EmissionColor", color * 3f);
        }

        coreMats[color] = m;
        return m;
    }

    static Material TrailMaterial()
    {
        if (trailMat != null) return trailMat;

        Shader sh = Shader.Find("Sprites/Default");
        if (sh == null) sh = Shader.Find("Universal Render Pipeline/Unlit");
        trailMat = new Material(sh);
        return trailMat;
    }
}
