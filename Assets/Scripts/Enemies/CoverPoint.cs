using System.Collections.Generic;
using UnityEngine;

public class CoverPoint : MonoBehaviour
{
    public float coverHeight = 1.1f;

    static readonly List<CoverPoint> all = new List<CoverPoint>();

    Transform occupant;

    public static IReadOnlyList<CoverPoint> All => all;
    public Vector3 Position => transform.position;
    public Vector3 ShieldedPoint => transform.position + Vector3.up * coverHeight;
    public Transform Occupant => occupant;
    public bool IsOccupied => occupant != null;

    void OnEnable()
    {
        if (!all.Contains(this)) all.Add(this);
    }

    void OnDisable()
    {
        all.Remove(this);
        occupant = null;
    }

    public bool IsFreeFor(Transform seeker)
    {
        return occupant == null || occupant == seeker;
    }

    public bool Claim(Transform seeker)
    {
        if (!IsFreeFor(seeker)) return false;
        occupant = seeker;
        return true;
    }

    public void Release(Transform seeker)
    {
        if (occupant == seeker) occupant = null;
    }

    public bool ProtectsFrom(Vector3 threatPoint, LayerMask obstacleMask)
    {
        Vector3 from = ShieldedPoint;
        Vector3 to = threatPoint - from;
        float dist = to.magnitude;
        if (dist < 0.001f) return false;
        return Physics.Raycast(from, to / dist, dist, obstacleMask, QueryTriggerInteraction.Ignore);
    }

    public static CoverPoint FindNearest(Vector3 seekerPosition, Vector3 threatPoint, float maxDistance, LayerMask obstacleMask, Transform seeker = null)
    {
        CoverPoint best = null;
        float bestSqr = maxDistance > 0f ? maxDistance * maxDistance : Mathf.Infinity;

        for (int i = 0; i < all.Count; i++)
        {
            CoverPoint cp = all[i];
            if (cp == null || !cp.IsFreeFor(seeker)) continue;

            float sqr = (cp.transform.position - seekerPosition).sqrMagnitude;
            if (sqr >= bestSqr) continue;
            if (!cp.ProtectsFrom(threatPoint, obstacleMask)) continue;

            best = cp;
            bestSqr = sqr;
        }

        return best;
    }

    public static CoverPoint FindNearest(Transform seeker, Transform threat, float maxDistance, LayerMask obstacleMask)
    {
        if (seeker == null || threat == null) return null;
        return FindNearest(seeker.position, threat.position + Vector3.up * 1.4f, maxDistance, obstacleMask, seeker);
    }

    void OnDrawGizmos()
    {
        Vector3 baseP = transform.position + Vector3.up * 0.1f;

        Gizmos.color = IsOccupied ? new Color(1f, 0.45f, 0.1f, 0.9f) : new Color(0.2f, 0.8f, 1f, 0.9f);
        Gizmos.DrawSphere(baseP, 0.16f);
        Gizmos.DrawLine(baseP, ShieldedPoint);

        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.45f);
        Gizmos.DrawRay(ShieldedPoint, transform.forward * 1.2f);
    }
}
