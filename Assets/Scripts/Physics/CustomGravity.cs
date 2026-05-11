using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CustomGravity : MonoBehaviour
{
    [Tooltip("Set the gravity scale for this object")]
    [SerializeField] private float gravityScale = 1.0f;

    private Rigidbody _rb;

    private float _globalGravity;

    private void OnEnable ()
    {
        _rb = GetComponent<Rigidbody>();
        _rb.useGravity = false;
        
        _globalGravity = Physics.gravity.y;
    }

    private void FixedUpdate ()
    {
        Vector3 gravity = _globalGravity * gravityScale * Vector3.up;
        _rb.AddForce(gravity, ForceMode.Acceleration);
    }
}