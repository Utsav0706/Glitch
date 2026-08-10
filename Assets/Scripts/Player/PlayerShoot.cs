using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public string fireButton = "Fire1";
    public float damage = 20f;
    public float range = 100f;
    public float fireCooldown = 0.07f;
    public int maxAmmo = 100;
    public float reloadTime = 1.2f;
    public MuzzleFlash muzzle;

    Camera cam;
    Crosshair reticle;
    Transform head;
    float nextFire;
    int ammo;
    bool reloading;

    public int Ammo => ammo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => reloading;

    void Start()
    {
        cam = Camera.main;
        reticle = FindFirstObjectByType<Crosshair>();
        head = FindHead();
        ammo = maxAmmo;
    }

    Transform FindHead()
    {
        foreach (Transform t in GetComponentsInChildren<Transform>())
            if (t.name.ToLower().EndsWith("head"))
                return t;
        return null;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R) && !reloading && ammo < maxAmmo)
            StartCoroutine(Reload());

        if (Input.GetButton(fireButton) && Time.time >= nextFire && !reloading && ammo > 0)
        {
            nextFire = Time.time + fireCooldown;
            Fire();
        }
    }

    void Fire()
    {
        if (cam == null) return;

        ammo--;

        Vector3 shotOrigin = head != null ? head.position + Vector3.up * 0.1f : transform.position + Vector3.up * 1.6f;

        if (muzzle != null)
        {
            muzzle.transform.position = shotOrigin;
            muzzle.Flash();
        }

        Vector3 aimPoint = reticle != null ? reticle.AimScreenPoint : new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        Ray ray = cam.ScreenPointToRay(aimPoint);
        RaycastHit[] hits = Physics.RaycastAll(ray, range, ~0, QueryTriggerInteraction.Ignore);

        RaycastHit best = default;
        bool found = false;
        float closest = float.MaxValue;
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.root == transform) continue;
            if (hits[i].distance < closest)
            {
                closest = hits[i].distance;
                best = hits[i];
                found = true;
            }
        }

        if (found)
        {
            Health h = best.collider.GetComponentInParent<Health>();
            if (h != null) h.TakeDamage(damage);
        }

        Vector3 tracerTo = found ? best.point : ray.GetPoint(range);
        Tracer.Spawn(shotOrigin, tracerTo, Color.white);

        if (ammo <= 0) StartCoroutine(Reload());
    }

    IEnumerator Reload()
    {
        reloading = true;
        yield return new WaitForSeconds(reloadTime);
        ammo = maxAmmo;
        reloading = false;
    }
}
