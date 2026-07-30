using UnityEngine;

public abstract class EnemyAction : UtilityAction
{
    protected readonly EnemyBase body;
    protected readonly EnemyPerception perception;
    protected readonly EnemyWeaponController weapon;

    protected EnemyAction(EnemyBase body, EnemyPerception perception, EnemyWeaponController weapon)
    {
        this.body = body;
        this.perception = perception;
        this.weapon = weapon;
    }

    protected Transform Target => perception != null ? perception.Target : null;

    protected bool Sees => perception != null && perception.CanSeeTarget && perception.Target != null;

    protected bool Known(float within) => Sees || (perception != null && perception.TimeSinceSeen <= within);

    protected float DistanceToTarget()
    {
        Transform t = Target;
        return t != null ? Vector3.Distance(body.transform.position, t.position) : Mathf.Infinity;
    }

    protected Vector3 ThreatPoint()
    {
        Vector3 basePos = Sees && Target != null ? Target.position : perception.LastKnownPosition;
        return basePos + Vector3.up * 1.4f;
    }
}
