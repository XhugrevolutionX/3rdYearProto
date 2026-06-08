using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;
    [SerializeField] private ParticleSystem impactParticles;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockBackDuration;
    
    [Header("Shake")]
    [SerializeField] private CameraShakeData cameraShakeData;

    [Header("Hit Stop")]
    [SerializeField] private HitStopData hitStopData;

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
        
        // Vector3 recoilDir = (controller.transform.position - other.transform.position).normalized;
        // _movement.KnockBack(recoilDir, attackKnockBackData.Force, attackKnockBackData.Duration);
        
        impactParticles.transform.position = other.transform.position;
        impactParticles.Play();
        CameraShakeManager.Instance?.Shake(cameraShakeData);
        TimeManager.Instance?.DoHitStop(hitStopData);
    }
}
