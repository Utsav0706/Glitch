using UnityEngine;
using UnityEngine.AI;

public class RepositionAction : EnemyAction
{
    public float weight = 0.5f;
    public float loseTargetTime = 4f;
    public float radius = 8f;
    public float repickInterval = 1.5f;

    float repickAt;

    public RepositionAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (!Known(loseTargetTime)) return 0f;
        return weight * (Sees ? 0.25f : 0.8f);
    }

    public override void OnEnter()
    {
        repickAt = 0f;
    }

    public override void Execute()
    {
        if (Time.time >= repickAt || body.AtDestination)
        {
            repickAt = Time.time + repickInterval;
            body.MoveTo(PickPoint());
        }

        if (Sees && Target != null)
            body.FaceTowards(Target.position);
    }

    Vector3 PickPoint()
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 r = Random.insideUnitCircle * radius;
            Vector3 candidate = body.transform.position + new Vector3(r.x, 0f, r.y);
            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
                return hit.position;
        }
        return body.transform.position;
    }
}
