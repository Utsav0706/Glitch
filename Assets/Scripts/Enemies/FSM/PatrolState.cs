using UnityEngine;

public class PatrolState : IState
{
    readonly FSMEnemy e;

    int index = -1;
    float repathAt;
    float settleUntil;

    public PatrolState(FSMEnemy enemy)
    {
        e = enemy;
    }

    public void Enter()
    {
        e.SetSpeed(e.patrolSpeed);
        PickNext();
    }

    public void Tick()
    {
        if (Time.time < settleUntil) return;
        if (e.AtDestination || Time.time >= repathAt) PickNext();
    }

    public void Exit()
    {
    }

    void PickNext()
    {
        Vector3 dest;

        if (e.patrolPoints != null && e.patrolPoints.Length > 0)
        {
            index = (index + 1) % e.patrolPoints.Length;
            Transform p = e.patrolPoints[index];
            if (p == null)
            {
                settleUntil = Time.time + 0.5f;
                return;
            }
            dest = p.position;
        }
        else if (!e.RandomPointNearHome(out dest))
        {
            settleUntil = Time.time + 1f;
            return;
        }

        e.MoveTo(dest);
        repathAt = Time.time + e.patrolRepathInterval;
        settleUntil = Time.time + 0.25f;
    }
}
