public class AttackState : AIState
{
    public AttackState(EnemyController ai, AIStateMachine stateMachine) : base(ai, stateMachine) { }

    public override void Enter()
    {
        _ai.Movement.StopMoving();
        _ai.PerformAttack();
    }

    public override void Update()
    {
        if (!_ai.CanAttack) return;

        _stateMachine.ChangeState(_ai.PlayerTransform
            ? new ChaseState(_ai, _stateMachine)
            : new IdleState(_ai, _stateMachine));
    }
}
