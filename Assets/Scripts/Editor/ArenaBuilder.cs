using System;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class ArenaBuilder
{
    const string ArenaRootName = "Arena";
    const float YardSize = 100f;
    const float BuildingSize = 60f;
    const int Cols = 4;
    const int Rows = 4;
    const float Thickness = 0.4f;
    const float WallHeight = 5f;
    const float PerimeterHeight = 6f;
    const float DoorWidth = 3.5f;
    const float GateWidth = 8f;
    const float TowerHeight = 6f;

    static readonly Color CFloor = new Color(0.20f, 0.21f, 0.23f);
    static readonly Color CPerimeter = new Color(0.42f, 0.45f, 0.30f);
    static readonly Color CExt = new Color(0.44f, 0.48f, 0.52f);
    static readonly Color CInt = new Color(0.62f, 0.63f, 0.65f);
    static readonly Color CRoof = new Color(0.28f, 0.30f, 0.33f);
    static readonly Color CTower = new Color(0.40f, 0.30f, 0.20f);
    static readonly Color CRail = new Color(0.18f, 0.18f, 0.20f);
    static readonly Color CCrate = new Color(0.58f, 0.38f, 0.16f);
    static readonly Color CSandbag = new Color(0.64f, 0.57f, 0.35f);
    static readonly Color CBarrel = new Color(0.60f, 0.20f, 0.16f);

    [MenuItem("GLITCH/Arena/Build Full Arena", priority = 0)]
    public static void BuildFullArena()
    {
        Transform root = GetOrCreateRoot();
        ClearChildren(root);
        BuildFloor();
        BuildPerimeter();
        BuildBuilding();
        BuildWatchtowers();
        BuildProps();
        AssetDatabase.SaveAssets();
        Selection.activeGameObject = root.gameObject;
        EditorSceneManager.MarkSceneDirty(root.gameObject.scene);
        Debug.Log("[ArenaBuilder] Full arena built.");
    }

    [MenuItem("GLITCH/Arena/Floor", priority = 20)]
    public static void BuildFloor()
    {
        Transform g = NewGroup("Floor");
        CreateBox("Slab", g, new Vector3(0f, -Thickness * 0.5f, 0f), new Vector3(YardSize, Thickness, YardSize), CFloor);
    }

    [MenuItem("GLITCH/Arena/Perimeter", priority = 21)]
    public static void BuildPerimeter()
    {
        Transform g = NewGroup("Perimeter");
        float off = YardSize * 0.5f - Thickness * 0.5f;
        BuildWallWithGaps(g, "North", false, off, -YardSize * 0.5f, YardSize * 0.5f, PerimeterHeight, null, 0f, CPerimeter);
        BuildWallWithGaps(g, "South", false, -off, -YardSize * 0.5f, YardSize * 0.5f, PerimeterHeight, new[] { 0f }, GateWidth, CPerimeter);
        BuildWallWithGaps(g, "East", true, off, -YardSize * 0.5f, YardSize * 0.5f, PerimeterHeight, null, 0f, CPerimeter);
        BuildWallWithGaps(g, "West", true, -off, -YardSize * 0.5f, YardSize * 0.5f, PerimeterHeight, null, 0f, CPerimeter);
    }

    [MenuItem("GLITCH/Arena/Building", priority = 22)]
    public static void BuildBuilding()
    {
        Transform g = NewGroup("Building");
        float bhalf = BuildingSize * 0.5f;
        float boff = bhalf - Thickness * 0.5f;

        BuildWallWithGaps(g, "Ext_North", false, boff, -bhalf, bhalf, WallHeight, null, 0f, CExt);
        BuildWallWithGaps(g, "Ext_South", false, -boff, -bhalf, bhalf, WallHeight, new[] { 0f }, DoorWidth, CExt);
        BuildWallWithGaps(g, "Ext_East", true, boff, -bhalf, bhalf, WallHeight, null, 0f, CExt);
        BuildWallWithGaps(g, "Ext_West", true, -boff, -bhalf, bhalf, WallHeight, null, 0f, CExt);

        float[] rowCenters = RoomCenters(Rows);
        float[] colCenters = RoomCenters(Cols);

        for (int i = 1; i < Cols; i++)
        {
            float x = -bhalf + (BuildingSize / Cols) * i;
            BuildWallWithGaps(g, "Int_V" + i, true, x, -bhalf, bhalf, WallHeight, rowCenters, DoorWidth, CInt);
        }
        for (int j = 1; j < Rows; j++)
        {
            float z = -bhalf + (BuildingSize / Rows) * j;
            BuildWallWithGaps(g, "Int_H" + j, false, z, -bhalf, bhalf, WallHeight, colCenters, DoorWidth, CInt);
        }

        CreateBox("Roof", g, new Vector3(0f, WallHeight + Thickness * 0.5f, 0f), new Vector3(BuildingSize, Thickness, BuildingSize), CRoof);

        GameObject stairs = new GameObject("RoofStairs");
        stairs.transform.SetParent(g, false);
        stairs.transform.localPosition = new Vector3(21.5f, 0f, -26.5f);
        BuildFlight(stairs.transform, 0f, WallHeight + Thickness, true);

        float railY = WallHeight + Thickness + 0.5f;
        CreateBox("RoofRail_N", g, new Vector3(0f, railY, 29.8f), new Vector3(59.6f, 1f, 0.2f), CRail);
        CreateBox("RoofRail_E", g, new Vector3(29.8f, railY, 0f), new Vector3(0.2f, 1f, 59.6f), CRail);
        CreateBox("RoofRail_W", g, new Vector3(-29.8f, railY, 0f), new Vector3(0.2f, 1f, 59.6f), CRail);
        CreateBox("RoofRail_S0", g, new Vector3(-2.9f, railY, -29.8f), new Vector3(53.8f, 1f, 0.2f), CRail);
        CreateBox("RoofRail_S1", g, new Vector3(27.6f, railY, -29.8f), new Vector3(4.4f, 1f, 0.2f), CRail);
    }

    [MenuItem("GLITCH/Arena/Watchtowers", priority = 23)]
    public static void BuildWatchtowers()
    {
        Transform g = NewGroup("Watchtowers");
        float c = YardSize * 0.5f - 8f;
        BuildTower(g, "Tower_NE", new Vector3(c, 0f, c), 0f, 2);
        BuildTower(g, "Tower_NW", new Vector3(-c, 0f, c), 0f, 1);
        BuildTower(g, "Tower_SE", new Vector3(c, 0f, -c), 180f, 1);
        BuildTower(g, "Tower_SW", new Vector3(-c, 0f, -c), 180f, 2);
    }

    [MenuItem("GLITCH/Arena/Props", priority = 24)]
    public static void BuildProps()
    {
        Transform g = NewGroup("Props");
        System.Random rng = new System.Random(12345);

        float[] rowCenters = RoomCenters(Rows);
        float[] colCenters = RoomCenters(Cols);
        foreach (float rz in rowCenters)
        {
            foreach (float cx in colCenters)
            {
                float ox = cx + (rng.Next(0, 2) == 0 ? -4f : 4f);
                float oz = rz + (rng.Next(0, 2) == 0 ? -4f : 4f);
                CreateBox("Crate", g, new Vector3(ox, 0.7f, oz), new Vector3(1.4f, 1.4f, 1.4f), CCrate);
                if (rng.Next(0, 3) == 0)
                    CreateBox("Crate", g, new Vector3(ox, 2.1f, oz), new Vector3(1.4f, 1.4f, 1.4f), CCrate);
            }
        }

        int placed = 0;
        int attempts = 0;
        while (placed < 40 && attempts < 400)
        {
            attempts++;
            float x = (float)(rng.NextDouble() * (YardSize - 6f) - (YardSize - 6f) * 0.5f);
            float z = (float)(rng.NextDouble() * (YardSize - 6f) - (YardSize - 6f) * 0.5f);
            if (Mathf.Abs(x) < BuildingSize * 0.5f + 3f && Mathf.Abs(z) < BuildingSize * 0.5f + 3f) continue;
            if (Mathf.Abs(x) < GateWidth && z < -BuildingSize * 0.5f) continue;

            int type = placed % 3;
            if (type == 0)
                CreateBox("Crate", g, new Vector3(x, 0.7f, z), new Vector3(1.4f, 1.4f, 1.4f), CCrate);
            else if (type == 1)
                CreateBox("Sandbag", g, new Vector3(x, 0.5f, z), new Vector3(4f, 1f, 1f), CSandbag);
            else
                CreateCylinder("Barrel", g, new Vector3(x, 0.6f, z), new Vector3(1f, 0.6f, 1f), CBarrel);
            placed++;
        }
    }

    static void BuildTower(Transform parent, string name, Vector3 center, float yaw, int floors)
    {
        GameObject t = new GameObject(name);
        t.transform.SetParent(parent, false);
        t.transform.localPosition = center;
        t.transform.localRotation = Quaternion.Euler(0f, yaw, 0f);
        float s = 2.5f;
        float legH = TowerHeight * floors;
        CreateBox("Leg_A", t.transform, new Vector3(s, legH * 0.5f, s), new Vector3(0.5f, legH, 0.5f), CTower);
        CreateBox("Leg_B", t.transform, new Vector3(-s, legH * 0.5f, s), new Vector3(0.5f, legH, 0.5f), CTower);
        CreateBox("Leg_C", t.transform, new Vector3(s, legH * 0.5f, -s), new Vector3(0.5f, legH, 0.5f), CTower);
        CreateBox("Leg_D", t.transform, new Vector3(-s, legH * 0.5f, -s), new Vector3(0.5f, legH, 0.5f), CTower);

        for (int f = 1; f <= floors; f++)
        {
            GameObject level = new GameObject("Level_" + f);
            level.transform.SetParent(t.transform, false);
            level.transform.localRotation = Quaternion.Euler(0f, -90f * (f - 1), 0f);
            float deckY = TowerHeight * f;
            float fromY = f == 1 ? 0f : TowerHeight * (f - 1) + Thickness * 0.5f;
            BuildDeck(level.transform, deckY);
            BuildFlight(level.transform, fromY, deckY + Thickness * 0.5f, f == 1);
        }
    }

    static void BuildDeck(Transform parent, float deckY)
    {
        float railY = deckY + 0.75f;
        CreateBox("Deck", parent, new Vector3(0f, deckY, 0f), new Vector3(7f, Thickness, 7f), CTower);
        CreateBox("Rail_N", parent, new Vector3(0f, railY, 3.3f), new Vector3(7f, 1.5f, 0.2f), CRail);
        CreateBox("Rail_E", parent, new Vector3(3.3f, railY, 0f), new Vector3(0.2f, 1.5f, 7f), CRail);
        CreateBox("Rail_W", parent, new Vector3(-3.3f, railY, 0f), new Vector3(0.2f, 1.5f, 7f), CRail);
        CreateBox("Rail_S", parent, new Vector3(-0.7f, railY, -3.3f), new Vector3(5.6f, 1.5f, 0.2f), CRail);
    }

    static void BuildFlight(Transform parent, float fromY, float toY, bool solid)
    {
        int steps = 24;
        float depth = 0.375f;
        float width = 2.2f;
        float xEnd = 3.5f;
        float zC = -4.6f;
        float rise = toY - fromY;
        float h = rise / steps;

        for (int j = 1; j <= steps; j++)
        {
            float top = fromY + h * j;
            float x = xEnd - (steps - j + 0.5f) * depth;
            if (solid)
                CreateBox("Step_" + j, parent, new Vector3(x, top * 0.5f, zC), new Vector3(depth, top, width), CTower);
            else
                CreateBox("Step_" + j, parent, new Vector3(x, top - 0.2f, zC), new Vector3(depth, 0.4f, width), CTower);
        }

        float run = steps * depth;
        float rampLen = Mathf.Sqrt(run * run + rise * rise);
        float angle = Mathf.Atan2(rise, run) * Mathf.Rad2Deg;
        GameObject ramp = CreateBox("Ramp", parent, new Vector3(xEnd - run * 0.5f, (fromY + toY) * 0.5f, zC), new Vector3(rampLen, 0.06f, width), CTower);
        ramp.transform.localRotation = Quaternion.Euler(0f, 0f, angle);
        ramp.GetComponent<MeshRenderer>().enabled = false;
    }

    static void BuildWallWithGaps(Transform parent, string name, bool vertical, float fixedCoord, float from, float to, float height, float[] gapCenters, float gapWidth, Color color)
    {
        float y = height * 0.5f;
        float start = from;
        int seg = 0;

        float[] gaps = gapCenters ?? new float[0];
        float[] sorted = (float[])gaps.Clone();
        Array.Sort(sorted);

        for (int k = 0; k < sorted.Length; k++)
        {
            float a = sorted[k] - gapWidth * 0.5f;
            float b = sorted[k] + gapWidth * 0.5f;
            if (a > start)
                AddSegment(parent, name + "_" + seg++, vertical, fixedCoord, start, a, height, y, color);
            start = Mathf.Max(start, b);
        }
        if (start < to)
            AddSegment(parent, name + "_" + seg++, vertical, fixedCoord, start, to, height, y, color);
    }

    static void AddSegment(Transform parent, string name, bool vertical, float fixedCoord, float a, float b, float height, float y, Color color)
    {
        float length = b - a;
        if (length <= 0.01f) return;
        float mid = (a + b) * 0.5f;
        if (vertical)
            CreateBox(name, parent, new Vector3(fixedCoord, y, mid), new Vector3(Thickness, height, length), color);
        else
            CreateBox(name, parent, new Vector3(mid, y, fixedCoord), new Vector3(length, height, Thickness), color);
    }

    static float[] RoomCenters(int n)
    {
        float w = BuildingSize / n;
        float half = BuildingSize * 0.5f;
        float[] c = new float[n];
        for (int i = 0; i < n; i++) c[i] = -half + w * (i + 0.5f);
        return c;
    }

    static GameObject CreateBox(string name, Transform parent, Vector3 localPos, Vector3 localScale, Color color)
    {
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.name = name;
        box.transform.SetParent(parent, false);
        box.transform.localPosition = localPos;
        box.transform.localScale = localScale;
        box.GetComponent<Renderer>().sharedMaterial = Mat(color);
        GameObjectUtility.SetStaticEditorFlags(box,
            StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic |
            StaticEditorFlags.BatchingStatic);
        return box;
    }

    static GameObject CreateCylinder(string name, Transform parent, Vector3 localPos, Vector3 localScale, Color color)
    {
        GameObject cyl = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        cyl.name = name;
        cyl.transform.SetParent(parent, false);
        cyl.transform.localPosition = localPos;
        cyl.transform.localScale = localScale;
        cyl.GetComponent<Renderer>().sharedMaterial = Mat(color);
        GameObjectUtility.SetStaticEditorFlags(cyl,
            StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic |
            StaticEditorFlags.BatchingStatic);
        return cyl;
    }

    internal static Material Mat(Color color)
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");

        string hex = ColorUtility.ToHtmlStringRGB(color);
        string path = "Assets/Materials/Arena_" + hex + ".mat";
        Material m = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (m == null)
        {
            Shader sh = Shader.Find("Universal Render Pipeline/Lit");
            if (sh == null) sh = Shader.Find("Standard");
            m = new Material(sh);
            m.SetColor("_BaseColor", color);
            m.color = color;
            AssetDatabase.CreateAsset(m, path);
        }
        return m;
    }

    static Transform NewGroup(string name)
    {
        Transform root = GetOrCreateRoot();
        Transform old = root.Find(name);
        if (old != null) UnityEngine.Object.DestroyImmediate(old.gameObject);
        GameObject g = new GameObject(name);
        g.transform.SetParent(root, false);
        Undo.RegisterCreatedObjectUndo(g, "Build " + name);
        return g.transform;
    }

    static void ClearChildren(Transform root)
    {
        for (int i = root.childCount - 1; i >= 0; i--)
            UnityEngine.Object.DestroyImmediate(root.GetChild(i).gameObject);
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
