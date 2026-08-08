using UnityEngine;

[RequireComponent(typeof(Health))]
public class Decoy : MonoBehaviour
{
    Transform target;
    Vector3 offset;
    Health health;
    Animator srcAnim;
    Animator dstAnim;

    public void Init(Transform player, Vector3 localOffset)
    {
        target = player;
        offset = localOffset;

        health = GetComponent<Health>();
        health.maxHealth = 1f;
        health.Revive();
        health.Died += OnDied;

        srcAnim = player.GetComponent<Animator>();
        dstAnim = GetComponent<Animator>();

        PlayerCopies.Register(transform);
        Follow();
        SyncAnim();
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
        SyncAnim();
    }

    void Follow()
    {
        if (target == null) return;
        transform.position = target.position + target.TransformDirection(offset);
        transform.rotation = target.rotation;
    }

    void SyncAnim()
    {
        if (srcAnim == null || dstAnim == null || dstAnim.runtimeAnimatorController == null) return;

        AnimatorControllerParameter[] ps = srcAnim.parameters;
        for (int i = 0; i < ps.Length; i++)
        {
            AnimatorControllerParameter p = ps[i];
            if (srcAnim.IsParameterControlledByCurve(p.nameHash)) continue;

            switch (p.type)
            {
                case AnimatorControllerParameterType.Float:
                    dstAnim.SetFloat(p.nameHash, srcAnim.GetFloat(p.nameHash));
                    break;
                case AnimatorControllerParameterType.Int:
                    dstAnim.SetInteger(p.nameHash, srcAnim.GetInteger(p.nameHash));
                    break;
                case AnimatorControllerParameterType.Bool:
                    dstAnim.SetBool(p.nameHash, srcAnim.GetBool(p.nameHash));
                    break;
            }
        }
    }
}
