using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class TargetDummy : MonoBehaviour
{
    public Color hitColor = new Color(1f, 0.35f, 0.3f);
    public Color deadColor = new Color(0.3f, 0.3f, 0.3f);
    public float flashTime = 0.08f;
    public float respawnDelay = 3f;

    Health health;
    Renderer[] rends;
    Collider body;
    MaterialPropertyBlock mpb;
    Color baseColor = Color.white;
    float flashOffAt;
    bool dead;

    void Awake()
    {
        health = GetComponent<Health>();
        rends = GetComponentsInChildren<Renderer>();
        body = GetComponent<Collider>();
        mpb = new MaterialPropertyBlock();
        if (rends.Length > 0 && rends[0].sharedMaterial != null)
            baseColor = rends[0].sharedMaterial.color;
    }

    void OnEnable()
    {
        health.Damaged += OnDamaged;
        health.Died += OnDied;
    }

    void OnDisable()
    {
        health.Damaged -= OnDamaged;
        health.Died -= OnDied;
    }

    void Update()
    {
        if (flashOffAt > 0f && Time.time >= flashOffAt)
        {
            flashOffAt = 0f;
            SetColor(dead ? deadColor : baseColor);
        }
    }

    void OnDamaged(float amount)
    {
        if (dead) return;
        SetColor(hitColor);
        flashOffAt = Time.time + flashTime;
    }

    void OnDied()
    {
        dead = true;
        flashOffAt = 0f;
        SetColor(deadColor);
        if (body != null) body.enabled = false;
        if (respawnDelay > 0f) StartCoroutine(Respawn());
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSeconds(respawnDelay);
        dead = false;
        SetColor(baseColor);
        if (body != null) body.enabled = true;
        health.Revive();
    }

    void SetColor(Color c)
    {
        if (rends == null) return;
        for (int i = 0; i < rends.Length; i++)
        {
            Renderer r = rends[i];
            if (r == null) continue;
            r.GetPropertyBlock(mpb);
            mpb.SetColor("_BaseColor", c);
            mpb.SetColor("_Color", c);
            r.SetPropertyBlock(mpb);
        }
    }
}
