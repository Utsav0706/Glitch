using UnityEngine;

public class ReorientAction : EnemyAction
{
    public float weight = 0.6f;
    public float loseTargetTime = 4f;

    public ReorientAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        float emergent = 0f;
        if (Known(loseTargetTime))
        {
            bool inCone = perception.IsInViewCone(ThreatPoint());
            emergent = weight * (inCone ? 0.1f : 0.9f);
        }
        return emergent + GlitchBonus("Reorient");
    }

    public override void OnEnter()
    {
        body.StopMoving();
    }

    public override void Execute()
    {
        body.FaceTowards(ThreatPoint());
    }
}
