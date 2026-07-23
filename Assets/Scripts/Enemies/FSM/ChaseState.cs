using UnityEngine;

public class ChaseState : IState
{
    readonly FSMEnemy e;

    float repathAt;

    public ChaseState(FSMEnemy enemy)
    {
        e = enemy;
    }

    public void Enter()
    {
        e.SetSpeed(e.chaseSpeed);
        repathAt = 0f;
    }

    public void Tick()
    {
        if (Time.time < repathAt) return;
        repathAt = Time.time + e.chaseRepathInterval;

        EnemyPerception p = e.Perception;
        Vector3 dest = p.CanSeeTarget && p.Target != null ? p.Target.position : p.LastKnownPosition;

        e.MoveTo(dest);
    }

    public void Exit()
    {
    }
}
