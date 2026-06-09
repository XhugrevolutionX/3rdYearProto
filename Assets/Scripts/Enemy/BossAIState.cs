public class BossAIState : AIState
{
    protected readonly BossController _boss;

    public BossAIState(BossController boss, AIStateMachine sm) : base(boss, sm)
    {
        _boss = boss;
    }
}
