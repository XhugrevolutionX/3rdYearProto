using UnityEngine;

public class MiniBossPortal : MonoBehaviour, IInteractable
{
    [Header("References")] 
    public EnemyController MiniBossController;
    
    
    public void Interact()
    {
        if (!MiniBossController) return;
        
        Instantiate(MiniBossController, transform.position, Quaternion.Euler(0, 180f, 0));
        Destroy(gameObject);
    }

    public void Enter()
    {
        Debug.Log("Enter");
    }

    public void Exit()
    {
        Debug.Log("Exit");
    }
}
