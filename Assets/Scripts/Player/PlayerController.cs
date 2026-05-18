using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started && canAttack) Attack();
    }
    
    protected override void Attack()
    {
        Debug.Log("Player attack");
        StartCoroutine(AttackRoutine());
    }
}
