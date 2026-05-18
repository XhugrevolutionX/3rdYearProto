using UnityEngine;

public static class CombatUtils
{
    public static void ApplyHit(GameObject target, Transform source, int damage, float knockBackForce, float knockBackUpForce, float knockBackDuration)
    {
        Health health = target.GetComponentInParent<Health>();
        if (!health) return;

        health.ChangeHealth(-damage);

        Movement movement = target.GetComponentInParent<Movement>();
        if (!movement) return;

        Vector3 direction = (target.transform.position - source.position).normalized;
        direction.y = knockBackUpForce;
        movement.KnockBack(direction, knockBackForce, knockBackDuration);
    }
}
