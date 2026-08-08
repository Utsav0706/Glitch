using UnityEngine;
using UnityEngine.SceneManagement;

public static class GameBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GlitchEvents.Reset();
        EnsureAll();
    }

    static void EnsureAll()
    {
        Ensure<GlitchManager>();
        Ensure<GravityFlipEffect>();
        Ensure<TimeStutterEffect>();
        Ensure<WallDisappearEffect>();
        Ensure<PlayerDuplicateEffect>();
        Ensure<EnemyDuplicateEffect>();
        Ensure<GameOverController>();
    }

    static void Ensure<T>() where T : MonoBehaviour
    {
        if (Object.FindFirstObjectByType<T>() == null)
            new GameObject(typeof(T).Name).AddComponent<T>();
    }
}
