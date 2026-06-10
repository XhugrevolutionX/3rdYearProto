using System.Collections;
using UnityEngine;

public class FinalBoss : BossController
{
    [Header("Tornado Attack")]
    [SerializeField] private Rotate tornadoRotate;
    [SerializeField] private Collider tornadoHitbox;
    [SerializeField] private ParticleSystem tornadoParticles;
    [SerializeField] private float tornadoWindUp = 1.5f;
    [SerializeField] private float tornadoActiveDuration = 4f;
    [SerializeField] private float tornadoWindDown = 1f;
    [SerializeField] private float tornadoMaxSpeed = 360f;
    [SerializeField] private float tornadoMoveSpeed = 8f;
    
    [Header("Meteorite Strike")]
    [SerializeField] private MeteoriteStrike meteoriteStrikePrefab;
    [SerializeField] private float meteoriteDuration = 10f;
    [SerializeField] private float meteoriteInterval = 1.5f;
    [SerializeField] private float meteoriteMinSpawnRadius = 3f;
    [SerializeField] private float meteoriteMaxSpawnRadius = 8f;
    
    [Header("Camera Shake")]
    [SerializeField] private CameraShakeData cameraShakeData;
    

    protected override void InitStateMachine()
    {
        _stateMachine.Init(new BossPatternState(this, _stateMachine, new BossPattern[]
        {
            new WaitPattern(2f),
            new ChasePattern(5f),
            new TornadoPattern(tornadoRotate, tornadoHitbox, tornadoParticles, tornadoWindUp, tornadoActiveDuration, tornadoWindDown, tornadoMaxSpeed, tornadoMoveSpeed),
            new WaitPattern(2f),
            new AttackPattern("Meteorite")
        }));
    }

    protected override BossPhase[] CreatePhases() => new[]
    {
        new BossPhase(0.5f, new BossPattern[]
        {
            new WaitPattern(1f),
            new ChasePattern(2f),
            new TornadoPattern(tornadoRotate, tornadoHitbox, tornadoParticles, tornadoWindUp, tornadoActiveDuration, tornadoWindDown, tornadoMaxSpeed, tornadoMoveSpeed),
            new WaitPattern(1f),
            new AttackPattern("Meteorite"),
            new AttackPattern("Meteorite"),
            new TornadoPattern(tornadoRotate, tornadoHitbox, tornadoParticles, tornadoWindUp, tornadoActiveDuration, tornadoWindDown, tornadoMaxSpeed, tornadoMoveSpeed),
            new AttackPattern("Meteorite")
        })
    };
    
    protected override void DetectPlayer()
    {
        if (IsActivated) return;
        if (Vector3.Distance(transform.position, PlayerTransform.position) <= detectionRadius)
            IsActivated = true;
    }

    public void Shake() => CameraShakeManager.Instance.Shake(cameraShakeData);

    public void PlayMeteoriteStrikes() => StartCoroutine(MeteoriteStrikesRoutine());

    private IEnumerator MeteoriteStrikesRoutine()
    {
        float timer = meteoriteDuration;
        while (timer > 0f)
        {
            SpawnMeteoriteStrike();
            yield return new WaitForSeconds(meteoriteInterval);
            timer -= meteoriteInterval;
        }
    }

    private void SpawnMeteoriteStrike()
    {
        Vector2 offset = Random.insideUnitCircle.normalized * Random.Range(meteoriteMinSpawnRadius, meteoriteMaxSpawnRadius);
        Vector3 position = transform.position + new Vector3(offset.x, 0f, offset.y);
        TypeColor[] types = { TypeColor.RED, TypeColor.GREEN, TypeColor.BLUE, TypeColor.YELLOW, TypeColor.ORANGE, TypeColor.PURPLE };
        TypeColor type = types[Random.Range(0, types.Length)];
        var strike = Instantiate(meteoriteStrikePrefab, position, Quaternion.identity);
        strike.Init(position, this, type);
    }

    public void KnockBackPlayer()
    {
        if (!PlayerTransform) return;
        if (Vector3.Distance(transform.position, PlayerTransform.position) > detectionRadius) return;
        Vector3 direction = (PlayerTransform.position - transform.position).normalized;
        PlayerController.playerController.Movement.KnockBack(direction, knockBackForce, knockBackDuration);
    }
}
