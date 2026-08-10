using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public float sampleInterval = 0.5f;

    const string Header = "time,enemyId,aiType,action,activeGlitch,health,distToPlayer,canSeePlayer";

    StreamWriter writer;
    string path;
    float startTime;
    float nextSample;
    Transform player;
    Health playerHealth;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<DataLogger>() == null)
            new GameObject("DataLogger").AddComponent<DataLogger>();
    }

    void Start()
    {
        startTime = Time.time;
        OpenFile();
    }

    void OpenFile()
    {
        try
        {
            string dir = Path.GetFullPath(Path.Combine(Application.dataPath, "..", "SessionLogs"));
            Directory.CreateDirectory(dir);
            path = Path.Combine(dir, "session_" + DateTime.Now.ToString("yyyyMMdd_HHmmss_fff") + ".csv");
            writer = new StreamWriter(path, false, Encoding.UTF8);
            writer.WriteLine(Header);
            writer.Flush();
            Debug.Log("[DataLogger] Session CSV: " + path);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("[DataLogger] Could not open log file: " + ex.Message);
            writer = null;
        }
    }

    void Update()
    {
        if (writer == null || Time.time < nextSample) return;
        nextSample = Time.time + sampleInterval;

        if (player == null)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null)
            {
                player = p.transform;
                playerHealth = p.GetComponent<Health>();
            }
        }

        float t = Time.time - startTime;
        string glitch = GlitchEvents.IsActive ? GlitchEvents.ActiveType.ToString() : "None";

        if (playerHealth != null)
            WriteRow(t, "Player", "Player", "-", glitch, playerHealth.Normalized, 0f, false, true);

        foreach (FSMEnemy e in FindObjectsByType<FSMEnemy>(FindObjectsSortMode.None))
            WriteEnemy(t, e, "FSM", e.IsDead ? "Dead" : e.StateName, glitch, e.Perception);

        foreach (UtilityEnemy e in FindObjectsByType<UtilityEnemy>(FindObjectsSortMode.None))
            WriteEnemy(t, e, "Utility", e.IsDead ? "Dead" : e.Brain.CurrentName, glitch, e.Perception);

        writer.Flush();
    }

    void WriteEnemy(float t, EnemyBase e, string ai, string action, string glitch, EnemyPerception perc)
    {
        float dist = player != null ? Vector3.Distance(e.transform.position, player.position) : -1f;
        bool sees = perc != null && perc.CanSeeTarget;
        WriteRow(t, e.name, ai, action, glitch, e.HealthNormalized, dist, sees, false);
    }

    void WriteRow(float t, string id, string ai, string action, string glitch, float health, float dist, bool sees, bool distNA)
    {
        writer.WriteLine(string.Join(",",
            t.ToString("F2", CultureInfo.InvariantCulture),
            Csv(id),
            ai,
            Csv(action),
            glitch,
            health.ToString("F2", CultureInfo.InvariantCulture),
            distNA ? "" : dist.ToString("F1", CultureInfo.InvariantCulture),
            sees ? "1" : "0"));
    }

    string Csv(string s)
    {
        if (string.IsNullOrEmpty(s)) return "";
        if (s.IndexOf(',') >= 0 || s.IndexOf('"') >= 0 || s.IndexOf('\n') >= 0)
            return "\"" + s.Replace("\"", "\"\"") + "\"";
        return s;
    }

    void OnDisable() { Close(); }
    void OnApplicationQuit() { Close(); }

    void Close()
    {
        if (writer == null) return;
        writer.Flush();
        writer.Close();
        writer = null;
    }
}
