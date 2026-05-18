using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;

    [Header("KnockBack")]
    [SerializeField] private float knockBackForce;
    [SerializeField] private float knockBackUpForce;
    [SerializeField] private float knockBackDuration;

    private void Awake()
    {
        if (!controller) controller = GetComponentInParent<Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = other.GetComponentInParent<Health>();
        if (!health) return;

        health.ChangeHealth(-controller.AttackDamage);

        Movement movement = other.GetComponentInParent<Movement>();
        if (movement)
        {
            Vector3 direction = (other.transform.position - controller.transform.position).normalized;
            direction.y = knockBackUpForce;
            movement.KnockBack(direction, knockBackForce, knockBackDuration);
        }
    }
}
