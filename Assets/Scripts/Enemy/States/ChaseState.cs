public class ChaseState : AIState
{
    public ChaseState(EnemyController ai, AIStateMachine stateMachine) : base(ai, stateMachine) { }

    public override void Update()
    {
        if (!_ai.PlayerTransform)
        {
            _stateMachine.ChangeState(new IdleState(_ai, _stateMachine));
            return;
        }

        _ai.Movement.SetTarget(_ai.PlayerTransform.position);
    }
}
