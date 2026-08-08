using UnityEngine;

public class EnemyWeaponController : MonoBehaviour
{
    public float damage = 10f;
    public float range = 30f;
    public float fireRate = 2f;
    public float spreadDegrees = 3f;
    public float muzzleHeight = 1.5f;
    public LayerMask hitMask = ~0;
    public MuzzleFlash muzzle;
    public Color shotColor = Color.clear;

    float nextShot;

    void Awake()
    {
        if (shotColor == Color.clear)
            shotColor = GetComponent<UtilityEnemy>() != null ? new Color(1f, 0.3f, 0.3f) : new Color(0.35f, 0.6f, 1f);
    }

    public bool CanFire => Time.time >= nextShot;
    public Vector3 MuzzlePosition => transform.position + Vector3.up * muzzleHeight;

    public bool TryFire(Vector3 targetPoint)
    {
        if (!CanFire) return false;
        nextShot = Time.time + (fireRate > 0f ? 1f / fireRate : 0f);
        Shoot(targetPoint);
        return true;
    }

    public bool TryFire(Transform targetTransform)
    {
        if (targetTransform == null) return false;
        return TryFire(targetTransform.position + Vector3.up * 1.4f);
    }

    void Shoot(Vector3 targetPoint)
    {
        Vector3 origin = MuzzlePosition;
        Vector3 to = targetPoint - origin;
        if (to.sqrMagnitude < 0.0001f) return;

        Vector3 dir = ApplySpread(to.normalized);

        if (muzzle != null) muzzle.Flash();

        Vector3 endPoint = origin + dir * range;

        if (Physics.Raycast(origin, dir, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Ignore)
            && hit.transform.root != transform.root)
        {
            endPoint = hit.point;
            Health h = hit.collider.GetComponentInParent<Health>();
            if (h != null) h.TakeDamage(damage);
        }

        Vector3 from = muzzle != null ? muzzle.transform.position : origin;
        Tracer.Spawn(from, endPoint, shotColor);
    }

    Vector3 ApplySpread(Vector3 dir)
    {
        if (spreadDegrees <= 0f) return dir;
        Vector2 c = Random.insideUnitCircle * spreadDegrees;
        return Quaternion.LookRotation(dir) * Quaternion.Euler(c.y, c.x, 0f) * Vector3.forward;
    }
}
