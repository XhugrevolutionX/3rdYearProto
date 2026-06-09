public class AttackPattern : BossPattern
{
    private readonly string _triggerName;

    public AttackPattern(string triggerName) { _triggerName = triggerName; }

    public override void Begin(BossController boss)
    {
        base.End(boss);
        boss.PlayAttack(_triggerName);
    }

    public override void Tick(BossController boss)
    {
        if (boss.AttackAnimationDone) IsComplete = true;
    }
}
