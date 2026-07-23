using UnityEngine;

public interface IState
{
    void Enter();
    void Tick();
    void Exit();
}

public class StateMachine
{
    public IState Current { get; private set; }
    public float TimeInState => Time.time - enteredAt;
    public string CurrentName => Current != null ? Current.GetType().Name : "None";

    float enteredAt;

    public void ChangeState(IState next)
    {
        if (next == Current) return;

        Current?.Exit();
        Current = next;
        enteredAt = Time.time;
        Current?.Enter();
    }

    public void Tick()
    {
        Current?.Tick();
    }
}
