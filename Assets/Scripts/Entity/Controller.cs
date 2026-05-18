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
    [SerializeField] protected TypeColor type = TypeColor.WHITE;
    [SerializeField] private bool typeDamageBonus;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.5f;

    public virtual TypeColor Type => type;
    public bool HasTypeDamageBonus => typeDamageBonus;
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
