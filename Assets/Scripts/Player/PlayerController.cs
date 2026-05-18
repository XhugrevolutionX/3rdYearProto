using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    private Display _display;

    private void Start()
    {
        _display = GetComponent<Display>();
    }
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started && canAttack) Attack();
    }
    
    protected override void Attack()
    {
        _display.Attack();
        StartCoroutine(AttackRoutine());
    }
}
