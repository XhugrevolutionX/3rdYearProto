using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem impactParticles;
    
    [Header("Parameters")]
    [SerializeField] private KnockBackData knockBackData;
    [SerializeField] private CameraShakeData cameraShakeData;
    [SerializeField] private HitStopData hitStopData;
    
    public Rigidbody Rb { get; private set; }
    
    private Controller _controller;
    private Movement _movement;
    
    private void Awake() => Rb = GetComponent<Rigidbody>();

    public void InitProjectile(Controller controller, Movement movement)
    {
        _controller = controller;
        _movement = movement;
    }

    private void OnCollisionEnter(Collision other)
    {
        bool hit = CombatUtils.ApplyHit<Health>(other.gameObject,
            _controller,
            _controller.AttackDamage,
            knockBackData
        );

        if (!hit || _movement == null) return;
        
        impactParticles.transform.position = other.transform.position;
        impactParticles.Play();
        CameraShakeManager.Instance?.Shake(cameraShakeData);
        TimeManager.Instance?.DoHitStop(hitStopData);
    }
}
