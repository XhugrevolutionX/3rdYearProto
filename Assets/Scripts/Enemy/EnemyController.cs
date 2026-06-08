using UnityEngine;

public class EnemyController : Controller
{
    [Header("Enemy Parameters")]
    [SerializeField] private int contactDamage = 1;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce = 10f;
    [SerializeField] private float knockBackDuration = 0.5f;
    
    [Header("Detection")]
    [SerializeField] private float detectionRadius = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackOrientationLockDuration = 0.3f;
    [SerializeField] private LayerMask playerMask;

    public EnemyMovement Movement { get; private set; }
    public Transform PlayerTransform { get; private set; }
    public float AttackRange => attackRange;
    public float AttackOrientationLockDuration => attackOrientationLockDuration;
    public bool CanAttack => canAttack;

    public void PerformAttack()
    {
        Attack();
        StartCoroutine(AttackRoutine());
    }

    private Display _display;
    protected AIStateMachine _stateMachine;

    protected virtual void Start()
    {
        Movement = GetComponent<EnemyMovement>();
        _display = GetComponent<Display>();

        _stateMachine = new AIStateMachine();
        InitStateMachine();
    }

    protected virtual void InitStateMachine() => _stateMachine.Init(new IdleState(this, _stateMachine));

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

    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(Movement.IsKnockedBack) return;
        
        CombatUtils.ApplyHit<PlayerHealth>(collision.gameObject,
            this, 
            contactDamage,
            new KnockBackData { Force = knockBackForce, Duration = knockBackDuration }
        );
    }

    protected override void Attack()
    {
        _display.Attack();
    }
}
