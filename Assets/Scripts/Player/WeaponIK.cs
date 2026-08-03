using UnityEngine;

[RequireComponent(typeof(Animator))]
public class WeaponIK : MonoBehaviour
{
    public Transform leftHandTarget;
    [Range(0f, 1f)] public float positionWeight = 1f;
    [Range(0f, 1f)] public float rotationWeight = 0f;

    Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if (anim == null || leftHandTarget == null) return;

        anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, positionWeight);
        anim.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);

        if (rotationWeight > 0f)
        {
            anim.SetIKRotationWeight(AvatarIKGoal.LeftHand, rotationWeight);
            anim.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
        }
    }
}
