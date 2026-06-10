public class BossPatternState : BossAIState
{
    private readonly BossPattern[] _patterns;
    private int _index;
    private BossPattern Current => _patterns[_index];

    public BossPatternState(BossController boss, AIStateMachine sm, BossPattern[] patterns)
        : base(boss, sm)
    {
        _patterns = patterns;
    }

    public override void Enter()
    {
        _index = 0;
        Current.Begin(_boss);
    }

    public override void Update()
    {
        if (!_boss.IsActivated) return;
        Current.Tick(_boss);

        if (!Current.IsComplete) return;

        Current.End(_boss);
        _index = (_index + 1) % _patterns.Length;
        Current.Begin(_boss);
    }

    public override void Exit() => Current.End(_boss);
}
