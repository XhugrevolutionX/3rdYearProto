using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement : Movement
{
    private NavMeshAgent _agent;

    protected override void Awake()
    {
        base.Awake();
        _agent = GetComponent<NavMeshAgent>();
        _agent.updatePosition = false;
        _agent.updateRotation = false;
    }

    public void SetTarget(Vector3 target) => _agent.SetDestination(target);

    public void StopMoving()
    {
        _direction = Vector2.zero;
        _agent.ResetPath();
    }

    protected override void FixedUpdate()
    {
        if (_agent.hasPath)
        {
            Vector3 dir = (_agent.steeringTarget - transform.position).normalized;
            _direction = new Vector2(dir.x, dir.z);
        }

        base.FixedUpdate();
        _agent.nextPosition = transform.position;
    }
}
