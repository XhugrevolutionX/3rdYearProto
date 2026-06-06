using TMPro;
using UnityEngine;

public class MiniBossPortal : MonoBehaviour, IInteractable
{
    [Header("References")] 
    [SerializeField] private TextMeshProUGUI interactTmp;
    public EnemyController MiniBossController;
    
    
    public void Interact()
    {
        if (!MiniBossController) return;
        
        Instantiate(MiniBossController, transform.position, Quaternion.Euler(0, 180f, 0));
        Destroy(gameObject);
    }

    public void Enter()
    {
        interactTmp.gameObject.SetActive(true);
    }

    public void Exit()
    {
        interactTmp.gameObject.SetActive(false);
    }
}
