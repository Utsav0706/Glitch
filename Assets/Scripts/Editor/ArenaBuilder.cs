using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

/// <summary>
/// GLITCH greybox arena generator.
///
/// Builds the indoor military-zone test arena piece by piece from menu commands
/// (GLITCH ▸ Arena ▸ ...). Every piece is parented under a single "Arena" root
/// and each build step is idempotent — re-running a command replaces only its
/// own geometry, so the arena can be regenerated safely at any time.
///
/// This is a deliberately ugly greybox: primitive cubes only. Visuals come last
/// (per the project blueprint). The geometry exists so the NavMesh can be baked
/// and the FSM / Utility AI enemies have somewhere to fight.
/// </summary>
public static class ArenaBuilder
{
    // ---- Shared arena dimensions (units = metres) ----
    const string ArenaRootName = "Arena";
    const float FloorSize = 40f;   // X/Z extent of the arena floor
    const float Thickness = 0.5f;  // slab thickness for floor / walls / ceiling

    // ---------------------------------------------------------------------
    // Step 1 — Floor
    // ---------------------------------------------------------------------
    [MenuItem("GLITCH/Arena/1. Build Floor", priority = 1)]
    public static void BuildFloor()
    {
        Transform root = GetOrCreateRoot();

        // Idempotent: drop any previous floor before rebuilding.
        Transform old = root.Find("Floor");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(root, false);
        floor.transform.localScale = new Vector3(FloorSize, Thickness, FloorSize);
        // Sink the slab so its top face sits exactly at y = 0.
        floor.transform.localPosition = new Vector3(0f, -Thickness * 0.5f, 0f);

        // Static so it participates in NavMesh baking and occlusion later.
        GameObjectUtility.SetStaticEditorFlags(floor,
            StaticEditorFlags.NavigationStatic | StaticEditorFlags.OccluderStatic |
            StaticEditorFlags.OccludeeStatic | StaticEditorFlags.BatchingStatic);

        Undo.RegisterCreatedObjectUndo(floor, "Build Arena Floor");
        Selection.activeGameObject = floor;
        EditorSceneManager.MarkSceneDirty(floor.scene);
        Debug.Log("[ArenaBuilder] Floor built (" + FloorSize + " x " + FloorSize + " m).");
    }

    // ---------------------------------------------------------------------
    // Helpers
    // ---------------------------------------------------------------------
    static Transform GetOrCreateRoot()
    {
        GameObject root = GameObject.Find(ArenaRootName);
        if (root == null)
        {
            root = new GameObject(ArenaRootName);
            Undo.RegisterCreatedObjectUndo(root, "Create Arena Root");
        }
        return root.transform;
    }
}
