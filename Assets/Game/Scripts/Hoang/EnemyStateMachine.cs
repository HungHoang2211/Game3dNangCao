using UnityEngine;

public class EnemyStateMachine
{
    public EnemyState currentState { get; private set; }
    
    public void Initialize(EnemyState starState)
    {
        currentState = starState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
