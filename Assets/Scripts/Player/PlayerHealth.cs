using UnityEngine;

public class PlayerHealth : Health
{
    [Header("Camera Shake")]
    [SerializeField] private CameraShakeData hitShakeData;
    
    protected override void TakeDamage(int amount)
    {
        base.TakeDamage(amount);
        CameraShakeManager.Instance?.Shake(hitShakeData);
    }

    protected override void Death()
    {
        Debug.Log("Player is death");
    }
}