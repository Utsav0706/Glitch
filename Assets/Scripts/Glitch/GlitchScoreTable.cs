using System.Collections.Generic;

public static class GlitchScoreTable
{
    static readonly Dictionary<GlitchType, Dictionary<string, float>> table = new Dictionary<GlitchType, Dictionary<string, float>>
    {
        { GlitchType.GravityFlip, new Dictionary<string, float>
        {
            { "TakeCover", 82f }, { "Reorient", 75f }, { "Attack", 40f }, { "Chase", 15f },
        } },
        { GlitchType.EnemyDuplicate, new Dictionary<string, float>
        {
            { "Attack", 78f }, { "Reposition", 65f },
        } },
        { GlitchType.WallDisappear, new Dictionary<string, float>
        {
            { "Chase", 85f }, { "TakeCover", 30f },
        } },
        { GlitchType.TimeStutter, new Dictionary<string, float>
        {
            { "Freeze", 100f },
        } },
        { GlitchType.PlayerDuplicate, new Dictionary<string, float>
        {
            { "SpreadFire", 70f }, { "Attack", 55f }, { "Retreat", 45f },
        } },
    };

    public static float Get(GlitchType glitch, string actionKey)
    {
        if (table.TryGetValue(glitch, out var actions) && actions.TryGetValue(actionKey, out var score))
            return score;
        return 0f;
    }
}
