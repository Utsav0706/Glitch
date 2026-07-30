using UnityEngine;
using UnityEngine.AI;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.AI.Navigation;

public static class NavMeshBaker
{
    [MenuItem("GLITCH/Arena/Bake NavMesh", priority = 10)]
    public static void Bake()
    {
        GameObject arena = GameObject.Find("Arena");
        if (arena == null)
        {
            Debug.LogError("[NavMeshBaker] No 'Arena' object found. Build the arena first.");
            return;
        }

        NavMeshSurface surface = arena.GetComponent<NavMeshSurface>();
        if (surface == null) surface = arena.AddComponent<NavMeshSurface>();

        surface.collectObjects = CollectObjects.Children;
        surface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
        surface.BuildNavMesh();

        EditorUtility.SetDirty(surface);
        EditorSceneManager.MarkSceneDirty(arena.scene);
        Debug.Log("[NavMeshBaker] NavMesh baked from Arena geometry. Build enemies after this so they snap onto it.");
    }
}
