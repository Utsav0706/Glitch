using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class EnemyBuilder
{
    const string RootName = "Enemies";

    static readonly Color CDummy = new Color(0.85f, 0.25f, 0.22f);

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
