using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PlayerBuilder
{
    const string PlayerName = "Player";

    static readonly Color CPlayer = new Color(0.20f, 0.50f, 0.90f);

    [MenuItem("GLITCH/Player/Build Player", priority = 40)]
    public static void BuildPlayer()
    {
        GameObject old = GameObject.Find(PlayerName);
        if (old != null) Object.DestroyImmediate(old);

        GameObject player = new GameObject(PlayerName);
        player.transform.position = new Vector3(0f, 0f, -40f);

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        CapsuleCollider col = player.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0f, 1.1f, 0f);
        col.height = 2.2f;
        col.radius = 0.35f;
        col.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/3rdPerson+Fly/Materials/Character.physicMaterial");

        Animator anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/3rdPerson+Fly/Animators/CharacterController.controller");

        Camera cam = Camera.main;
        if (cam == null)
        {
            GameObject camObj = GameObject.Find("Main Camera");
            if (camObj != null) cam = camObj.GetComponent<Camera>();
        }
        if (cam == null)
        {
            Debug.LogError("[PlayerBuilder] No main camera found in scene.");
            return;
        }

        ThirdPersonOrbitCamBasic orbit = cam.GetComponent<ThirdPersonOrbitCamBasic>();
        if (orbit == null) orbit = cam.gameObject.AddComponent<ThirdPersonOrbitCamBasic>();
        orbit.player = player.transform;

        BasicBehaviour basic = player.AddComponent<BasicBehaviour>();
        basic.playerCamera = cam.transform;

        MoveBehaviour move = player.AddComponent<MoveBehaviour>();
        move.jumpInertialForce = 0f;

        AimBehaviourBasic aim = player.AddComponent<AimBehaviourBasic>();
        aim.crosshair = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3rdPerson+Fly/Textures/decal_crosshair.png");

        player.AddComponent<PlayerMotor>();

        PlayerFire fire = player.AddComponent<PlayerFire>();
        fire.projectileMaterial = ArenaBuilder.Mat(new Color(0.95f, 0.85f, 0.20f));

        GameObject body = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        body.name = "Body";
        Object.DestroyImmediate(body.GetComponent<Collider>());
        body.transform.SetParent(player.transform, false);
        body.transform.localPosition = new Vector3(0f, 1.1f, 0f);
        body.transform.localScale = new Vector3(0.7f, 1.1f, 0.7f);
        body.GetComponent<Renderer>().sharedMaterial = ArenaBuilder.Mat(CPlayer);

        GameObject nose = GameObject.CreatePrimitive(PrimitiveType.Cube);
        nose.name = "Nose";
        Object.DestroyImmediate(nose.GetComponent<Collider>());
        nose.transform.SetParent(player.transform, false);
        nose.transform.localPosition = new Vector3(0f, 1.6f, 0.4f);
        nose.transform.localScale = new Vector3(0.2f, 0.2f, 0.4f);
        nose.GetComponent<Renderer>().sharedMaterial = ArenaBuilder.Mat(CPlayer * 0.6f);

        Undo.RegisterCreatedObjectUndo(player, "Build Player");
        Selection.activeGameObject = player;
        EditorSceneManager.MarkSceneDirty(player.scene);
        Debug.Log("[PlayerBuilder] Player built at south yard, camera wired.");
    }
}
