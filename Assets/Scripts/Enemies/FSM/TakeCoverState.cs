using UnityEngine;

public class TakeCoverState : IState
{
    readonly FSMEnemy e;

    CoverPoint cover;
    float refreshAt;

    public TakeCoverState(FSMEnemy enemy)
    {
        e = enemy;
    }

    public bool HasCover => cover != null;

    public void Enter()
    {
        e.SetSpeed(e.chaseSpeed);
        refreshAt = 0f;
        Acquire();
    }

    public void Tick()
    {
        if (Time.time >= refreshAt) Acquire();

        if (cover != null && (cover.Position - e.transform.position).sqrMagnitude > e.coverArriveRadius * e.coverArriveRadius)
            return;

        if (cover != null) e.StopMoving();

        Transform t = e.Perception.Target;
        if (t == null || !e.Perception.CanSeeTarget) return;

        e.FaceTowards(t.position);
        e.Weapon.TryFire(t);
    }

    public void Exit()
    {
        Release();
    }

    void Acquire()
    {
        refreshAt = Time.time + e.coverRefreshInterval;

        Vector3 threat = ThreatPoint();
        LayerMask mask = e.Perception.obstacleMask;

        if (cover != null && cover.ProtectsFrom(threat, mask)) return;

        CoverPoint next = CoverPoint.FindNearest(e.transform.position, threat, e.coverSearchRadius, mask, e.transform);

        if (next == null)
        {
            Release();
            return;
        }

        if (next == cover) return;

        Release();

        if (next.Claim(e.transform))
        {
            cover = next;
            e.MoveTo(cover.Position);
        }
    }

    Vector3 ThreatPoint()
    {
        Transform t = e.Perception.Target;
        Vector3 basePos = t != null && e.Perception.CanSeeTarget ? t.position : e.Perception.LastKnownPosition;
        return basePos + Vector3.up * 1.4f;
    }

    void Release()
    {
        if (cover == null) return;
        cover.Release(e.transform);
        cover = null;
    }
}
