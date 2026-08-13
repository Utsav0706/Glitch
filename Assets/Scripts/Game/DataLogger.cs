using System;
using System.Globalization;
using System.IO;
using System.Text;
using UnityEngine;

public class DataLogger : MonoBehaviour
{
    public float sampleInterval = 1f;

    const string Header = "time,enemyId,aiType,action,activeGlitch,health,distToPlayer,canSeePlayer,playerHealth,damageDealt,topUtilityScore,secondScore,seed";

    StreamWriter writer;
    string path;
    int seed;
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
        seed = Environment.TickCount & 0x7fffffff;
        UnityEngine.Random.InitState(seed);
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
            Debug.Log("[DataLogger] Session CSV: " + path + "  (seed " + seed + ")");
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
            Row(t, "Player", "Player", "-", glitch, F2(playerHealth.Normalized), "", false, "", "", "");

        foreach (FSMEnemy e in FindObjectsByType<FSMEnemy>(FindObjectsSortMode.None))
            WriteEnemy(t, e, "FSM", e.IsDead ? "Dead" : e.StateName, glitch, e.Perception, null);

        foreach (UtilityEnemy e in FindObjectsByType<UtilityEnemy>(FindObjectsSortMode.None))
            WriteEnemy(t, e, "Utility", e.IsDead ? "Dead" : e.Brain.CurrentName, glitch, e.Perception, e.Brain);

        writer.Flush();
    }

    void WriteEnemy(float t, EnemyBase e, string ai, string action, string glitch, EnemyPerception perc, UtilityBrain brain)
    {
        float dist = player != null ? Vector3.Distance(e.transform.position, player.position) : -1f;
        bool sees = perc != null && perc.CanSeeTarget;

        EnemyWeaponController weapon = e.GetComponent<EnemyWeaponController>();
        string dmg = weapon != null ? ((int)weapon.DamageDealt).ToString(CultureInfo.InvariantCulture) : "";
        string top = brain != null ? F3(brain.TopScore) : "";
        string second = brain != null ? F3(brain.SecondScore) : "";

        Row(t, e.name, ai, action, glitch,
            F2(e.HealthNormalized),
            dist >= 0f ? dist.ToString("F1", CultureInfo.InvariantCulture) : "",
            sees, dmg, top, second);
    }

    void Row(float t, string id, string ai, string action, string glitch, string health, string dist, bool sees, string dmg, string top, string second)
    {
        string ph = playerHealth != null ? F2(playerHealth.Normalized) : "";
        writer.WriteLine(string.Join(",",
            t.ToString("F2", CultureInfo.InvariantCulture),
            Csv(id), ai, Csv(action), glitch,
            health, dist, sees ? "1" : "0",
            ph, dmg, top, second,
            seed.ToString(CultureInfo.InvariantCulture)));
    }

    static string F2(float v) => v.ToString("F2", CultureInfo.InvariantCulture);
    static string F3(float v) => v.ToString("F3", CultureInfo.InvariantCulture);

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
