using System.Collections.Generic;
using UnityEngine;

public class UtilityBrain
{
    public float evaluateInterval = 0.25f;

    readonly List<UtilityAction> actions = new List<UtilityAction>();
    UtilityAction current;
    float nextEvaluate;

    public UtilityBrain(float evaluateInterval = 0.25f)
    {
        this.evaluateInterval = evaluateInterval;
    }

    public IReadOnlyList<UtilityAction> Actions => actions;
    public UtilityAction Current => current;
    public string CurrentName => current != null ? current.Name : "None";

    public void Add(UtilityAction action)
    {
        if (action != null) actions.Add(action);
    }

    public void Tick()
    {
        if (Time.time >= nextEvaluate)
        {
            nextEvaluate = Time.time + evaluateInterval;
            Evaluate();
        }

        current?.Execute();
    }

    void Evaluate()
    {
        UtilityAction best = null;
        float bestScore = float.NegativeInfinity;

        for (int i = 0; i < actions.Count; i++)
        {
            float score = actions[i].Evaluate();
            if (score > bestScore)
            {
                bestScore = score;
                best = actions[i];
            }
        }

        if (best != current)
        {
            current?.OnExit();
            current = best;
            current?.OnEnter();
        }
    }
}
