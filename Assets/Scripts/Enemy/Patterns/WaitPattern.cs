using UnityEngine;

public class WaitPattern : BossPattern
{
    private readonly float _duration;
    private float _timer;

    public WaitPattern(float duration) { _duration = duration; }

    public override void Begin(BossController boss)
    {
        base.End(boss);
        _timer = _duration;
        boss.Movement.StopMoving();
    }

    public override void Tick(BossController boss)
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f) IsComplete = true;
    }
}
