using UnityEngine;

public class ReorientAction : EnemyAction
{
    public float weight = 0.6f;
    public float loseTargetTime = 4f;

    public ReorientAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (TryGlitchScore("Reorient", out float g)) return g;
        if (!Known(loseTargetTime)) return 0f;
        bool inCone = perception.IsInViewCone(ThreatPoint());
        return weight * (inCone ? 0.1f : 0.9f);
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
