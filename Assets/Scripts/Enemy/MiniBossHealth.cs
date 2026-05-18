using System;

public class MiniBossHealth : EnemyHealth
{
    public static event Action<TypeColor> OnDeath;

    private Controller _controller;

    protected override void Start()
    {
        base.Start();
        _controller = GetComponent<Controller>();
    }

    protected override void Death()
    {
        OnDeath?.Invoke(_controller.Type);
        base.Death();
    }
}
