using System;

public class MiniBossHealth : EnemyHealth
{
    public static event Action<TypeColor> OnMiniBossDeath;

    private Controller _controller;

    protected override void Awake()
    {
        base.Awake();
        _controller = GetComponent<Controller>();
    }

    protected override void Death()
    {
        OnMiniBossDeath?.Invoke(_controller.Type);
        base.Death();
    }
}
