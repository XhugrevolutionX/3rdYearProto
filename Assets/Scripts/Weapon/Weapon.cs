using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockBackUpForce;
    [SerializeField] private float knockBackDuration;

    private void Awake()
    {
        if (!controller) controller = GetComponentInParent<Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        CombatUtils.ApplyHit<Health>(other.gameObject,
            controller, 
            controller.AttackDamage,
            new KnockBackData { Force = knockBackForce, UpForce = knockBackUpForce, Duration = knockBackDuration }
        );
    }
}
