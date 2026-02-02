public class EnemyStateMachine_Phat
{
    public EnemyState_Phat currentState { get; private set; }

    public void Initialize(EnemyState_Phat startState)
    {
        currentState = startState;
        currentState.Enter();
    }

    public void ChangeState(EnemyState_Phat newState)
    {
        currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }
}
