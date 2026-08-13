using UnityEngine;
using UnityEngine.AI;

public class PatrolAction : EnemyAction
{
    public float weight = 0.12f;
    public float loseTargetTime = 4f;
    public float radius = 12f;
    public float repathInterval = 4f;

    float repathAt;

    public PatrolAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (Known(loseTargetTime)) return 0f;
        return weight;
    }

    public override void OnEnter()
    {
        repathAt = 0f;
    }

    public override void Execute()
    {
        if (Time.time < repathAt && !body.AtDestination) return;
        repathAt = Time.time + repathInterval;
        body.MoveTo(NextPoint());
    }

    Vector3 NextPoint()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = body.transform.position + new Vector3(r.x, 0f, r.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 4f, NavMesh.AllAreas))
                return hit.position;
        }
        return body.transform.position;
    }
}
