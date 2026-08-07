using UnityEngine;

public class AttackAction : EnemyAction
{
    public float range = 18f;
    public float weight = 1f;

    public AttackAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
        : base(body, perception, weapon) { }

    public override float Score()
    {
        if (TryGlitchScore("Attack", out float g)) return g;
        if (!Sees) return 0f;
        return weight * Considerations.Closeness(DistanceToTarget(), range) * Certainty();
    }

    public override void OnEnter()
    {
        body.StopMoving();
    }

    public override void Execute()
    {
        Transform t = Target;
        if (t == null) return;
        body.FaceTowards(t.position);
        weapon.TryFire(t);
    }
}
