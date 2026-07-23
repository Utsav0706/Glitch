using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class CoverBuilder
{
    const string RootName = "CoverPoints";
    const float Standoff = 0.9f;
    const float MinPropHeight = 0.6f;
    const float ClearRadius = 0.45f;

    [MenuItem("GLITCH/Cover/Build Cover Points", priority = 80)]
    public static void BuildCoverPoints()
    {
        GameObject old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);

        Transform props = FindProps();
        if (props == null)
        {
            Debug.LogError("[CoverBuilder] Arena 'Props' group not found. Build the arena first.");
            return;
        }

        GameObject root = new GameObject(RootName);
        Vector3[] dirs = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        int count = 0;

        foreach (Transform prop in props)
        {
            Collider col = prop.GetComponent<Collider>();
            if (col == null) continue;

            Bounds b = col.bounds;
            if (b.size.y < MinPropHeight) continue;

            for (int i = 0; i < dirs.Length; i++)
            {
                Vector3 d = dirs[i];
                float extent = Mathf.Abs(d.z) > 0f ? b.extents.z : b.extents.x;
                Vector3 pos = new Vector3(b.center.x, 0f, b.center.z) + d * (extent + Standoff);

                if (Physics.CheckSphere(pos + Vector3.up * 0.6f, ClearRadius, ~0, QueryTriggerInteraction.Ignore))
                    continue;

                count++;
                GameObject go = new GameObject("Cover " + count);
                go.transform.SetParent(root.transform, false);
                go.transform.position = pos;
                go.transform.rotation = Quaternion.LookRotation(-d);

                CoverPoint cp = go.AddComponent<CoverPoint>();
                cp.coverHeight = Mathf.Clamp(b.max.y * 0.8f, 0.5f, 1.5f);
            }
        }

        Undo.RegisterCreatedObjectUndo(root, "Build Cover Points");
        Selection.activeGameObject = root;
        EditorSceneManager.MarkSceneDirty(root.scene);
        Debug.Log("[CoverBuilder] " + count + " cover points placed.");
    }

    static Transform FindProps()
    {
        GameObject arena = GameObject.Find("Arena");
        if (arena == null) return null;
        return arena.transform.Find("Props");
    }
}
