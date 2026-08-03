using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class PlayerBuilder
{
    const string PlayerName = "Player";
    const string ModelPath = "Assets/Shoved Reaction With Spin.fbx";
    const string GunPath = "Assets/GDTV Sharp Shooter Assets/Models/Kinetic Weapons/Heavy_machine_gun_01_Blue_Team.fbx";
    const string PalettePath = "Assets/GDTV Sharp Shooter Assets/Textures & Materials/Sci Fi/Colour Palette.mat";

    static readonly Color CPlayer = new Color(0.53f, 0.81f, 0.92f);

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
        player.tag = "Player";
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
        aim.crosshair = null;

        Health health = player.AddComponent<Health>();
        health.maxHealth = 100f;

        GameObject gun = AttachGun(player, anim);

        GameObject muzzleObj = new GameObject("Muzzle");
        muzzleObj.transform.SetParent(gun != null ? gun.transform : player.transform, false);
        muzzleObj.transform.localPosition = gun != null ? new Vector3(0f, 0f, 0.55f) : new Vector3(0.25f, 1.3f, 0.6f);

        Light muzzleLight = muzzleObj.AddComponent<Light>();
        muzzleLight.type = LightType.Point;
        muzzleLight.color = new Color(1f, 0.85f, 0.5f);
        muzzleLight.range = 6f;
        muzzleLight.intensity = 4f;

        GameObject spark = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        spark.name = "FlashSpark";
        Object.DestroyImmediate(spark.GetComponent<Collider>());
        spark.transform.SetParent(muzzleObj.transform, false);
        spark.transform.localPosition = Vector3.zero;
        spark.transform.localScale = Vector3.one * 0.12f;
        spark.GetComponent<Renderer>().sharedMaterial = FlashMat();

        MuzzleFlash muzzle = muzzleObj.AddComponent<MuzzleFlash>();

        PlayerShoot shoot = player.AddComponent<PlayerShoot>();
        shoot.damage = 18f;
        shoot.range = 120f;
        shoot.fireCooldown = 0.1f;
        shoot.maxAmmo = 30;
        shoot.reloadTime = 1.8f;
        shoot.muzzle = muzzle;

        Crosshair crosshair = player.AddComponent<Crosshair>();
        crosshair.shoot = shoot;
        crosshair.hideWhileAiming = false;

        HUD hud = player.AddComponent<HUD>();
        hud.health = health;
        hud.shoot = shoot;

        player.AddComponent<GlitchTester>();

        Undo.RegisterCreatedObjectUndo(player, "Build Player");
        Selection.activeGameObject = player;
        EditorSceneManager.MarkSceneDirty(player.scene);
        Debug.Log("[PlayerBuilder] Animated player built from Mixamo character.");
    }

    static GameObject AttachGun(GameObject player, Animator anim)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(GunPath);
        if (asset == null)
        {
            Debug.LogWarning("[PlayerBuilder] Gun model not found at " + GunPath);
            return null;
        }

        Transform hand = anim != null ? anim.GetBoneTransform(HumanBodyBones.RightHand) : null;
        Transform parent = hand != null ? hand : player.transform;

        GameObject gun = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        PrefabUtility.UnpackPrefabInstance(gun, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        gun.name = "Gun";
        gun.transform.SetParent(parent, false);
        gun.transform.localPosition = hand != null ? new Vector3(0.03f, 0.02f, 0.05f) : new Vector3(0.25f, 1.3f, 0.35f);
        gun.transform.localRotation = Quaternion.identity;

        foreach (Collider c in gun.GetComponentsInChildren<Collider>())
            Object.DestroyImmediate(c);

        Material palette = AssetDatabase.LoadAssetAtPath<Material>(PalettePath);
        if (palette != null)
        {
            foreach (Renderer r in gun.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = r.sharedMaterials;
                bool changed = false;
                for (int i = 0; i < mats.Length; i++)
                    if (mats[i] == null) { mats[i] = palette; changed = true; }
                if (changed) r.sharedMaterials = mats;
            }
        }

        return gun;
    }

    internal static Material FlashMat()
    {
        string path = "Assets/Materials/MuzzleFlash.mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            if (!AssetDatabase.IsValidFolder("Assets/Materials"))
                AssetDatabase.CreateFolder("Assets", "Materials");
            Shader sh = Shader.Find("Universal Render Pipeline/Lit");
            if (sh == null) sh = Shader.Find("Standard");
            m = new Material(sh);
            Color c = new Color(1f, 0.9f, 0.5f);
            m.color = c;
            m.SetColor("_BaseColor", c);
            m.EnableKeyword("_EMISSION");
            m.globalIlluminationFlags = MaterialGlobalIlluminationFlags.RealtimeEmissive;
            m.SetColor("_EmissionColor", c * 4f);
            AssetDatabase.CreateAsset(m, path);
        }
        return m;
    }
}
