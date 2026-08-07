using System.Collections;
using UnityEngine;

public class PlayerShoot : MonoBehaviour
{
    public string fireButton = "Fire1";
    public float damage = 20f;
    public float range = 100f;
    public float fireCooldown = 0.15f;
    public int maxAmmo = 12;
    public float reloadTime = 1.2f;
    public MuzzleFlash muzzle;

    Camera cam;
    float nextFire;
    int ammo;
    bool reloading;

    public int Ammo => ammo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => reloading;

    void Start()
    {
        cam = Camera.main;
        ammo = maxAmmo;
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

        if (muzzle != null) muzzle.Flash();

        Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f));
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
