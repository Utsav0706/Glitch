using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Health))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour
{
    public float moveSpeed = 3.5f;
    public float stoppingDistance = 1.5f;
    public float turnSpeed = 8f;
    public float despawnDelay = 3f;

    protected Health health;
    protected NavMeshAgent agent;

    Collider body;
    bool dead;

    public NavMeshAgent Agent => agent;
    public bool IsDead => dead;
    public bool CanMove => !dead && agent != null && agent.enabled && agent.isOnNavMesh;
    public bool HasPath => CanMove && agent.hasPath;
    public float RemainingDistance => CanMove && !agent.pathPending ? agent.remainingDistance : Mathf.Infinity;
    public bool AtDestination => CanMove && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

    protected virtual void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        body = GetComponent<Collider>();
        agent.speed = moveSpeed;
        agent.stoppingDistance = stoppingDistance;
    }

    protected virtual void OnEnable()
    {
        health.Damaged += HandleDamaged;
        health.Died += HandleDied;
    }

    protected virtual void OnDisable()
    {
        health.Damaged -= HandleDamaged;
        health.Died -= HandleDied;
    }

    public void MoveTo(Vector3 destination)
    {
        if (!CanMove) return;
        agent.isStopped = false;
        agent.SetDestination(destination);
    }

    public void StopMoving()
    {
        if (!CanMove) return;
        agent.isStopped = true;
        agent.ResetPath();
    }

    public void SetSpeed(float speed)
    {
        moveSpeed = speed;
        if (agent != null) agent.speed = speed;
    }

    public void FaceTowards(Vector3 point)
    {
        Vector3 dir = point - transform.position;
        dir.y = 0f;
        if (dir.sqrMagnitude < 0.0001f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(dir), turnSpeed * Time.deltaTime);
    }

    void HandleDamaged(float amount)
    {
        if (dead) return;
        OnDamaged(amount);
    }

    void HandleDied()
    {
        if (dead) return;
        dead = true;

        if (agent != null && agent.enabled)
        {
            if (agent.isOnNavMesh)
            {
                agent.isStopped = true;
                agent.ResetPath();
            }
            agent.enabled = false;
        }

        if (body != null) body.enabled = false;

        OnDeath();

        if (despawnDelay > 0f) StartCoroutine(Despawn());
    }

    protected virtual void OnDamaged(float amount) { }

    protected virtual void OnDeath() { }

    IEnumerator Despawn()
    {
        yield return new WaitForSeconds(despawnDelay);
        Destroy(gameObject);
    }
}
