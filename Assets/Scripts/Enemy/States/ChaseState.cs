using UnityEngine;

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

        float dist = Vector3.Distance(_ai.transform.position, _ai.PlayerTransform.position);
        if (dist <= _ai.AttackRange && _ai.CanAttack && !_ai.Movement.IsKnockedBack)
        {
            _stateMachine.ChangeState(new AttackState(_ai, _stateMachine));
            return;
        }

        _ai.Movement.SetTarget(_ai.PlayerTransform.position);
    }
}
