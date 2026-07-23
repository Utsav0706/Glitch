using UnityEngine;

public class FrozenState : IState
{
    readonly FSMEnemy e;

    public FrozenState(FSMEnemy enemy)
    {
        e = enemy;
    }

    public void Enter()
    {
        e.StopMoving();
    }

    public void Tick()
    {
    }

    public void Exit()
    {
    }
}
