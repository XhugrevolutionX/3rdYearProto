using System;

public class EnemyHealth : Health
{
    public event Action OnDeath;

    protected override void Death()
    {
        OnDeath?.Invoke();
        Destroy(gameObject);
    }
}