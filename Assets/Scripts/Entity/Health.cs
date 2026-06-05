using System.Collections;
using UnityEngine;

/**
 * @jemoelablay
 * EntityHealth.sc
 * 09.09.2024
 * Description : This script simulate the health of an entity, it can be
 * a player, an enemy, or even an object. To deal damage or heal the entity
 * simply call the ChangeHealth() function with the desired amount.
 */

public class Health : MonoBehaviour
{
    [Header("Hit Flash")] 
    [SerializeField] private Renderer characterRenderer;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float hitFlashDuration = 0.1f;
    
    [Header("Health Configuration")]
    [Tooltip("The maximum that the player can have")]
    [Min(1)] private int MaxHealth = 100;
    
    private Material _originalMaterial;

    int _currentHealth;

    /// <summary>
    /// Gets or sets the current health of the entity.
    /// Health is clamped between 0 and MaxHealth.
    /// </summary>
    private int CurrentHealth 
    { 
        get => _currentHealth;
        set
        {
            if (value > MaxHealth) _currentHealth = MaxHealth;
            else if (value <= 0) { _currentHealth = 0; Death(); }
            else _currentHealth = value;
        }
    }

    protected virtual void Start()
    {
        // Initialize the entity with the maximum health
        CurrentHealth = MaxHealth;
        
        _originalMaterial = characterRenderer.material;
    }
    
    /// <summary>
    /// Handles the entity taking damage. Override for specific behavior.
    /// </summary>
    protected virtual void TakeDamage(int amount)
    {
        StartCoroutine(HitFlash());
        Debug.Log($"Entity took {Mathf.Abs(amount)} damage");
    }
    
    /// <summary>
    /// This function allows to change the entity's health by a specific amount.
    /// </summary>
    /// <param name="amount">The amount to add to the entity's health. A negative value will deal damage.</param>
    public virtual void ChangeHealth(int amount)
    {
        CurrentHealth += amount;
        if (amount < 0) TakeDamage(amount);
    }

    /// <summary>
    /// Handles the entity's death. Override to customize death behavior.
    /// </summary>
    protected virtual void Death()
    {
        Debug.Log("Entity is dead");
    }

    private IEnumerator HitFlash()
    {
        characterRenderer.material = hitMaterial;
        yield return new WaitForSeconds(hitFlashDuration);
        characterRenderer.material = _originalMaterial;
    }
}