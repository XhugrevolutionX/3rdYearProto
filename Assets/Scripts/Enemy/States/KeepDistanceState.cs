using UnityEngine;

public class KeepDistanceState : AIState
{
    private readonly float _minDistance;
    private readonly float _facingThreshold;

    public KeepDistanceState(EnemyController ai, AIStateMachine sm, float minDistance, float facingThreshold = 0f) : base(ai, sm)
    {
        _minDistance = minDistance;
        _facingThreshold = facingThreshold;
    }

    public override void Update()
    {
        if (!_ai.PlayerTransform)
        {
            _ai.Movement.StopMoving();
            return;
        }

        float dist = Vector3.Distance(_ai.transform.position, _ai.PlayerTransform.position);

        Vector3 toPlayer = (_ai.PlayerTransform.position - _ai.transform.position).normalized;
        bool isFacing = _facingThreshold <= 0f || Vector3.Dot(_ai.Movement.Forward, toPlayer) >= _facingThreshold;

        if (isFacing && dist <= _ai.AttackRange && _ai.CanAttack && !_ai.Movement.IsKnockedBack)
        {
            _ai.PerformAttack();
            return;
        }

        if (dist < _minDistance)
        {
            Vector3 awayDir = (_ai.transform.position - _ai.PlayerTransform.position).normalized;
            _ai.Movement.SetTarget(_ai.transform.position + awayDir * _minDistance);
        }
        else if (dist > _ai.AttackRange)
            _ai.Movement.SetTarget(_ai.PlayerTransform.position);
        else
        {
            _ai.Movement.StopMoving();
            _ai.Movement.RotateToward(_ai.PlayerTransform.position);
        }
    }
}
