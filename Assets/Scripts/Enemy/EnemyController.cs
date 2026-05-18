using UnityEngine;

public class EnemyController : Controller
{
    [Header("Enemy Parameters")]
    [SerializeField] private int contactDamage = 1;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private float knockBackUpForce = 1f;
    [SerializeField] private float knockBackDuration = 0.5f;
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private LayerMask playerMask;

    public EnemyMovement Movement { get; private set; }
    public Transform PlayerTransform { get; private set; }

    private AIStateMachine _stateMachine;

    private void Start()
    {
        Movement = GetComponent<EnemyMovement>();
        
        _stateMachine = new AIStateMachine();
        _stateMachine.Init(new IdleState(this, _stateMachine));
    }

    private void Update()
    {
        _stateMachine.Update();
    }

    private void FixedUpdate()
    {
        _stateMachine.FixedUpdate();
        DetectPlayer();
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, playerMask);
        PlayerTransform = hits.Length > 0 ? hits[0].transform : null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        CombatUtils.ApplyHit(collision.gameObject, transform, contactDamage, knockBackForce, knockBackUpForce, knockBackDuration);
    }

    protected override void Attack()
    {
        Debug.Log("Enemy Attacking");
    }
}
