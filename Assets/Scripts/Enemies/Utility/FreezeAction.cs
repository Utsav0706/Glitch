public class FreezeAction : EnemyAction
{
    public FreezeAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        return body.IsFrozen ? 1000f : 0f;
    }

    public override void OnEnter()
    {
        body.StopMoving();
    }

    public override void Execute()
    {
    }
}
