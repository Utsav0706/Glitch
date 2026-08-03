public class FreezeAction : EnemyAction
{
    public FreezeAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (body.IsFrozen) return 100f;
        if (TryGlitchScore("Freeze", out float g)) return g;
        return 0f;
    }

    public override void OnEnter()
    {
        body.StopMoving();
    }

    public override void Execute()
    {
    }
}
