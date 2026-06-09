using UnityEngine;

public class Projectile : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ParticleSystem impactParticles;
    
    [Header("Parameters")]
    [SerializeField] private KnockBackData knockBackData;
    [SerializeField] private CameraShakeData cameraShakeData;
    [SerializeField] private HitStopData hitStopData;
    [SerializeField] private float lifeTime = 5f;
    
    public Rigidbody Rb { get; private set; }
    
    private Controller _controller;
    private Movement _movement;

    private void Awake()
    {
        Rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

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

        if (hit && _movement)
        {

            CameraShakeManager.Instance?.Shake(cameraShakeData);
            TimeManager.Instance?.DoHitStop(hitStopData);
        }
        
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
