using UnityEngine;

public class ChaseAction : EnemyAction
{
    public float weight = 0.7f;
    public float attackRange = 18f;
    public float loseTargetTime = 4f;
    public float repathInterval = 0.25f;

    float repathAt;

    public ChaseAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (TryGlitchScore("Chase", out float g)) return g;
        if (!Known(loseTargetTime)) return 0f;
        if (!Sees) return weight * 0.7f;

        float beyond = Considerations.Linear((DistanceToTarget() - attackRange) / Mathf.Max(1f, attackRange));
        return weight * beyond;
    }

    public override void Execute()
    {
        if (Time.time < repathAt) return;
        repathAt = Time.time + repathInterval;

        Vector3 dest = Sees && Target != null ? Target.position : perception.LastKnownPosition;
        body.MoveTo(dest);
    }
}
