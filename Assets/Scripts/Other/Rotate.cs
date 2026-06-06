using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Vector3 rotationSpeed;

    private void Update() => transform.Rotate(rotationSpeed * Time.deltaTime);
}