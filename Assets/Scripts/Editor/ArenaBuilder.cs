using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;


public static class ArenaBuilder
{
    const string ArenaRootName = "Arena";
    const float FloorSize = 40f;   
    const float Thickness = 0.5f; 

    
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
