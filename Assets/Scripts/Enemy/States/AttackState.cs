public class AttackState : AIState
{
    public AttackState(EnemyController ai, AIStateMachine stateMachine) : base(ai, stateMachine) { }

    public override void Enter()
    {
        _ai.Movement.StopMoving();
        _ai.Movement.LockRotation(_ai.AttackOrientationLockDuration);
        _ai.PerformAttack();
    }

    public override void Update()
    {
        if (!_ai.AttackIgnoresKnockback && _ai.Movement.IsKnockedBack)
        {
            _stateMachine.ChangeState(new ChaseState(_ai, _stateMachine));
            return;
        }

        if (!_ai.CanAttack)
        {
            if (!_ai.Movement.IsRotationLocked && _ai.PlayerTransform)
                _ai.Movement.RotateToward(_ai.PlayerTransform.position);
            return;
        }

        _stateMachine.ChangeState(_ai.PlayerTransform
            ? new ChaseState(_ai, _stateMachine)
            : new IdleState(_ai, _stateMachine));
    }
}
