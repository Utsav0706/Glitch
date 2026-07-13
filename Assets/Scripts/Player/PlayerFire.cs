using UnityEngine;

public class PlayerFire : MonoBehaviour
{
    public string fireButton = "Fire1";
    public float projectileSpeed = 40f;
    public float fireCooldown = 0.15f;
    public Material projectileMaterial;

    Transform cam;
    float nextFire;

    void Start()
    {
        cam = Camera.main.transform;
    }

    void Update()
    {
        if (Input.GetButton(fireButton) && Time.time >= nextFire)
        {
            nextFire = Time.time + fireCooldown;
            Fire();
        }
    }

    void Fire()
    {
        GameObject ball = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        ball.name = "Projectile";
        ball.transform.position = transform.position + Vector3.up * 1.6f + cam.forward * 0.8f;
        ball.transform.localScale = Vector3.one * 0.15f;
        if (projectileMaterial != null)
            ball.GetComponent<Renderer>().sharedMaterial = projectileMaterial;
        Rigidbody rb = ball.AddComponent<Rigidbody>();
        rb.mass = 0.2f;
        rb.linearVelocity = cam.forward * projectileSpeed;
        ball.AddComponent<Projectile>();
    }
}
