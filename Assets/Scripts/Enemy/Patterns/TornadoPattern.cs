using UnityEngine;

public class TornadoPattern : BossPattern
{
    private enum Phase { WindUp, Active, WindDown, Recover }

    private readonly Rotate _rotate;
    private readonly Collider _hitbox;
    private readonly ParticleSystem _particles;
    private readonly float _windUpDuration;
    private readonly float _activeDuration;
    private readonly float _windDownDuration;
    private readonly float _maxSpeed;
    private readonly float _moveSpeed;

    private Phase _phase;
    private float _timer;
    private float _originalMoveSpeed;
    private Quaternion _originalRotation;

    public TornadoPattern(Rotate rotate, Collider hitbox, ParticleSystem particles, float windUp, float active, float windDown, float maxSpeed, float moveSpeed)
    {
        _rotate = rotate;
        _hitbox = hitbox;
        _particles = particles;
        _windUpDuration = windUp;
        _activeDuration = active;
        _windDownDuration = windDown;
        _maxSpeed = maxSpeed;
        _moveSpeed = moveSpeed;
    }

    public override void Begin(BossController boss)
    {
        base.End(boss);
        _originalMoveSpeed = boss.Movement.Speed;
        _originalRotation = _rotate.transform.localRotation;
        boss.Movement.Speed = _moveSpeed;
        _phase = Phase.WindUp;
        _timer = _windUpDuration;
        _rotate.enabled = true;
        _rotate.Speed = Vector3.zero;
        _hitbox.enabled = false;
    }

    public override void Tick(BossController boss)
    {
        _timer -= Time.deltaTime;

        switch (_phase)
        {
            case Phase.WindUp:
                float windUpT = 1f - Mathf.Clamp01(_timer / _windUpDuration);
                _rotate.Speed = Vector3.forward * Mathf.Lerp(0f, _maxSpeed, windUpT);
                if (_timer <= 0f) EnterActive(boss);
                break;

            case Phase.Active:
                if (boss.PlayerTransform)
                    boss.Movement.SetTarget(boss.PlayerTransform.position);
                if (_timer <= 0f) EnterWindDown(boss);
                break;

            case Phase.WindDown:
                float windDownT = 1f - Mathf.Clamp01(_timer / _windDownDuration);
                _rotate.Speed = Vector3.forward * Mathf.Lerp(_maxSpeed, 0f, windDownT);
                if (_timer <= 0f) EnterRecover();
                break;

            case Phase.Recover:
                _rotate.transform.localRotation = Quaternion.RotateTowards(
                    _rotate.transform.localRotation,
                    _originalRotation,
                    180f * Time.deltaTime
                );
                if (Quaternion.Angle(_rotate.transform.localRotation, _originalRotation) < 0.5f)
                {
                    _rotate.transform.localRotation = _originalRotation;
                    IsComplete = true;
                }
                break;
        }
    }

    public override void End(BossController boss)
    {
        _rotate.enabled = false;
        _rotate.Speed = Vector3.zero;
        _hitbox.enabled = false;
        _particles?.Stop();
        boss.Movement.Speed = _originalMoveSpeed;
        boss.Movement.StopMoving();
        base.End(boss);
    }

    private void EnterActive(BossController boss)
    {
        _phase = Phase.Active;
        _timer = _activeDuration;
        _rotate.Speed = Vector3.forward * _maxSpeed;
        _hitbox.enabled = true;
        _particles?.Play();
    }

    private void EnterWindDown(BossController boss)
    {
        _phase = Phase.WindDown;
        _timer = _windDownDuration;
        _hitbox.enabled = false;
        _particles?.Stop();
    }

    private void EnterRecover()
    {
        _phase = Phase.Recover;
        _rotate.enabled = false;
        _rotate.Speed = Vector3.zero;
    }
}
