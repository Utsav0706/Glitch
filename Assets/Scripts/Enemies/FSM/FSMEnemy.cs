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
    public float coverHealthThreshold = 0.4f;
    public float coverSearchRadius = 25f;
    public float coverRefreshInterval = 1.5f;
    public float coverArriveRadius = 1.2f;
    public float glitchReactionTime = 5f;

    public EnemyPerception Perception { get; private set; }
    public EnemyWeaponController Weapon { get; private set; }
    public StateMachine Machine { get; private set; }
    public string StateName => Machine != null ? Machine.CurrentName : "None";
    public bool IsFrozen => Time.time < frozenUntil;
    public float HealthNormalized => health != null ? health.Normalized : 1f;
    public bool IsReactingToGlitch => reaction != null && Time.time < reactionUntil;
    public GlitchType LastGlitch => lastGlitch;

    Vector3 home;
    float frozenUntil;
    IState reaction;
    float reactionUntil;
    GlitchType lastGlitch;
    IState patrol;
    IState chase;
    IState attack;
    IState takeCover;
    IState frozen;

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
        takeCover = new TakeCoverState(this);
        frozen = new FrozenState(this);
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        GlitchEvents.Triggered += OnGlitch;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        GlitchEvents.Triggered -= OnGlitch;
    }

    void OnGlitch(GlitchType type, float duration)
    {
        if (IsDead) return;

        IState next = ReactionFor(type);
        if (next == null) return;

        lastGlitch = type;
        reaction = next;
        reactionUntil = Time.time + (duration > 0f ? duration : glitchReactionTime);
    }

    IState ReactionFor(GlitchType type)
    {
        switch (type)
        {
            case GlitchType.GravityFlip: return takeCover;
            case GlitchType.WallDisappear: return chase;
            case GlitchType.Blackout: return takeCover;
            case GlitchType.TimeDilation: return attack;
            default: return null;
        }
    }

    public void Freeze(float duration)
    {
        if (duration <= 0f) return;
        frozenUntil = Mathf.Max(frozenUntil, Time.time + duration);
    }

    public void Unfreeze()
    {
        frozenUntil = 0f;
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
        if (IsFrozen)
        {
            Machine.ChangeState(frozen);
            return;
        }

        if (reaction != null)
        {
            if (Time.time < reactionUntil)
            {
                Machine.ChangeState(reaction);
                return;
            }
            reaction = null;
        }

        Transform t = Perception.Target;
        bool sees = Perception.CanSeeTarget && t != null;
        bool recent = Perception.TimeSinceSeen <= loseTargetTime;

        if (HealthNormalized <= coverHealthThreshold && (sees || recent))
        {
            Machine.ChangeState(takeCover);
            return;
        }

        if (sees)
        {
            float dist = Vector3.Distance(transform.position, t.position);
            Machine.ChangeState(dist <= attackRange ? attack : chase);
            return;
        }

        if (recent)
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
