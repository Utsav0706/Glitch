using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public bool systemGlitches = true;
    public float minInterval = 20f;
    public float maxInterval = 40f;
    public float systemDuration = 5f;
    public GlitchType[] systemPool = { GlitchType.GravityFlip, GlitchType.EnemyDuplicate, GlitchType.WallDisappear };

    public KeyCode timeStutterKey = KeyCode.Q;
    public KeyCode playerDuplicateKey = KeyCode.E;
    public float timeStutterDuration = 3f;
    public float playerDuplicateDuration = 5f;
    public float timeStutterCooldown = 12f;
    public float playerDuplicateCooldown = 15f;

    public float warningDuration = 2.5f;
    public float flashDuration = 0.4f;

    float nextSystem;
    float stutterReady;
    float duplicateReady;

    GlitchType lastGlitch;
    float warnUntil;
    float flashUntil;

    GUIStyle banner;
    GUIStyle small;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<GlitchManager>() == null)
            new GameObject("GlitchManager").AddComponent<GlitchManager>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }
    void OnDisable() { GlitchEvents.Triggered -= OnGlitch; }

    void Start()
    {
        ScheduleNext();
    }

    void Update()
    {
        if (systemGlitches && Time.time >= nextSystem)
        {
            if (GlitchEvents.IsActive)
                nextSystem = Time.time + 1f;
            else
            {
                FireSystemGlitch();
                ScheduleNext();
            }
        }

        if (Input.GetKeyDown(timeStutterKey) && Time.time >= stutterReady && !GlitchEvents.IsActive)
        {
            stutterReady = Time.time + timeStutterCooldown;
            GlitchEvents.Raise(GlitchType.TimeStutter, timeStutterDuration);
        }

        if (Input.GetKeyDown(playerDuplicateKey) && Time.time >= duplicateReady && !GlitchEvents.IsActive)
        {
            duplicateReady = Time.time + playerDuplicateCooldown;
            GlitchEvents.Raise(GlitchType.PlayerDuplicate, playerDuplicateDuration);
        }
    }

    void ScheduleNext()
    {
        nextSystem = Time.time + Random.Range(minInterval, maxInterval);
    }

    void FireSystemGlitch()
    {
        if (systemPool == null || systemPool.Length == 0) return;
        GlitchEvents.Raise(systemPool[Random.Range(0, systemPool.Length)], systemDuration);
    }

    void OnGlitch(GlitchType type, float duration)
    {
        lastGlitch = type;
        warnUntil = Time.time + warningDuration;
        flashUntil = Time.time + flashDuration;
    }

    void OnGUI()
    {
        EnsureStyles();

        if (Time.time < flashUntil)
        {
            float t = (flashUntil - Time.time) / flashDuration;
            Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.62f, 0.12f, 0.9f, 0.4f * t));
        }
        else if (GlitchEvents.IsActive)
        {
            Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.5f, 0.12f, 0.85f, 0.07f));
        }

        if (Time.time < warnUntil)
        {
            float w = 460f;
            Rect r = new Rect((Screen.width - w) * 0.5f, 44f, w, 36f);
            Fill(r, new Color(0f, 0f, 0f, 0.72f));
            Frame(r, new Color(0.9f, 0.3f, 1f, 0.9f));
            GUI.Label(r, "!!  GLITCH:  " + Readable(lastGlitch), banner);
        }

        DrawAbility(timeStutterKey, "Time Stutter", stutterReady, timeStutterCooldown, 0);
        DrawAbility(playerDuplicateKey, "Player Duplicate", duplicateReady, playerDuplicateCooldown, 1);
    }

    void DrawAbility(KeyCode key, string label, float readyAt, float cooldown, int slot)
    {
        float w = 200f;
        float h = 24f;
        float x = Screen.width * 0.5f - w - 8f + slot * (w + 16f);
        float y = Screen.height - 40f;
        Rect r = new Rect(x, y, w, h);
        Fill(r, new Color(0f, 0f, 0f, 0.6f));

        float remain = readyAt - Time.time;
        bool ready = remain <= 0f;
        if (ready)
            Fill(r, new Color(0.3f, 0.8f, 0.4f, 0.22f));
        else
            Fill(new Rect(r.x, r.y, r.width * Mathf.Clamp01(1f - remain / Mathf.Max(0.01f, cooldown)), r.height), new Color(0.6f, 0.2f, 0.9f, 0.5f));

        Frame(r, new Color(1f, 1f, 1f, 0.25f));
        GUI.Label(new Rect(r.x + 6f, r.y + 2f, r.width, r.height), "[" + key + "]  " + label + (ready ? "  READY" : "  " + Mathf.CeilToInt(remain) + "s"), small);
    }

    string Readable(GlitchType t)
    {
        switch (t)
        {
            case GlitchType.GravityFlip: return "GRAVITY FLIP";
            case GlitchType.EnemyDuplicate: return "ENEMY DUPLICATE";
            case GlitchType.WallDisappear: return "WALL DISAPPEAR";
            case GlitchType.TimeStutter: return "TIME STUTTER";
            case GlitchType.PlayerDuplicate: return "PLAYER DUPLICATE";
            default: return t.ToString();
        }
    }

    void EnsureStyles()
    {
        if (banner != null) return;
        banner = new GUIStyle(GUI.skin.label) { fontSize = 16, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        banner.normal.textColor = new Color(1f, 0.85f, 1f);
        small = new GUIStyle(GUI.skin.label) { fontSize = 12, fontStyle = FontStyle.Bold };
        small.normal.textColor = Color.white;
    }

    void Fill(Rect r, Color c)
    {
        Color prev = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = prev;
    }

    void Frame(Rect r, Color c)
    {
        Fill(new Rect(r.x, r.y, r.width, 1f), c);
        Fill(new Rect(r.x, r.yMax - 1f, r.width, 1f), c);
        Fill(new Rect(r.x, r.y, 1f, r.height), c);
        Fill(new Rect(r.xMax - 1f, r.y, 1f, r.height), c);
    }
}
