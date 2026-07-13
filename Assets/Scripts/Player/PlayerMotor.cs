using UnityEngine;

public class PlayerMotor : MonoBehaviour
{
    public float moveScale = 3.5f;
    public float aimStrafeSpeed = 2f;

    Rigidbody rBody;
    Animator anim;
    int speedFloat;
    int aimBool;
    int groundedBool;
    int hFloat;
    int vFloat;

    void Awake()
    {
        rBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        speedFloat = Animator.StringToHash("Speed");
        aimBool = Animator.StringToHash("Aim");
        groundedBool = Animator.StringToHash("Grounded");
        hFloat = Animator.StringToHash("H");
        vFloat = Animator.StringToHash("V");
    }

    void FixedUpdate()
    {
        if (!anim.GetBool(groundedBool)) return;

        Vector3 planar;
        if (anim.GetBool(aimBool))
        {
            planar = transform.forward * anim.GetFloat(vFloat) + transform.right * anim.GetFloat(hFloat);
            planar = Vector3.ClampMagnitude(planar, 1f) * aimStrafeSpeed;
        }
        else
        {
            planar = transform.forward * (anim.GetFloat(speedFloat) * moveScale);
        }

        planar.y = rBody.linearVelocity.y;
        rBody.linearVelocity = planar;
    }
}
