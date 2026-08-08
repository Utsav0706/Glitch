using System.Collections.Generic;
using UnityEngine;

public class PlayerDuplicateEffect : MonoBehaviour
{
    public Vector3[] offsets = { new Vector3(1.6f, 0f, -0.4f), new Vector3(-1.6f, 0f, -0.4f) };

    readonly List<GameObject> decoys = new List<GameObject>();
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
        GameObject d = Instantiate(player);
        d.name = "PlayerDecoy";
        d.tag = "Untagged";
        d.transform.SetPositionAndRotation(
            player.transform.position + player.transform.TransformDirection(offset),
            player.transform.rotation);

        foreach (MonoBehaviour mb in d.GetComponentsInChildren<MonoBehaviour>(true))
            if (mb != null && !(mb is Health)) mb.enabled = false;

        foreach (Rigidbody rb in d.GetComponentsInChildren<Rigidbody>(true))
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        Transform muzzle = d.transform.Find("Muzzle");
        if (muzzle != null) muzzle.gameObject.SetActive(false);

        Animator anim = d.GetComponent<Animator>();
        if (anim != null) anim.applyRootMotion = false;

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
}
