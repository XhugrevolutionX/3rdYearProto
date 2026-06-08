using UnityEngine;

public class RangedEnemyController : EnemyController
{
    [Header("Ranged")]
    [SerializeField] private Projectile projectilePrefab;
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private float projectileSpeed = 15f;
    [SerializeField] private float minDistance = 5f;
    [Range(0f, 1f)]
    [SerializeField] private float facingThreshold = 0.9f;

    protected override void Attack()
    {
        base.Attack();

        if (!PlayerTransform) return;

        Vector3 dir = (PlayerTransform.position - projectileSpawnPoint.position).normalized;
        var proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(dir));
        proj.InitProjectile(this, Movement);
        proj.Rb.linearVelocity = dir * projectileSpeed;
    }

    protected override void InitStateMachine() =>
        _stateMachine.Init(new KeepDistanceState(this, _stateMachine, minDistance, facingThreshold));

    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, minDistance);
    }
}
