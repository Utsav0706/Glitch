using System.Collections.Generic;
using UnityEngine;

public static class PlayerCopies
{
    static readonly List<Transform> decoys = new List<Transform>();

    public static void Register(Transform decoy)
    {
        if (decoy != null && !decoys.Contains(decoy)) decoys.Add(decoy);
    }

    public static void Unregister(Transform decoy)
    {
        decoys.Remove(decoy);
    }

    public static int Count
    {
        get
        {
            Prune();
            int c = decoys.Count + (Player() != null ? 1 : 0);
            return Mathf.Max(1, c);
        }
    }

    public static Transform Random()
    {
        List<Transform> all = All();
        return all.Count == 0 ? null : all[UnityEngine.Random.Range(0, all.Count)];
    }

    public static List<Transform> All()
    {
        Prune();
        List<Transform> list = new List<Transform>();
        Transform p = Player();
        if (p != null) list.Add(p);
        list.AddRange(decoys);
        return list;
    }

    static Transform Player()
    {
        GameObject go = GameObject.FindGameObjectWithTag("Player");
        return go != null ? go.transform : null;
    }

    static void Prune()
    {
        for (int i = decoys.Count - 1; i >= 0; i--)
            if (decoys[i] == null) decoys.RemoveAt(i);
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    static void Reset()
    {
        decoys.Clear();
    }
}
