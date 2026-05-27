using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : Movement
{
    [Header("Jump")]
    [Tooltip("Amount of jump force")]
    [SerializeField] private float jumpForce = 10f;
    
    [Tooltip("The mask on what the player is grounded")]
    [SerializeField] private LayerMask groundMask;
    
    [Tooltip("The radius of the sphere cast")]
    [SerializeField] private float sphereRadius = 0.5f;

    [Tooltip("The maximum distance for the sphere cast")]
    [SerializeField] private float sphereMaxDistance;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
    
    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    protected override void RotateView()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, transform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 direction = worldPoint - transform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
                transform.rotation = Quaternion.LookRotation(direction);
        }
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
