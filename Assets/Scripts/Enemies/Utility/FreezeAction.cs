public class FreezeAction : EnemyAction
{
    public FreezeAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (body.IsFrozen) return 100f;
        return GlitchBonus("Freeze");
    }

    public override void OnEnter()
    {
        body.StopMoving();
    }

    public override void Execute()
    {
    }
}
