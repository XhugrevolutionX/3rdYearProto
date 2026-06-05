using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockBackDuration;
    
    [Header("Shake")]
    [SerializeField] private CameraShakeData cameraShakeData;

    [Header("Attack KnockBack (self)")]
    [SerializeField] private KnockBackData attackKnockBackData;

    private Movement _movement;

    private void Awake()
    {
        if (!controller) controller = GetComponentInParent<Controller>();
        _movement = controller.GetComponent<Movement>();
    }

    private void OnTriggerEnter(Collider other)
    {
        bool hit = CombatUtils.ApplyHit<Health>(other.gameObject,
            controller,
            controller.AttackDamage,
            new KnockBackData { Force = knockBackForce, Duration = knockBackDuration }
        );

        if (!hit || _movement == null) return;
        
        Vector3 recoilDir = (controller.transform.position - other.transform.position).normalized;
        _movement.KnockBack(recoilDir, attackKnockBackData.Force, attackKnockBackData.Duration);
        
        CameraShakeManager.Instance?.Shake(cameraShakeData);
    }
}
