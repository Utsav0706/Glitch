using UnityEngine;

public class SpreadFireAction : EnemyAction
{
    public float weight = 0.6f;
    public float loseTargetTime = 4f;
    public float spreadDegrees = 10f;

    float savedSpread;

    public SpreadFireAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (TryGlitchScore("SpreadFire", out float g)) return g;
        if (!Known(loseTargetTime)) return 0f;
        if (!perception.IsInViewCone(ThreatPoint())) return 0f;

        float rangeOk = Considerations.InRange(DistanceToTarget(), weapon.range * 0.5f, weapon.range * 0.5f);
        return weight * (Sees ? 0.3f : 0.6f) * rangeOk;
    }

    public override void OnEnter()
    {
        body.StopMoving();
        savedSpread = weapon.spreadDegrees;
        weapon.spreadDegrees = spreadDegrees;
    }

    public override void OnExit()
    {
        weapon.spreadDegrees = savedSpread;
    }

    public override void Execute()
    {
        body.FaceTowards(ThreatPoint());
        weapon.TryFire(ThreatPoint());
    }
}
