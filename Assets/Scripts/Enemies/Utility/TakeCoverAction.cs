using UnityEngine;

public class TakeCoverAction : EnemyAction
{
    public float weight = 1f;
    public float searchRadius = 25f;
    public float loseTargetTime = 4f;
    public float glitchBonus = 0.4f;
    public float arriveRadius = 1.2f;

    CoverPoint cover;

    public TakeCoverAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (!Known(loseTargetTime)) return 0f;

        float hurt = Considerations.Hurt(body.HealthNormalized);
        float exposure = Considerations.Exposure(Sees, DistanceToTarget(), searchRadius);
        return weight * Considerations.Product(hurt, exposure) + Considerations.GlitchBonus(GlitchEvents.IsActive, glitchBonus);
    }

    public override void OnEnter()
    {
        Acquire();
    }

    public override void OnExit()
    {
        Release();
    }

    public override void Execute()
    {
        if (cover == null || !cover.ProtectsFrom(ThreatPoint(), perception.obstacleMask))
            Acquire();

        if (cover == null) return;
        if ((cover.Position - body.transform.position).sqrMagnitude > arriveRadius * arriveRadius) return;

        body.StopMoving();
        if (Sees && Target != null)
        {
            body.FaceTowards(Target.position);
            weapon.TryFire(Target);
        }
    }

    void Acquire()
    {
        CoverPoint next = CoverPoint.FindNearest(body.transform.position, ThreatPoint(), searchRadius, perception.obstacleMask, body.transform);
        if (next == cover) return;

        Release();
        if (next != null && next.Claim(body.transform))
        {
            cover = next;
            body.MoveTo(cover.Position);
        }
    }

    void Release()
    {
        if (cover == null) return;
        cover.Release(body.transform);
        cover = null;
    }
}
