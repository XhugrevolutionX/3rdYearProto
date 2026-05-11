public class AIState
{
    protected readonly EnemyController _ai;
    protected readonly AIStateMachine _stateMachine;

    public AIState(EnemyController ai, AIStateMachine stateMachine)
    {
        _ai = ai;
        _stateMachine = stateMachine;
    }
    
    public virtual void Enter(){}
    public virtual void Exit(){}
    public virtual void Update(){}
    public virtual void FixedUpdate(){}
}
