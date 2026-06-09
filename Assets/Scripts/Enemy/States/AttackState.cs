using UnityEngine;

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

        if (!_ai.PlayerTransform) { _stateMachine.ChangeState(new IdleState(_ai, _stateMachine)); return; }

        float dist = Vector3.Distance(_ai.transform.position, _ai.PlayerTransform.position);
        _stateMachine.ChangeState(dist <= _ai.AttackRange
            ? new AttackState(_ai, _stateMachine)
            : new ChaseState(_ai, _stateMachine));
    }
}
