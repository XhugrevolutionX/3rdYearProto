using System.Collections;
using UnityEngine;

public enum TypeColor
{
    WHITE,
    RED,
    GREEN,
    BLUE,
    YELLOW,
    ORANGE,
    PURPLE
}

public abstract class Controller : MonoBehaviour
{
    [Header("Parameters")] 
    [SerializeField] private TypeColor type = TypeColor.WHITE;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;
    
    public TypeColor Type => type;
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
