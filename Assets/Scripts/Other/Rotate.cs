using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private Vector3 rotationSpeed;

    public Vector3 Speed { get => rotationSpeed; set => rotationSpeed = value; }

    private void Update() => transform.Rotate(rotationSpeed * Time.deltaTime);
}