using UnityEngine;

public struct KnockBackData
{
    public float Force;
    public float UpForce;
    public float Duration;
}

public static class CombatUtils
{
    public static void ApplyHit(GameObject target, Controller attacker, int damage, KnockBackData knockBack)
    {
        Health health = target.GetComponentInParent<Health>();
        if (!health) return;

        Controller targetController = target.GetComponentInParent<Controller>();
        if (attacker.HasTypeDamageBonus && targetController != null && targetController.Type == attacker.Type) damage *= 2;

        health.ChangeHealth(-damage);

        Movement movement = target.GetComponentInParent<Movement>();
        if (!movement) return;

        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;
        direction.y = knockBack.UpForce;
        movement.KnockBack(direction, knockBack.Force, knockBack.Duration);
    }
}
