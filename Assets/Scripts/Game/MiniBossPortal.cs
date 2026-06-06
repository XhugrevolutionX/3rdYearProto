using UnityEngine;

public class MiniBossPortal : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        Debug.Log("Interact");
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
