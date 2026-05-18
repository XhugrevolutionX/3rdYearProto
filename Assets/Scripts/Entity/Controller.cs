using System.Collections;
using UnityEngine;

public abstract class Controller : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    
    public int AttackDamage => attackDamage;

    protected bool canAttack = true;

    protected abstract void Attack();

    protected IEnumerator AttackRoutine()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }
}
