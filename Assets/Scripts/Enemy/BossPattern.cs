public abstract class BossPattern
{
    public bool IsComplete { get; protected set; }

    public abstract void Begin(BossController boss);
    public virtual void Tick(BossController boss) { }
    public virtual void End(BossController boss) { IsComplete = false; }
}
