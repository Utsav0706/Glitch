using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class TargetDummy : MonoBehaviour
{
    public Color hitColor = Color.white;
    public Color deadColor = new Color(0.25f, 0.25f, 0.25f);
    public float flashTime = 0.08f;
    public float respawnDelay = 3f;

    Health health;
    Renderer rend;
    Collider body;
    MaterialPropertyBlock mpb;
    Color baseColor;
    float flashOffAt;
    bool dead;

    void Awake()
    {
        health = GetComponent<Health>();
        rend = GetComponentInChildren<Renderer>();
        body = GetComponent<Collider>();
        mpb = new MaterialPropertyBlock();
        if (rend != null && rend.sharedMaterial != null)
            baseColor = rend.sharedMaterial.color;
        else
            baseColor = Color.white;
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
        if (rend == null) return;
        rend.GetPropertyBlock(mpb);
        mpb.SetColor("_BaseColor", c);
        mpb.SetColor("_Color", c);
        rend.SetPropertyBlock(mpb);
    }
}
