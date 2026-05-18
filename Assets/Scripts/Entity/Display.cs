using UnityEngine;

public class Display : MonoBehaviour
{
    protected Animator animator;

    private void Start() => animator = GetComponentInChildren<Animator>();

    public void Attack() => animator.SetTrigger("Attack");
}
