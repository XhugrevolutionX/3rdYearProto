using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform viewTransform;
    
    [Header("Parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;

    protected Rigidbody _rb;
    protected Vector2 _direction;

    private void Update() => RotateView();

    private void FixedUpdate() => Move();
    
    private void Move() => _rb.linearVelocity = new Vector3(_direction.x * speed, _rb.linearVelocity.y, _direction.y * speed);
    
    
    private void RotateView()
    {
        if (_direction.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = new Vector3(_direction.x, 0f, _direction.y);
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        viewTransform.rotation = Quaternion.Slerp(viewTransform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
