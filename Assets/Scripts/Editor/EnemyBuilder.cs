using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EnemyBuilder
{
    const string RootName = "Enemies";
    const string FSMRootName = "FSM Enemies";

    static readonly Color CDummy = new Color(0.85f, 0.25f, 0.22f);
    static readonly Color CFSM = new Color(0.95f, 0.55f, 0.12f);

    [MenuItem("GLITCH/Enemies/Build Dummy Targets", priority = 60)]
    public static void BuildDummies()
    {
        GameObject old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        GameObject root = new GameObject(RootName);

        Vector3[] spots =
        {
            new Vector3(-8f, 1f, -12f),
            new Vector3(0f, 1f, -6f),
            new Vector3(8f, 1f, -12f),
        };

        Material mat = ArenaBuilder.Mat(CDummy);
        for (int i = 0; i < spots.Length; i++)
            CreateDummy(root.transform, spots[i], mat, i);

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
            new Vector3(-20f, 1f, 10f),
            new Vector3(20f, 1f, 10f),
            new Vector3(0f, 1f, 28f),
        };

        Material mat = ArenaBuilder.Mat(CFSM);
        for (int i = 0; i < spots.Length; i++)
            CreateFSMEnemy(root.transform, spots[i], mat, i);

        Undo.RegisterCreatedObjectUndo(root, "Build FSM Enemies");
        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[EnemyBuilder] " + spots.Length + " FSM enemies built.");
    }

    static void CreateFSMEnemy(Transform parent, Vector3 pos, Material mat, int index)
    {
        GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        enemy.name = "FSM Enemy " + (index + 1);
        enemy.transform.SetParent(parent, false);
        enemy.transform.localPosition = pos;
        enemy.GetComponent<Renderer>().sharedMaterial = mat;

        NavMeshAgent agent = enemy.AddComponent<NavMeshAgent>();
        agent.radius = 0.4f;
        agent.height = 2f;
        agent.speed = 3.5f;
        agent.angularSpeed = 720f;
        agent.acceleration = 12f;
        agent.stoppingDistance = 1.5f;

        Health health = enemy.AddComponent<Health>();
        health.maxHealth = 80f;

        GameObject muzzleObj = new GameObject("Muzzle");
        muzzleObj.transform.SetParent(enemy.transform, false);
        muzzleObj.transform.localPosition = new Vector3(0f, 0.5f, 0.55f);

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
        perception.eyeHeight = 0.7f;

        EnemyWeaponController weapon = enemy.AddComponent<EnemyWeaponController>();
        weapon.muzzle = muzzle;
        weapon.damage = 8f;
        weapon.range = 30f;
        weapon.fireRate = 1.5f;
        weapon.spreadDegrees = 4f;
        weapon.muzzleHeight = 0.5f;

        enemy.AddComponent<FSMEnemy>();
    }

    static void CreateDummy(Transform parent, Vector3 pos, Material mat, int index)
    {
        GameObject dummy = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        dummy.name = "Dummy " + (index + 1);
        dummy.transform.SetParent(parent, false);
        dummy.transform.localPosition = pos;
        dummy.GetComponent<Renderer>().sharedMaterial = mat;

        Health health = dummy.AddComponent<Health>();
        health.maxHealth = 60f;

        dummy.AddComponent<TargetDummy>();
    }
}
