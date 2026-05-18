public class EnemyHealth : Health
{
    protected override void Death()
    {
        Destroy(gameObject);
    }
}