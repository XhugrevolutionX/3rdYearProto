using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private Vector3 zoneCenter    = Vector3.zero;
    [SerializeField] private float   initialRadius  = 40f;
    [SerializeField] private float   finalRadius    = 4f;
    [SerializeField] private float   shrinkDuration = 300f;

    [Header("Behaviour")]
    [SerializeField] private bool shrinkOnStart = false;

    public float   CurrentRadius => _radius;
    public Vector3 ZoneCenter    => zoneCenter;
    public bool    IsShrinking   => _isShrinking;

    public bool IsInsideZone(Vector3 worldPos)
    {
        float dx = worldPos.x - zoneCenter.x;
        float dz = worldPos.z - zoneCenter.z;
        return dx * dx + dz * dz <= _radius * _radius;
    }

    private static readonly int PropCenter = Shader.PropertyToID("_ZoneCenter");
    private static readonly int PropRadius = Shader.PropertyToID("_ZoneRadius");

    private float _radius;
    private float _elapsed;
    private bool  _isShrinking;

    private void Start()
    {
        _radius = initialRadius;
        PushToShader();

        if (shrinkOnStart)
            StartShrink();
    }

    private void Update()
    {
        if (!_isShrinking || _radius <= finalRadius) return;

        _elapsed += Time.deltaTime;
        _radius   = Mathf.Lerp(initialRadius, finalRadius, Mathf.Clamp01(_elapsed / shrinkDuration));

        PushToShader();
    }

    public void StartShrink() => _isShrinking = true;

    private void PushToShader()
    {
        Shader.SetGlobalVector(PropCenter, new Vector4(zoneCenter.x, zoneCenter.y, zoneCenter.z, 0f));
        Shader.SetGlobalFloat(PropRadius, _radius);
    }
}
