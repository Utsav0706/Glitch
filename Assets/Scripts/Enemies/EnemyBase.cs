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
    public float respawnDelay = 15f;

    protected Health health;
    protected NavMeshAgent agent;

    Collider body;
    bool dead;
    float frozenUntil;

    public NavMeshAgent Agent => agent;
    public bool IsDead => dead;
    public float HealthNormalized => health != null ? health.Normalized : 1f;
    public bool IsFrozen => Time.time < frozenUntil;
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

    public void Freeze(float duration)
    {
        if (duration > 0f) frozenUntil = Mathf.Max(frozenUntil, Time.time + duration);
    }

    public void Unfreeze()
    {
        frozenUntil = 0f;
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

        SetRenderersEnabled(false);
        OnDeath();

        StartCoroutine(RespawnRoutine());
    }

    protected virtual void OnDamaged(float amount) { }

    protected virtual void OnDeath() { }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);
        RespawnNow();
    }

    void RespawnNow()
    {
        Vector3 pos = RandomNavPoint();
        transform.position = pos;

        health.Revive();
        dead = false;
        frozenUntil = 0f;

        if (agent != null)
        {
            agent.enabled = true;
            agent.Warp(pos);
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.stoppingDistance = stoppingDistance;
        }

        if (body != null) body.enabled = true;
        SetRenderersEnabled(true);
    }

    Vector3 RandomNavPoint()
    {
        GameObject playerGo = GameObject.FindGameObjectWithTag("Player");
        Transform player = playerGo != null ? playerGo.transform : null;

        for (int i = 0; i < 40; i++)
        {
            Vector3 c = new Vector3(Random.Range(-46f, 46f), 1f, Random.Range(-46f, 46f));
            if (NavMesh.SamplePosition(c, out NavMeshHit hit, 8f, NavMesh.AllAreas))
            {
                if (player != null && (hit.position - player.position).sqrMagnitude < 144f) continue;
                return hit.position;
            }
        }

        return transform.position;
    }

    void SetRenderersEnabled(bool on)
    {
        foreach (Renderer r in GetComponentsInChildren<Renderer>(true))
            r.enabled = on;
    }
}
