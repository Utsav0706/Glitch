using UnityEngine;

public class AttackState : IState
{
    readonly FSMEnemy e;

    public AttackState(FSMEnemy enemy)
    {
        e = enemy;
    }

    public void Enter()
    {
        e.StopMoving();
    }

    public void Tick()
    {
        Transform t = e.Perception.Target;
        if (t == null) return;

        e.FaceTowards(t.position);
        e.Weapon.TryFire(t);
    }

    public void Exit()
    {
    }
}
