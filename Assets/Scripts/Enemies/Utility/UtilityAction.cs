public abstract class UtilityAction
{
    public float LastScore { get; private set; }

    public abstract float Score();
    public abstract void Execute();

    public virtual void OnEnter() { }
    public virtual void OnExit() { }

    public virtual string Name => GetType().Name;

    public float Evaluate()
    {
        LastScore = Score();
        return LastScore;
    }
}
