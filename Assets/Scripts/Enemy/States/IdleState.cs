public class IdleState : AIState
{
    public IdleState(EnemyController ai, AIStateMachine stateMachine) : base(ai, stateMachine) { }

    public override void Enter()=> _ai.Movement.StopMoving();
    

    public override void Update()
    {
        if (_ai.PlayerTransform)
            _stateMachine.ChangeState(new ChaseState(_ai, _stateMachine));
    }
}
