using UnityEngine;

public class FinalBoss : BossController
{
    [Header("Tornado Attack")]
    [SerializeField] private Rotate tornadoRotate;
    [SerializeField] private Collider tornadoHitbox;
    [SerializeField] private float tornadoWindUp = 1.5f;
    [SerializeField] private float tornadoActiveDuration = 4f;
    [SerializeField] private float tornadoWindDown = 1f;
    [SerializeField] private float tornadoMaxSpeed = 360f;
    [SerializeField] private float tornadoMoveSpeed = 8f;

    protected override void InitStateMachine()
    {
        _stateMachine.Init(new BossPatternState(this, _stateMachine, new BossPattern[]
        {
            new ChasePattern(5f),
            new TornadoPattern(tornadoRotate, tornadoHitbox, tornadoWindUp, tornadoActiveDuration, tornadoWindDown, tornadoMaxSpeed, tornadoMoveSpeed),
            new WaitPattern(2f),
        }));
    }

    protected override BossPhase[] CreatePhases() => new[]
    {
        new BossPhase(0.5f, new BossPattern[]
        {
            new ChasePattern(5f),
            new WaitPattern(1f),
        })
    };
}
