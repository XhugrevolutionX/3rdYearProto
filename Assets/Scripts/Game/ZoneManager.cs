using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private float initialRadius = 40f;
    [SerializeField] private float finalRadius = 4f;
    [SerializeField] private float shrinkDuration = 300f;

    [Header("References")]
    [Tooltip("If assigned, the zone moves toward the boss spawn point when shrinking starts.")]
    [SerializeField] private GridGenerator gridGenerator;
    [SerializeField] private PlayerHealth playerHealth;

    [Header("Out-of-Zone Damage")]
    [SerializeField] private int   outOfZoneDamage = 10;
    [SerializeField] private float damageInterval = 1f;

    [Header("Behaviour")]
    [SerializeField] private bool shrinkOnStart = false;

    public float CurrentRadius => _radius;
    public Vector3 ZoneCenter => _center;
    public bool IsShrinking => _isShrinking;

    private bool IsInsideZone(Vector3 worldPos)
    {
        float dx = worldPos.x - _center.x;
        float dz = worldPos.z - _center.z;
        return dx * dx + dz * dz <= _radius * _radius;
    }

    private static readonly int PropCenter = Shader.PropertyToID("_ZoneCenter");
    private static readonly int PropRadius = Shader.PropertyToID("_ZoneRadius");

    private float   _radius;
    private float   _elapsed;
    private bool    _isShrinking;
    private Vector3 _center;
    private Vector3 _startCenter;
    private Vector3 _targetCenter;
    private float   _damageTimer;

    private void Start()
    {
        _center       = Vector3.zero;
        _targetCenter = Vector3.zero;

        if (gridGenerator != null && gridGenerator.NeutralZoneSpawnPoint != null)
            _targetCenter = gridGenerator.NeutralZoneSpawnPoint.position;

        _radius = initialRadius;
        PushToShader();

        if (shrinkOnStart)
            StartShrink();
    }

    private void Update()
    {
        if (_isShrinking && _radius > finalRadius)
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / shrinkDuration);

            _radius = Mathf.Lerp(initialRadius, finalRadius, t);
            _center = Vector3.Lerp(_startCenter, _targetCenter, t);

            PushToShader();
        }

        if (_isShrinking)
            TickOutOfZoneDamage();
    }

    private void TickOutOfZoneDamage()
    {
        if (playerHealth == null) return;
        if (IsInsideZone(playerHealth.transform.position)) { _damageTimer = 0f; return; }

        _damageTimer += Time.deltaTime;
        if (_damageTimer < damageInterval) return;

        _damageTimer = 0f;
        playerHealth.ChangeHealth(-outOfZoneDamage);
    }

    public void StartShrink()
    {
        _startCenter = _center;
        _isShrinking = true;
    }

    private void PushToShader()
    {
        Shader.SetGlobalVector(PropCenter, new Vector4(_center.x, _center.y, _center.z, 0f));
        Shader.SetGlobalFloat(PropRadius, _radius);
    }
}
