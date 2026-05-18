using UnityEngine;

public class Display : MonoBehaviour
{
    protected Animator animator;

    private void Start() => GetComponentInChildren<Animator>();
}
