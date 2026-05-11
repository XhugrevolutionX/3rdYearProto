using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform viewTransform;
    
    [Header("Parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float rotationSpeed = 10f;
    
    [Header("Ground Check")]
    [Tooltip("The mask on what the player is grounded")]
    [SerializeField] private LayerMask groundMask;
    
    [Tooltip("The radius of the sphere cast")]
    [SerializeField] private float sphereRadius = 0.5f;

    [Tooltip("The maximum distance for the sphere cast")]
    [SerializeField] private float sphereMaxDistance;

    private Rigidbody _rb;

    private Vector2 _direction;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        _rb.linearVelocity = new Vector3(_direction.x * speed, _rb.linearVelocity.y, _direction.y * speed);
        RotateView();
    }

    private void RotateView()
    {
        if (_direction.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = new Vector3(_direction.x, 0f, _direction.y);
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        viewTransform.rotation = Quaternion.Slerp(viewTransform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
    
    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    private bool CheckCollisions()
    {
        return Physics.SphereCast(
            transform.position,
            sphereRadius,
            Vector3.down,
            out RaycastHit _,
            sphereMaxDistance,
            groundMask
        );
    }

    private void OnDrawGizmosSelected()
    {
        bool grounded = CheckCollisions();
        Gizmos.color = grounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(transform.position + Vector3.down * sphereMaxDistance, sphereRadius);
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context) => _direction = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started && CheckCollisions()) Jump();
    }

    #endregion
}
