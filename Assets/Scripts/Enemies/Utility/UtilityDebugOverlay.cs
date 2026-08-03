using UnityEngine;

public class UtilityDebugOverlay : MonoBehaviour
{
    public bool show = true;
    public KeyCode toggleKey = KeyCode.F5;
    public float headHeight = 2.6f;
    public float maxDistance = 70f;
    public float width = 172f;

    Camera cam;
    GUIStyle label;
    GUIStyle winner;
    GUIStyle header;

    void Update()
    {
        if (Input.GetKeyDown(toggleKey)) show = !show;
    }

    void OnGUI()
    {
        if (!show) return;
        if (cam == null) cam = Camera.main;
        if (cam == null) return;
        EnsureStyles();

        var list = UtilityEnemy.All;
        for (int i = 0; i < list.Count; i++) Draw(list[i]);
    }

    void Draw(UtilityEnemy e)
    {
        if (e == null || e.Brain == null || e.IsDead) return;

        Vector3 head = e.transform.position + Vector3.up * headHeight;
        Vector3 sp = cam.WorldToScreenPoint(head);
        if (sp.z <= 0f || sp.z > maxDistance) return;

        var actions = e.Brain.Actions;
        int n = actions.Count;
        float rowH = 15f;
        float headH = 18f;
        float pad = 6f;
        float h = headH + n * rowH + pad;
        Rect box = new Rect(sp.x - width * 0.5f, Screen.height - sp.y - h, width, h);

        Fill(box, new Color(0.05f, 0.05f, 0.07f, 0.82f));
        Frame(box, new Color(1f, 1f, 1f, 0.16f));

        float max = 0.0001f;
        for (int i = 0; i < n; i++) if (actions[i].LastScore > max) max = actions[i].LastScore;

        bool engaged = e.Brain.Current != null && e.Brain.Current.LastScore > 0.01f;
        GUI.Label(new Rect(box.x + pad, box.y + 2f, width, headH), engaged ? Short(e.Brain.CurrentName) : "IDLE", header);

        for (int i = 0; i < n; i++)
        {
            var a = actions[i];
            bool win = engaged && a == e.Brain.Current;
            float ry = box.y + headH + i * rowH;

            Rect barBg = new Rect(box.x + 62f, ry + 3f, width - 100f, rowH - 6f);
            Fill(barBg, new Color(1f, 1f, 1f, 0.08f));
            Rect fillR = new Rect(barBg.x, barBg.y, barBg.width * Mathf.Clamp01(a.LastScore / max), barBg.height);
            Fill(fillR, win ? new Color(0.3f, 0.92f, 0.42f, 0.95f) : new Color(0.45f, 0.68f, 1f, 0.75f));

            GUI.Label(new Rect(box.x + pad, ry, 58f, rowH), Short(a.Name), win ? winner : label);
            GUI.Label(new Rect(box.xMax - 36f, ry, 32f, rowH), a.LastScore.ToString("0.0"), win ? winner : label);
        }
    }

    string Short(string s) => s.Replace("Action", "");

    void EnsureStyles()
    {
        if (label != null) return;
        label = new GUIStyle(GUI.skin.label) { fontSize = 10 };
        label.normal.textColor = new Color(0.85f, 0.88f, 0.92f);
        winner = new GUIStyle(label) { fontStyle = FontStyle.Bold };
        winner.normal.textColor = new Color(0.4f, 1f, 0.5f);
        header = new GUIStyle(GUI.skin.label) { fontSize = 11, fontStyle = FontStyle.Bold };
        header.normal.textColor = Color.white;
    }

    void Fill(Rect r, Color c) => Draw(r, c);

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
