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

    private Camera _mainCamera;
    private Vector2 _lookInput;
    private bool _isGamepad;

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _mainCamera = Camera.main;
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context) => _direction = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started && CheckCollisions()) Jump();
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        _lookInput = context.ReadValue<Vector2>();
        _isGamepad = context.control.device is Gamepad;
    }

    #endregion

    private void Jump()
    {
        _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, 0, _rb.linearVelocity.z);
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    protected override void RotateView()
    {
        if (_isGamepad)
        {
            if (_lookInput.sqrMagnitude > 0.01f)
            {
                Vector3 direction = new Vector3(_lookInput.x, 0f, _lookInput.y);
                Quaternion target = Quaternion.LookRotation(direction);
                viewTransform.rotation = Quaternion.Slerp(viewTransform.rotation, target, rotationSpeed * Time.deltaTime);
            }
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        Plane groundPlane = new Plane(Vector3.up, viewTransform.position);

        if (groundPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPoint = ray.GetPoint(distance);
            Vector3 direction = worldPoint - viewTransform.position;
            direction.y = 0f;

            if (direction.sqrMagnitude > 0.01f)
            {
                Quaternion target = Quaternion.LookRotation(direction);
                viewTransform.rotation = Quaternion.Slerp(viewTransform.rotation, target, rotationSpeed * Time.deltaTime);
            }
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
}
