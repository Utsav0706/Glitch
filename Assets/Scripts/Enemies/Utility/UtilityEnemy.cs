using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EnemyPerception))]
[RequireComponent(typeof(EnemyWeaponController))]
public class UtilityEnemy : EnemyBase
{
    public float attackRange = 18f;
    public float combatSpeed = 4.5f;

    public EnemyPerception Perception { get; private set; }
    public EnemyWeaponController Weapon { get; private set; }
    public UtilityBrain Brain { get; private set; }

    static readonly List<UtilityEnemy> all = new List<UtilityEnemy>();
    public static IReadOnlyList<UtilityEnemy> All => all;

    protected override void Awake()
    {
        base.Awake();

        Perception = GetComponent<EnemyPerception>();
        Weapon = GetComponent<EnemyWeaponController>();
        SetSpeed(combatSpeed);

        Brain = new UtilityBrain(0.25f);
        Brain.Add(new AttackAction(this, Perception, Weapon) { range = attackRange });
        Brain.Add(new ChaseAction(this, Perception, Weapon) { attackRange = attackRange });
        Brain.Add(new TakeCoverAction(this, Perception, Weapon));
        Brain.Add(new RepositionAction(this, Perception, Weapon));
        Brain.Add(new ReorientAction(this, Perception, Weapon));
        Brain.Add(new SpreadFireAction(this, Perception, Weapon));
        Brain.Add(new RetreatAction(this, Perception, Weapon));
        Brain.Add(new FreezeAction(this, Perception, Weapon));
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        if (!all.Contains(this)) all.Add(this);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        all.Remove(this);
    }

    void Update()
    {
        if (IsDead) return;
        Brain.Tick();
    }
}
