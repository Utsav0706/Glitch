using UnityEngine;

public class SpreadFireAction : EnemyAction
{
    public float weight = 0.6f;
    public float loseTargetTime = 4f;
    public float spreadDegrees = 10f;
    public float uncertaintyWeight = 0.9f;

    float savedSpread;

    public SpreadFireAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        float emergent = 0f;
        if (Known(loseTargetTime) && perception.IsInViewCone(ThreatPoint()))
        {
            float rangeOk = Considerations.InRange(DistanceToTarget(), weapon.range * 0.5f, weapon.range * 0.5f);
            float uncertain = uncertaintyWeight * (1f - Certainty());
            emergent = weight * (Sees ? 0.3f : 0.6f) * rangeOk + uncertain;
        }
        return emergent + GlitchBonus("SpreadFire");
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
