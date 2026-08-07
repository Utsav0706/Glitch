using System.Collections.Generic;
using UnityEngine;

public class PlayerDuplicateEffect : MonoBehaviour
{
    public Vector3[] offsets = { new Vector3(1.6f, 0f, -0.4f), new Vector3(-1.6f, 0f, -0.4f) };
    public Color decoyColor = new Color(0.53f, 0.81f, 0.92f);

    readonly List<GameObject> decoys = new List<GameObject>();
    Material decoyMat;
    bool active;
    float endTime;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void AutoCreate()
    {
        if (FindFirstObjectByType<PlayerDuplicateEffect>() == null)
            new GameObject("PlayerDuplicateEffect").AddComponent<PlayerDuplicateEffect>();
    }

    void OnEnable() { GlitchEvents.Triggered += OnGlitch; }

    void OnDisable()
    {
        GlitchEvents.Triggered -= OnGlitch;
        if (active) Clear();
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (type != GlitchType.PlayerDuplicate || active) return;

        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;

        Begin(player, duration);
    }

    void Begin(GameObject player, float duration)
    {
        active = true;
        endTime = Time.time + duration;
        decoys.Clear();

        for (int i = 0; i < offsets.Length; i++)
            decoys.Add(MakeDecoy(player, offsets[i]));
    }

    GameObject MakeDecoy(GameObject player, Vector3 offset)
    {
        GameObject d = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        d.name = "PlayerDecoy";
        d.transform.localScale = new Vector3(0.6f, 0.9f, 0.6f);
        d.transform.position = player.transform.position + player.transform.TransformDirection(offset);
        d.transform.rotation = player.transform.rotation;
        d.GetComponent<Renderer>().sharedMaterial = DecoyMaterial();

        Rigidbody rb = d.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;

        Decoy decoy = d.AddComponent<Decoy>();
        decoy.Init(player.transform, offset);
        return d;
    }

    void Update()
    {
        if (active && Time.time >= endTime) Clear();
    }

    void Clear()
    {
        active = false;
        for (int i = 0; i < decoys.Count; i++)
            if (decoys[i] != null) Destroy(decoys[i]);
        decoys.Clear();
    }

    Material DecoyMaterial()
    {
        if (decoyMat != null) return decoyMat;

        Shader sh = Shader.Find("Universal Render Pipeline/Lit");
        if (sh == null) sh = Shader.Find("Standard");
        decoyMat = new Material(sh);
        decoyMat.color = decoyColor;
        decoyMat.SetColor("_BaseColor", decoyColor);
        return decoyMat;
    }
}
