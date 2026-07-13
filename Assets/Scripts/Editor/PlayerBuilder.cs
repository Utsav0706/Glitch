using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PlayerBuilder
{
    const string PlayerName = "Player";
    const string ModelPath = "Assets/Shoved Reaction With Spin.fbx";

    static readonly Color CPlayer = new Color(0.20f, 0.50f, 0.90f);

    [MenuItem("GLITCH/Player/Build Player", priority = 40)]
    public static void BuildPlayer()
    {
        GameObject old = GameObject.Find(PlayerName);
        if (old != null) Object.DestroyImmediate(old);

        GameObject model = AssetDatabase.LoadAssetAtPath<GameObject>(ModelPath);
        if (model == null)
        {
            Debug.LogError("[PlayerBuilder] Character model not found at " + ModelPath);
            return;
        }

        GameObject player = (GameObject)PrefabUtility.InstantiatePrefab(model);
        PrefabUtility.UnpackPrefabInstance(player, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        player.name = PlayerName;
        player.transform.position = new Vector3(0f, 0f, -40f);
        player.transform.rotation = Quaternion.identity;

        Rigidbody rb = player.AddComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;

        CapsuleCollider col = player.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0f, 0.9f, 0f);
        col.height = 1.8f;
        col.radius = 0.3f;
        col.sharedMaterial = AssetDatabase.LoadAssetAtPath<PhysicsMaterial>("Assets/3rdPerson+Fly/Materials/Character.physicMaterial");

        Animator anim = player.GetComponent<Animator>();
        if (anim == null) anim = player.AddComponent<Animator>();
        anim.runtimeAnimatorController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/3rdPerson+Fly/Animators/CharacterController.controller");
        anim.applyRootMotion = true;

        Material bodyMat = ArenaBuilder.Mat(CPlayer);
        foreach (Renderer r in player.GetComponentsInChildren<Renderer>())
        {
            Material[] mats = r.sharedMaterials;
            for (int i = 0; i < mats.Length; i++) mats[i] = bodyMat;
            r.sharedMaterials = mats;
        }

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

        player.AddComponent<MoveBehaviour>();

        AimBehaviourBasic aim = player.AddComponent<AimBehaviourBasic>();
        aim.crosshair = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/3rdPerson+Fly/Textures/decal_crosshair.png");

        PlayerFire fire = player.AddComponent<PlayerFire>();
        fire.projectileMaterial = ArenaBuilder.Mat(new Color(0.95f, 0.85f, 0.20f));

        Undo.RegisterCreatedObjectUndo(player, "Build Player");
        Selection.activeGameObject = player;
        EditorSceneManager.MarkSceneDirty(player.scene);
        Debug.Log("[PlayerBuilder] Animated player built from Mixamo character.");
    }
}
