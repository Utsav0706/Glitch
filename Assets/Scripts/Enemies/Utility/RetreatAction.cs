using UnityEngine;
using UnityEngine.AI;

public class RetreatAction : EnemyAction
{
    public float weight = 1f;
    public float loseTargetTime = 4f;
    public float searchRadius = 25f;
    public float retreatDistance = 8f;
    public float repathInterval = 0.4f;

    float repathAt;

    public RetreatAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (!Known(loseTargetTime)) return 0f;
        float hurt = Considerations.Curve(Considerations.Hurt(body.HealthNormalized), 3f);
        float exposure = Considerations.Exposure(Sees, DistanceToTarget(), searchRadius);
        return weight * Considerations.Product(hurt, exposure);
    }

    public override void Execute()
    {
        if (Sees && Target != null) body.FaceTowards(Target.position);

        if (Time.time < repathAt) return;
        repathAt = Time.time + repathInterval;

        Vector3 away = body.transform.position - ThreatPoint();
        away.y = 0f;
        if (away.sqrMagnitude < 0.0001f) return;

        Vector3 goal = body.transform.position + away.normalized * retreatDistance;
        if (NavMesh.SamplePosition(goal, out NavMeshHit hit, retreatDistance, NavMesh.AllAreas))
            body.MoveTo(hit.position);
    }
}
