using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public static class ArenaBuilder
{
    const string ArenaRootName = "Arena";
    const float FloorSize = 40f;
    const float Thickness = 0.5f;
    const float WallHeight = 5f;

    
    [MenuItem("GLITCH/Arena/1. Build Floor", priority = 1)]
    public static void BuildFloor()
    {
        Transform root = GetOrCreateRoot();

       
        Transform old = root.Find("Floor");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        GameObject floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floor.name = "Floor";
        floor.transform.SetParent(root, false);
        floor.transform.localScale = new Vector3(FloorSize, Thickness, FloorSize);
      
        floor.transform.localPosition = new Vector3(0f, -Thickness * 0.5f, 0f);

        GameObjectUtility.SetStaticEditorFlags(floor,
            StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic |
            StaticEditorFlags.BatchingStatic);

        Undo.RegisterCreatedObjectUndo(floor, "Build Arena Floor");
        Selection.activeGameObject = floor;
        EditorSceneManager.MarkSceneDirty(floor.scene);
        Debug.Log("[ArenaBuilder] Floor built (" + FloorSize + " x " + FloorSize + " m).");
    }

    [MenuItem("GLITCH/Arena/2. Build Outer Walls", priority = 2)]
    public static void BuildOuterWalls()
    {
        Transform root = GetOrCreateRoot();

        Transform old = root.Find("OuterWalls");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        GameObject group = new GameObject("OuterWalls");
        group.transform.SetParent(root, false);
        Undo.RegisterCreatedObjectUndo(group, "Build Outer Walls");

        float half = FloorSize * 0.5f;
        float offset = half - Thickness * 0.5f;
        float y = WallHeight * 0.5f;

        CreateBox("Wall_North", group.transform, new Vector3(0f, y, offset), new Vector3(FloorSize, WallHeight, Thickness));
        CreateBox("Wall_South", group.transform, new Vector3(0f, y, -offset), new Vector3(FloorSize, WallHeight, Thickness));
        CreateBox("Wall_East", group.transform, new Vector3(offset, y, 0f), new Vector3(Thickness, WallHeight, FloorSize));
        CreateBox("Wall_West", group.transform, new Vector3(-offset, y, 0f), new Vector3(Thickness, WallHeight, FloorSize));

        Selection.activeGameObject = group;
        EditorSceneManager.MarkSceneDirty(group.scene);
        Debug.Log("[ArenaBuilder] Outer walls built.");
    }

    static GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 localScale)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = localPos;
        box.transform.localScale = localScale;
        GameObjectUtility.SetStaticEditorFlags(box,
            StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic |
            StaticEditorFlags.BatchingStatic);
        return box;
    }

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
