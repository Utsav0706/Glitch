using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EnemyBuilder
{
    const string RootName = "Enemies";
    const string FSMRootName = "FSM Enemies";

    const string RobotModelPath = "Assets/GDTV Sharp Shooter Assets/Models/Enemies/HoveringRobot02.fbx";
    const string PalettePath = "Assets/GDTV Sharp Shooter Assets/Textures & Materials/Sci Fi/Colour Palette.mat";
    const float HoverGap = 0.35f;

    [MenuItem("GLITCH/Enemies/Build Dummy Targets", priority = 60)]
    public static void BuildDummies()
    {
        GameObject old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        GameObject root = new GameObject(RootName);

        Vector3[] spots =
        {
            new Vector3(-8f, 0f, -34f),
            new Vector3(0f, 0f, -34f),
            new Vector3(8f, 0f, -34f),
        };

        for (int i = 0; i < spots.Length; i++)
            CreateDummy(root.transform, OnNavMesh(spots[i]), i);

        Undo.RegisterCreatedObjectUndo(root, "Build Dummy Targets");
        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[EnemyBuilder] Dummy targets built.");
    }

    [MenuItem("GLITCH/Enemies/Build FSM Enemies", priority = 61)]
    public static void BuildFSMEnemies()
    {
        GameObject old = GameObject.Find(FSMRootName);
        if (old != null) Object.DestroyImmediate(old);

        GameObject root = new GameObject(FSMRootName);

        Vector3[] spots =
        {
            new Vector3(-40f, 0f, 40f),
            new Vector3(40f, 0f, 40f),
            new Vector3(0f, 0f, 42f),
            new Vector3(42f, 0f, -15f),
            new Vector3(-42f, 0f, 15f),
            new Vector3(28f, 0f, -42f),
            new Vector3(-28f, 0f, -42f),
            new Vector3(-7.5f, 0f, 7.5f),
            new Vector3(7.5f, 0f, -7.5f),
            new Vector3(22.5f, 0f, 22.5f),
        };

        for (int i = 0; i < spots.Length; i++)
            CreateFSMEnemy(root.transform, OnNavMesh(spots[i]), i);

        Undo.RegisterCreatedObjectUndo(root, "Build FSM Enemies");
        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[EnemyBuilder] " + spots.Length + " FSM enemies built.");
    }

    static void CreateFSMEnemy(Transform parent, Vector3 pos, int index)
    {
        GameObject enemy = NewRobotEnemy(parent, "FSM Enemy " + (index + 1), pos, out float height, out float radius, out float centerY);

        NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
        agent.radius = radius;
        agent.height = height;
        agent.speed = 3.5f;
        agent.angularSpeed = 720f;
        agent.acceleration = 12f;
        agent.stoppingDistance = 1.5f;

        Health health = enemy.AddComponent<Health>();
        health.maxHealth = 80f;

        GameObject muzzleObj = new GameObject("Muzzle");
        muzzleObj.transform.SetParent(enemy.transform, false);
        muzzleObj.transform.localPosition = new Vector3(0f, centerY, radius + 0.15f);

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
        spark.transform.localScale = Vector3.one * 0.15f;
        spark.GetComponent<Renderer>().sharedMaterial = PlayerBuilder.FlashMat();

        MuzzleFlash muzzle = muzzleObj.AddComponent<MuzzleFlash>();

        EnemyPerception perception = enemy.AddComponent<EnemyPerception>();
        perception.eyeHeight = centerY;

        EnemyWeaponController weapon = enemy.AddComponent<EnemyWeaponController>();
        weapon.muzzle = muzzle;
        weapon.damage = 8f;
        weapon.range = 30f;
        weapon.fireRate = 1.5f;
        weapon.spreadDegrees = 4f;
        weapon.muzzleHeight = centerY;

        enemy.AddComponent<FSMEnemy>();
    }

    static void CreateDummy(Transform parent, Vector3 pos, int index)
    {
        GameObject dummy = NewRobotEnemy(parent, "Dummy " + (index + 1), pos, out float _, out float _, out float _);

        Health health = dummy.AddComponent<Health>();
        health.maxHealth = 60f;

        dummy.AddComponent<TargetDummy>();
    }

    static GameObject NewRobotEnemy(Transform parent, string name, Vector3 pos, out float height, out float radius, out float centerY)
    {
        height = 2f;
        radius = 0.4f;
        centerY = 1f;

        GameObject root = new GameObject(name);
        root.transform.SetParent(parent, false);
        root.transform.localPosition = pos;

        GameObject model = AttachRobotModel(root.transform);
        if (model != null)
        {
            Bounds b = CombinedBounds(model);
            height = Mathf.Max(0.5f, b.size.y);
            radius = Mathf.Clamp(Mathf.Max(b.extents.x, b.extents.z), 0.2f, height * 0.5f);

            float bottomLocal = b.min.y - root.transform.position.y;
            model.transform.localPosition += new Vector3(0f, HoverGap - bottomLocal, 0f);

            centerY = HoverGap + height * 0.5f;
        }

        CapsuleCollider col = root.AddComponent<CapsuleCollider>();
        col.center = new Vector3(0f, centerY, 0f);
        col.height = height;
        col.radius = radius;

        return root;
    }

    static GameObject AttachRobotModel(Transform root)
    {
        GameObject asset = AssetDatabase.LoadAssetAtPath<GameObject>(RobotModelPath);
        if (asset == null)
        {
            Debug.LogError("[EnemyBuilder] Robot model not found at " + RobotModelPath);
            return null;
        }

        GameObject model = (GameObject)PrefabUtility.InstantiatePrefab(asset);
        PrefabUtility.UnpackPrefabInstance(model, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
        model.name = "Model";
        model.transform.SetParent(root, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;

        foreach (Collider c in model.GetComponentsInChildren<Collider>())
            Object.DestroyImmediate(c);

        Material palette = AssetDatabase.LoadAssetAtPath<Material>(PalettePath);
        if (palette != null)
        {
            foreach (Renderer r in model.GetComponentsInChildren<Renderer>())
            {
                Material[] mats = r.sharedMaterials;
                for (int i = 0; i < mats.Length; i++) mats[i] = palette;
                r.sharedMaterials = mats;
            }
        }
        else
        {
            Debug.LogWarning("[EnemyBuilder] Colour Palette material not found at " + PalettePath);
        }

        return model;
    }

    static Bounds CombinedBounds(GameObject go)
    {
        Renderer[] rs = go.GetComponentsInChildren<Renderer>();
        if (rs.Length == 0) return new Bounds(go.transform.position, Vector3.one);

        Bounds b = rs[0].bounds;
        for (int i = 1; i < rs.Length; i++) b.Encapsulate(rs[i].bounds);
        return b;
    }

    static Vector3 OnNavMesh(Vector3 p)
    {
        if (NavMesh.SamplePosition(p, out NavMeshHit hit, 8f, NavMesh.AllAreas))
            return hit.position;
        return p;
    }
}
