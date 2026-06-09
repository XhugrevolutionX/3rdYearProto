using UnityEngine;

public class ChasePattern : BossPattern
{
    private readonly float _duration;
    private float _timer;

    public ChasePattern(float duration) { _duration = duration; }

    public override void Begin(BossController boss)
    {
        base.End(boss);
        _timer = _duration;
    }

    public override void Tick(BossController boss)
    {
        if (boss.PlayerTransform)
        {
            float dist = Vector3.Distance(boss.transform.position, boss.PlayerTransform.position);
            if (dist <= boss.AttackRange) { IsComplete = true; return; }
            boss.Movement.SetTarget(boss.PlayerTransform.position);
        }

        _timer -= Time.deltaTime;
        if (_timer <= 0f) IsComplete = true;
    }

    public override void End(BossController boss)
    {
        boss.Movement.StopMoving();
        base.End(boss);
    }
}
