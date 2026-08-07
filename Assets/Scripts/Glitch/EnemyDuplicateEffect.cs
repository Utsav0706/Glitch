using System.Collections.Generic;
using UnityEngine;

public class EnemyDuplicateEffect : MonoBehaviour
{
    public Vector3 offset = new Vector3(1.4f, 0f, 0f);
    public Color ghostTint = new Color(0.82f, 0.9f, 1f);

    readonly List<GameObject> ghosts = new List<GameObject>();
    bool active;
    float endTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<EnemyDuplicateEffect>() == null)
            new GameObject("EnemyDuplicateEffect").AddComponent<EnemyDuplicateEffect>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }

    void OnDisable()
    {
        GlitchEvents.Triggered -= OnGlitch;
        if (active) Clear();
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (type != GlitchType.EnemyDuplicate || active) return;
        Begin(duration);
    }

    void Begin(float duration)
    {
        active = true;
        endTime = Time.time + duration;
        ghosts.Clear();

        foreach (EnemyBase e in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (e == null || e.IsDead) continue;
            GameObject g = MakeGhost(e);
            if (g != null) ghosts.Add(g);
        }
    }

    GameObject MakeGhost(EnemyBase enemy)
    {
        GameObject ghost = new GameObject("EnemyGhost");
        ghost.transform.SetPositionAndRotation(
            enemy.transform.position + enemy.transform.TransformDirection(offset),
            enemy.transform.rotation);

        Transform model = enemy.transform.Find("Model");
        if (model != null)
        {
            GameObject vis = Instantiate(model.gameObject, ghost.transform);
            vis.name = "Model";
            vis.transform.localPosition = model.localPosition;
            vis.transform.localRotation = model.localRotation;
            vis.transform.localScale = model.localScale;
            Tint(vis);
        }

        CapsuleCollider src = enemy.GetComponent<CapsuleCollider>();
        CapsuleCollider col = ghost.AddComponent<CapsuleCollider>();
        if (src != null)
        {
            col.center = src.center;
            col.height = src.height;
            col.radius = src.radius;
        }

        Health h = ghost.AddComponent<Health>();
        Ghost g = ghost.AddComponent<Ghost>();
        g.Init(h);

        return ghost;
    }

    void Tint(GameObject vis)
    {
        MaterialPropertyBlock mpb = new MaterialPropertyBlock();
        foreach (Renderer r in vis.GetComponentsInChildren<Renderer>())
        {
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor", ghostTint);
            mpb.SetColor("_Color", ghostTint);
            r.SetPropertyBlock(mpb);
        }
    }

    void Update()
    {
        if (active && Time.time >= endTime) Clear();
    }

    void Clear()
    {
        active = false;
        for (int i = 0; i < ghosts.Count; i++)
            if (ghosts[i] != null) Destroy(ghosts[i]);
        ghosts.Clear();
    }
}
