using System;
using UnityEngine;

[Serializable]
public struct KnockBackData
{
    public float Force;
    public float Duration;
}

public static class CombatUtils
{
    public static bool ApplyHit<T>(GameObject target, Controller attacker, int damage, KnockBackData knockBack) where T : Health
    {
        T health = target.GetComponentInParent<T>();
        if (!health) return false;

        Controller targetController = target.GetComponentInParent<Controller>();
        if (attacker.HasTypeDamageBonus && targetController != null && targetController.Type == attacker.Type) damage *= 2;
        
        // Debug.Log($"Attack type : {attacker.Type} / Target type : {targetController.Type}");

        health.ChangeHealth(-damage);

        Movement movement = target.GetComponentInParent<Movement>();
        if (!movement) return false;

        Vector3 direction = (target.transform.position - attacker.transform.position).normalized;
        movement.KnockBack(direction, knockBack.Force, knockBack.Duration);
        return true;
    }
}
