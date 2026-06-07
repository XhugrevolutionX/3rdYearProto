using UnityEngine;

public class XP : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField][Min(1)] private int amount = 1;
    [SerializeField] private float magnetForce = 20f;

    public Rigidbody Rb { get; private set; }

    private Transform _target;

    private void Awake() => Rb = GetComponent<Rigidbody>();

    private void FixedUpdate()
    {
        if (!_target) return;

        Vector3 dir = (_target.position - transform.position).normalized;
        Rb.AddForce(dir * magnetForce, ForceMode.Force);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            _target = other.transform;
        else if(other.CompareTag("PlayerCurrency"))
        {
            PlayerLevelSystem.Instance.GainXP(amount);
            Destroy(gameObject);
        }
    }
}
