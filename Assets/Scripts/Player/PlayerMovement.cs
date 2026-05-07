using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private float _speed = 5f;
    
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
        _rb.linearVelocity = new Vector3(_direction.x * _speed, _rb.linearVelocity.y, _direction.y * _speed);
    }

    public void OnMove(InputAction.CallbackContext context) => _direction = context.ReadValue<Vector2>();
}
