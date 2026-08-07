using UnityEngine;

[RequireComponent(typeof(Health))]
public class Decoy : MonoBehaviour
{
    Transform target;
    Vector3 offset;
    Health health;

    public void Init(Transform player, Vector3 localOffset)
    {
        target = player;
        offset = localOffset;

        health = GetComponent<Health>();
        health.maxHealth = 1f;
        health.Revive();
        health.Died += OnDied;

        PlayerCopies.Register(transform);
        Follow();
    }

    void OnDestroy()
    {
        PlayerCopies.Unregister(transform);
        if (health != null) health.Died -= OnDied;
    }

    void OnDied()
    {
        Destroy(gameObject);
    }

    void LateUpdate()
    {
        Follow();
    }

    void Follow()
    {
        if (target == null) return;
        transform.position = target.position + target.TransformDirection(offset);
        transform.rotation = target.rotation;
    }
}
