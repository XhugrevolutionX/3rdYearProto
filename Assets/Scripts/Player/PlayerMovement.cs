using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
    
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
    }
    
    private void Jump()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Force);
    }

    #region Input

    public void OnMove(InputAction.CallbackContext context) => _direction = context.ReadValue<Vector2>();

    public void OnJump(InputAction.CallbackContext context)
    {
        if(context.started) Jump();
    }

    #endregion
}
