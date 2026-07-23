using System;
using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    public Transform target;
    public float viewRadius = 20f;
    [Range(0f, 360f)] public float viewAngle = 110f;
    public float eyeHeight = 1.6f;
    public float targetHeight = 1.4f;
    public LayerMask obstacleMask = ~0;
    public float scanInterval = 0.1f;

    public event Action TargetSpotted;
    public event Action TargetLost;

    bool canSee;
    float nextScan;
    Vector3 lastKnown;
    float lastSeenTime = -999f;

    public bool CanSeeTarget => canSee;
    public Transform Target => target;
    public Vector3 LastKnownPosition => lastKnown;
    public float TimeSinceSeen => Time.time - lastSeenTime;
    public Vector3 EyePosition => transform.position + Vector3.up * eyeHeight;

    void Awake()
    {
        if (target == null) AcquireTarget();
    }

    void Update()
    {
        if (Time.time < nextScan) return;
        nextScan = Time.time + scanInterval;

        if (target == null) AcquireTarget();

        bool now = Scan();

        if (now)
        {
            lastKnown = target.position;
            lastSeenTime = Time.time;
        }

        if (now != canSee)
        {
            canSee = now;
            if (now) TargetSpotted?.Invoke();
            else TargetLost?.Invoke();
        }
    }

    bool Scan()
    {
        if (target == null) return false;

        Vector3 eye = EyePosition;
        Vector3 point = target.position + Vector3.up * targetHeight;
        Vector3 to = point - eye;
        float dist = to.magnitude;

        if (dist > viewRadius || dist < 0.0001f) return dist <= viewRadius;

        Vector3 dir = to / dist;
        Vector3 flat = new Vector3(dir.x, 0f, dir.z);
        if (flat.sqrMagnitude > 0.0001f && Vector3.Angle(transform.forward, flat.normalized) > viewAngle * 0.5f)
            return false;

        if (Physics.Raycast(eye, dir, out RaycastHit hit, dist, obstacleMask, QueryTriggerInteraction.Ignore))
            return hit.transform == target || hit.transform.IsChildOf(target);

        return true;
    }

    public bool HasLineOfSight(Vector3 point)
    {
        Vector3 eye = EyePosition;
        Vector3 to = point - eye;
        float dist = to.magnitude;
        if (dist < 0.0001f) return true;
        return !Physics.Raycast(eye, to / dist, dist, obstacleMask, QueryTriggerInteraction.Ignore);
    }

    public bool IsInViewCone(Vector3 point)
    {
        Vector3 flat = point - transform.position;
        flat.y = 0f;
        if (flat.sqrMagnitude < 0.0001f) return true;
        return Vector3.Angle(transform.forward, flat.normalized) <= viewAngle * 0.5f;
    }

    void AcquireTarget()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        if (go != null)
        {
            target = go.transform;
            return;
        }

        PlayerShoot ps = FindFirstObjectByType<PlayerShoot>();
        if (ps != null) target = ps.transform;
    }

    void OnDrawGizmosSelected()
    {
        Vector3 eye = transform.position + Vector3.up * eyeHeight;

        Gizmos.color = new Color(1f, 0.85f, 0.2f, 0.6f);
        Gizmos.DrawWireSphere(eye, viewRadius);

        Vector3 left = Quaternion.Euler(0f, -viewAngle * 0.5f, 0f) * transform.forward;
        Vector3 right = Quaternion.Euler(0f, viewAngle * 0.5f, 0f) * transform.forward;
        Gizmos.DrawLine(eye, eye + left * viewRadius);
        Gizmos.DrawLine(eye, eye + right * viewRadius);

        if (target != null)
        {
            Gizmos.color = canSee ? Color.green : Color.red;
            Gizmos.DrawLine(eye, target.position + Vector3.up * targetHeight);
        }
    }
}
