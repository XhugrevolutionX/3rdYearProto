using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Controller controller;

    private void Awake()
    {
        if (!controller) controller = GetComponentInParent<Controller>();
    }

    private void OnTriggerEnter(Collider other)
    {
        Health health = GetHealthComponent(other.gameObject);
        
        if(health) health.ChangeHealth(-controller.AttackDamage);
    }
    
    private Health GetHealthComponent(GameObject target)
    {
        if(target.TryGetComponent(out Health health))
            return health;
        if (target.transform.parent)
            return GetHealthComponent(target.transform.parent.gameObject);
        return null;
    }
}
