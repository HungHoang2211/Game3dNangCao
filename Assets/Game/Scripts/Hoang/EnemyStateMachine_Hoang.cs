using UnityEngine;

public class EnemyStateMachine_Hoang
{
    public EnemyState_Hoang currentState { get; private set; }
    public void Initialize(EnemyState_Hoang starState)
    {
        currentState = starState;
        currentState.Enter();
    }
    public void ChangeState(EnemyState_Hoang newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
