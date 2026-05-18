using UnityEngine;

public struct KnockBackData
{
    public float Force;
    public float UpForce;
    public float Duration;
}

public static class CombatUtils
{
    public static void ApplyHit<T>(GameObject target, Controller attacker, int damage, KnockBackData knockBack) where T : Health
    {
        T health = target.GetComponentInParent<T>();
        if (!health) return;

        Controller targetController = target.GetComponentInParent<Controller>();
        if (attacker.HasTypeDamageBonus && targetController != null && targetController.Type == attacker.Type) damage *= 2;
        
        Debug.Log($"Attack type : {attacker.Type} / Target type : {targetController.Type}");

        health.ChangeHealth(-damage);

        Movement movement = target.GetComponentInParent<Movement>();
        if (!movement) return;

        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;
        direction.y = knockBack.UpForce;
        movement.KnockBack(direction, knockBack.Force, knockBack.Duration);
    }
}
