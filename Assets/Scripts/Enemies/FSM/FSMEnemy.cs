using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyPerception))]
[RequireComponent(typeof(EnemyWeaponController))]
public class FSMEnemy : EnemyBase
{
    public Transform[] patrolPoints;
    public float patrolSpeed = 2.5f;
    public float chaseSpeed = 4.5f;
    public float patrolRadius = 12f;
    public float patrolRepathInterval = 6f;
    public float chaseRepathInterval = 0.25f;
    public float attackRange = 18f;
    public float loseTargetTime = 4f;

    public EnemyPerception Perception { get; private set; }
    public EnemyWeaponController Weapon { get; private set; }
    public StateMachine Machine { get; private set; }
    public string StateName => Machine != null ? Machine.CurrentName : "None";

    Vector3 home;
    IState patrol;
    IState chase;
    IState attack;

    protected override void Awake()
    {
        base.Awake();

        Perception = GetComponent<EnemyPerception>();
        Weapon = GetComponent<EnemyWeaponController>();
        home = transform.position;

        Machine = new StateMachine();
        patrol = new PatrolState(this);
        chase = new ChaseState(this);
        attack = new AttackState(this);
    }

    void Start()
    {
        Machine.ChangeState(patrol);
    }

    void Update()
    {
        if (IsDead) return;

        Evaluate();
        Machine.Tick();
    }

    void Evaluate()
    {
        Transform t = Perception.Target;

        if (Perception.CanSeeTarget && t != null)
        {
            float dist = Vector3.Distance(transform.position, t.position);
            Machine.ChangeState(dist <= attackRange ? attack : chase);
            return;
        }

        if (Perception.TimeSinceSeen <= loseTargetTime)
        {
            Machine.ChangeState(chase);
            return;
        }

        Machine.ChangeState(patrol);
    }

    public bool RandomPointNearHome(out Vector3 point)
    {
        for (int i = 0; i < 8; i++)
        {
            Vector2 r = Random.insideUnitCircle * patrolRadius;
            Vector3 candidate = home + new Vector3(r.x, 0f, r.y);

            if (NavMesh.SamplePosition(candidate, out NavMeshHit hit, 3f, NavMesh.AllAreas))
            {
                point = hit.position;
                return true;
            }
        }

        point = home;
        return false;
    }

    protected override void OnDeath()
    {
        Machine.ChangeState(null);
    }
}
