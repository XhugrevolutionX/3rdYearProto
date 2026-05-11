public class AIStateMachine
{
    private AIState _currentState;

    public void Init(AIState enemyState)
    {
        _currentState = enemyState;
        _currentState.Enter();
    }

    public void ChangeState(AIState newState)
    {
        if(_currentState == newState) return;
        
        _currentState.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
    
    public void Update() => _currentState?.Update();
    
    public void FixedUpdate() => _currentState?.FixedUpdate();
}
