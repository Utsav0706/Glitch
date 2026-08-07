using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DefaultExecutionOrder(1000)]
public class TimeStutterEffect : MonoBehaviour
{
    public Color veilColor = new Color(0.72f, 0.75f, 0.8f);
    public float veilAlpha = 0.3f;

    struct Held
    {
        public EnemyBase enemy;
        public NavMeshAgent agent;
        public Animator anim;
        public float animSpeed;
    }

    readonly List<Held> held = new List<Held>();
    bool active;
    float endTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<TimeStutterEffect>() == null)
            new GameObject("TimeStutterEffect").AddComponent<TimeStutterEffect>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }

    void OnDisable()
    {
        GlitchEvents.Triggered -= OnGlitch;
        if (active) Release();
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (type != GlitchType.TimeStutter || active) return;
        Begin(duration);
    }

    void Begin(float duration)
    {
        active = true;
        endTime = Time.time + duration;
        held.Clear();

        foreach (EnemyBase e in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (e == null || e.IsDead) continue;

            e.Freeze(duration);

            NavMeshAgent agent = e.Agent;
            if (agent != null && agent.enabled && agent.isOnNavMesh) agent.isStopped = true;

            Animator anim = e.GetComponentInChildren<Animator>();
            float speed = anim != null ? anim.speed : 1f;
            if (anim != null) anim.speed = 0f;

            held.Add(new Held { enemy = e, agent = agent, anim = anim, animSpeed = speed });
        }
    }

    void Release()
    {
        active = false;
        for (int i = 0; i < held.Count; i++)
        {
            Held h = held[i];
            if (h.enemy != null) h.enemy.Unfreeze();
            if (h.agent != null && h.agent.enabled && h.agent.isOnNavMesh) h.agent.isStopped = false;
            if (h.anim != null) h.anim.speed = h.animSpeed;
        }
        held.Clear();
    }

    void Update()
    {
        if (active && Time.time >= endTime) Release();
    }

    void OnGUI()
    {
        if (!active) return;

        float flicker = 0.05f * Mathf.PerlinNoise(Time.unscaledTime * 24f, 0.7f);
        Color veil = veilColor;
        veil.a = veilAlpha + flicker;
        Fill(new Rect(0f, 0f, Screen.width, Screen.height), veil);

        Color line = new Color(0f, 0f, 0f, 0.1f);
        int offset = (int)(Time.unscaledTime * 40f) % 4;
        for (int y = offset; y < Screen.height; y += 4)
            Fill(new Rect(0f, y, Screen.width, 1f), line);
    }

    void Fill(Rect r, Color c)
    {
        Color prev = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = prev;
    }
}
