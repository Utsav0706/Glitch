using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverController : MonoBehaviour
{
    public float slomoScale = 0.25f;
    public float slomoRealSeconds = 1.2f;

    Health playerHealth;
    bool dying;
    bool showMenu;
    float deathRealtime;
    float menuRealtime;
    float originalFixed = 0.02f;

    GUIStyle titleStyle;
    GUIStyle hintStyle;
    GUIStyle buttonStyle;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<GameOverController>() == null)
            new GameObject("GameOverController").AddComponent<GameOverController>();
    }

    void Update()
    {
        if (playerHealth == null && !dying)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                playerHealth = p.GetComponent<Health>();
                if (playerHealth != null) playerHealth.Died += OnPlayerDied;
            }
        }

        if (dying && !showMenu && Time.realtimeSinceStartup >= menuRealtime)
        {
            showMenu = true;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    void OnDestroy()
    {
        if (playerHealth != null) playerHealth.Died -= OnPlayerDied;
    }

    void OnPlayerDied()
    {
        if (dying) return;
        dying = true;
        deathRealtime = Time.realtimeSinceStartup;
        menuRealtime = deathRealtime + slomoRealSeconds;
        originalFixed = Time.fixedDeltaTime;
        Time.timeScale = slomoScale;
        Time.fixedDeltaTime = originalFixed * slomoScale;
    }

    void OnGUI()
    {
        if (!dying) return;
        EnsureStyles();

        float k = showMenu ? 1f : Mathf.Clamp01((Time.realtimeSinceStartup - deathRealtime) / Mathf.Max(0.01f, slomoRealSeconds));
        Fill(new Rect(0f, 0f, Screen.width, Screen.height), new Color(0.02f, 0f, 0.03f, Mathf.Lerp(0.12f, 0.82f, k)));

        if (!showMenu) return;

        GUI.Label(new Rect(0f, Screen.height * 0.24f, Screen.width, 90f), "YOU DIED", titleStyle);
        GUI.Label(new Rect(0f, Screen.height * 0.24f + 92f, Screen.width, 30f), "the glitch got you", hintStyle);

        float bw = 280f, bh = 60f, gap = 22f;
        float cx = Screen.width * 0.5f;
        float y = Screen.height * 0.5f;

        Color prevBg = GUI.backgroundColor;

        GUI.backgroundColor = new Color(0.24f, 0.68f, 0.36f, 0.95f);
        if (GUI.Button(new Rect(cx - bw * 0.5f, y, bw, bh), "RESTART", buttonStyle))
            Restart();

        GUI.backgroundColor = new Color(0.82f, 0.26f, 0.26f, 0.95f);
        if (GUI.Button(new Rect(cx - bw * 0.5f, y + bh + gap, bw, bh), "QUIT", buttonStyle))
            Quit();

        GUI.backgroundColor = prevBg;
    }

    void Restart()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixed;
        Scene s = SceneManager.GetActiveScene();
        SceneManager.LoadScene(s.buildIndex >= 0 ? s.buildIndex : 0);
    }

    void Quit()
    {
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixed;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void EnsureStyles()
    {
        if (titleStyle != null) return;

        titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 56, fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
        titleStyle.normal.textColor = new Color(1f, 0.28f, 0.28f);

        hintStyle = new GUIStyle(GUI.skin.label) { fontSize = 16, alignment = TextAnchor.MiddleCenter };
        hintStyle.normal.textColor = new Color(0.85f, 0.85f, 0.9f);

        buttonStyle = new GUIStyle(GUI.skin.button) { fontSize = 22, fontStyle = FontStyle.Bold };
        buttonStyle.normal.textColor = Color.white;
        buttonStyle.hover.textColor = Color.white;
        buttonStyle.active.textColor = Color.white;
    }

    void Fill(Rect r, Color c)
    {
        Color prev = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = prev;
    }
}
