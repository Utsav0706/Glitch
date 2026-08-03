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
    public Vector2 aimOffset = new Vector2(0.14f, 0f);
    public float firePoseHold = 0.18f;

    Camera cam;
    Animator anim;
    int gunLayer = -1;
    float nextFire;
    float fireHoldUntil;
    int ammo;
    bool reloading;

    public int Ammo => ammo;
    public int MaxAmmo => maxAmmo;
    public bool IsReloading => reloading;
    public Vector2 AimScreenPoint => new Vector2(Screen.width * (0.5f + aimOffset.x), Screen.height * (0.5f + aimOffset.y));

    void Start()
    {
        cam = Camera.main;
        ammo = maxAmmo;
        anim = GetComponent<Animator>();
        if (anim != null) gunLayer = anim.GetLayerIndex("Gunplay");
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

        if (anim != null && gunLayer >= 0)
        {
            float target = Time.time < fireHoldUntil ? 1f : 0f;
            float w = Mathf.MoveTowards(anim.GetLayerWeight(gunLayer), target, Time.deltaTime * 8f);
            anim.SetLayerWeight(gunLayer, w);
        }
    }

    void Fire()
    {
        if (cam == null) return;

        ammo--;
        fireHoldUntil = Time.time + firePoseHold;

        if (muzzle != null) muzzle.Flash();

        Ray ray = cam.ScreenPointToRay(new Vector3(AimScreenPoint.x, AimScreenPoint.y, 0f));
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
