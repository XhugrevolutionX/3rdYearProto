using System.Collections;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform viewTransform;

    [Header("Parameters")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float rotationSpeed = 10f;
    [SerializeField] private float knockBackResistance = 5f;
    
    public bool IsKnockedBack => _isKnockedBack;

    protected Rigidbody _rb;
    protected Vector2 _direction;
    private bool _isKnockedBack;

    protected virtual void Awake() => _rb = GetComponent<Rigidbody>();

    protected virtual void Update() => RotateView();

    protected virtual void FixedUpdate()
    {
        if (_isKnockedBack)
        {
            Vector3 vel = _rb.linearVelocity;
            float damping = 1f - knockBackResistance * Time.fixedDeltaTime;
            vel.x *= damping;
            vel.z *= damping;
            _rb.linearVelocity = vel;
        }
        else Move();
    }

    private Coroutine _knockBackCoroutine;

    public void KnockBack(Vector3 direction, float force, float duration)
    {
        if (_knockBackCoroutine != null) StopCoroutine(_knockBackCoroutine);
        _knockBackCoroutine = StartCoroutine(KnockBackRoutine(direction, force, duration));
    }

    private IEnumerator KnockBackRoutine(Vector3 direction, float force, float duration)
    {
        _isKnockedBack = true;
        _rb.linearVelocity = Vector3.zero;
        _rb.AddForce(direction * force, ForceMode.Impulse);
        yield return new WaitForSeconds(duration);
        _isKnockedBack = false;
    }

    private void Move() => _rb.linearVelocity = new Vector3(_direction.x * speed, _rb.linearVelocity.y, _direction.y * speed);

    protected virtual void RotateView()
    {
        if (_direction.sqrMagnitude < 0.01f) return;

        Vector3 moveDir = new Vector3(_direction.x, 0f, _direction.y);
        Quaternion targetRotation = Quaternion.LookRotation(moveDir);
        viewTransform.rotation = Quaternion.Slerp(viewTransform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
    }
}
