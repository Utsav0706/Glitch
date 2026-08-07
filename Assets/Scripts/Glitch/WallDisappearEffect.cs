using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(1000)]
public class WallDisappearEffect : MonoBehaviour
{
    public string[] wallGroups = { "Perimeter", "Building" };
    public Color transparentColor = new Color(0.45f, 0.7f, 1f, 0.16f);

    struct Swap { public Renderer renderer; public Material[] originalMats; }
    struct Trig { public Collider collider; public bool wasTrigger; }

    readonly List<Swap> swaps = new List<Swap>();
    readonly List<Trig> trigs = new List<Trig>();
    Material transMat;
    bool active;
    float endTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<WallDisappearEffect>() == null)
            new GameObject("WallDisappearEffect").AddComponent<WallDisappearEffect>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }

    void OnDisable()
    {
        GlitchEvents.Triggered -= OnGlitch;
        if (active) Restore();
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (type != GlitchType.WallDisappear || active) return;
        Begin(duration);
    }

    void Begin(float duration)
    {
        active = true;
        endTime = Time.time + duration;
        swaps.Clear();
        trigs.Clear();

        Material mat = TransparentMaterial();

        foreach (Transform group in WallGroups())
        {
            foreach (Renderer r in group.GetComponentsInChildren<Renderer>())
            {
                swaps.Add(new Swap { renderer = r, originalMats = r.sharedMaterials });
                Material[] repl = new Material[r.sharedMaterials.Length];
                for (int i = 0; i < repl.Length; i++) repl[i] = mat;
                r.sharedMaterials = repl;
            }

            foreach (Collider c in group.GetComponentsInChildren<Collider>())
            {
                trigs.Add(new Trig { collider = c, wasTrigger = c.isTrigger });
                c.isTrigger = true;
            }
        }
    }

    void Restore()
    {
        active = false;

        for (int i = 0; i < swaps.Count; i++)
            if (swaps[i].renderer != null) swaps[i].renderer.sharedMaterials = swaps[i].originalMats;

        for (int i = 0; i < trigs.Count; i++)
            if (trigs[i].collider != null) trigs[i].collider.isTrigger = trigs[i].wasTrigger;

        swaps.Clear();
        trigs.Clear();
    }

    void Update()
    {
        if (active && Time.time >= endTime) Restore();
    }

    List<Transform> WallGroups()
    {
        List<Transform> list = new List<Transform>();
        GameObject arena = GameObject.Find("Arena");
        if (arena == null) return list;

        foreach (string name in wallGroups)
        {
            Transform g = arena.transform.Find(name);
            if (g != null) list.Add(g);
        }
        return list;
    }

    Material TransparentMaterial()
    {
        if (transMat != null) return transMat;

        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        transMat = new Material(sh);
        transMat.SetFloat("_Surface", 1f);
        transMat.SetFloat("_Blend", 0f);
        transMat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
        transMat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        transMat.SetFloat("_ZWrite", 0f);
        transMat.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");
        transMat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;
        transMat.color = transparentColor;
        transMat.SetColor("_BaseColor", transparentColor);
        return transMat;
    }
}
