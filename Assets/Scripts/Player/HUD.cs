using UnityEngine;

public class HUD : MonoBehaviour
{
    public Health health;
    public PlayerShoot shoot;

    GUIStyle label;

    void EnsureStyle()
    {
        if (label != null) return;
        label = new GUIStyle(GUI.skin.label);
        label.fontSize = 14;
        label.fontStyle = FontStyle.Bold;
        label.normal.textColor = Color.white;
    }

    void OnGUI()
    {
        EnsureStyle();

        float pad = 20f;
        float barW = 240f;
        float barH = 18f;
        float yHealth = Screen.height - pad - barH;
        float yGlitch = yHealth - 48f;

        if (health != null)
        {
            GUI.Label(new Rect(pad, yHealth - 20f, barW, 18f), "HEALTH", label);
            Rect r = new Rect(pad, yHealth, barW, barH);
            Fill(r, 1f, new Color(0f, 0f, 0f, 0.6f));
            Color fg = Color.Lerp(new Color(0.85f, 0.2f, 0.2f), new Color(0.3f, 0.85f, 0.35f), health.Normalized);
            Fill(r, health.Normalized, fg);
            Frame(r, new Color(1f, 1f, 1f, 0.5f));
            GUI.Label(new Rect(r.x + 8f, r.y, barW, barH), Mathf.CeilToInt(health.Current) + " / " + Mathf.CeilToInt(health.Max), label);
        }

        GUI.Label(new Rect(pad, yGlitch - 20f, barW, 18f), "GLITCH", label);
        Rect g = new Rect(pad, yGlitch, barW, barH);
        Fill(g, 1f, new Color(0f, 0f, 0f, 0.6f));
        Frame(g, new Color(0.8f, 0.2f, 0.9f, 0.7f));
        GUI.Label(new Rect(g.x + 8f, g.y, barW, barH), "placeholder", label);

        if (shoot != null)
        {
            string text = shoot.IsReloading ? "RELOADING" : "AMMO  " + shoot.Ammo + " / " + shoot.MaxAmmo;
            Rect a = new Rect(Screen.width - pad - 180f, yHealth, 180f, barH);
            Fill(a, 1f, new Color(0f, 0f, 0f, 0.6f));
            Frame(a, new Color(1f, 1f, 1f, 0.5f));
            GUI.Label(new Rect(a.x + 8f, a.y, a.width, a.height), text, label);
        }
    }

    void Fill(Rect r, float t, Color c)
    {
        Draw(new Rect(r.x, r.y, r.width * Mathf.Clamp01(t), r.height), c);
    }

    void Frame(Rect r, Color c)
    {
        Draw(new Rect(r.x, r.y, r.width, 1f), c);
        Draw(new Rect(r.x, r.yMax - 1f, r.width, 1f), c);
        Draw(new Rect(r.x, r.y, 1f, r.height), c);
        Draw(new Rect(r.xMax - 1f, r.y, 1f, r.height), c);
    }

    void Draw(Rect r, Color c)
    {
        Color prev = GUI.color;
        GUI.color = c;
        GUI.DrawTexture(r, Texture2D.whiteTexture);
        GUI.color = prev;
    }
}
