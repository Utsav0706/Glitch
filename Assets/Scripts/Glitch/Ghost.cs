using UnityEngine;

public class Ghost : MonoBehaviour
{
    Health health;

    public void Init(Health h)
    {
        health = h;
        health.maxHealth = 1f;
        health.Revive();
        health.Died += OnDied;
    }

    void OnDestroy()
    {
        if (health != null) health.Died -= OnDied;
    }

    void OnDied()
    {
        Destroy(gameObject);
    }
}
