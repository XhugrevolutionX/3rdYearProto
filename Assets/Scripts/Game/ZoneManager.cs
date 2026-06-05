using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    [Header("Zone")]
    [SerializeField] private Vector3 zoneCenter    = Vector3.zero;
    [SerializeField] private float   initialRadius  = 40f;
    [SerializeField] private float   finalRadius    = 4f;
    [SerializeField] private float   shrinkDuration = 300f;

    [Header("Visual")]
    [Tooltip("Material using the Custom/BattleZone shader.")]
    [SerializeField] private Material overlayMaterial;
    [Tooltip("Half-size of the overlay quad — must cover the whole map.")]
    [SerializeField] private float    overlayExtent = 150f;

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

    // ─────────────────────────────────────────────────────────────────────────
    private static readonly int PropCenter = Shader.PropertyToID("_ZoneCenter");
    private static readonly int PropRadius = Shader.PropertyToID("_ZoneRadius");

    private float _radius;
    private float _elapsed;
    private bool  _isShrinking;

    private void Start()
    {
        _radius = initialRadius;
        CreateOverlayPlane();
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
    

    // private void OnGUI()
    // {
    //     GUI.Label(new Rect(10, 10, 400, 20), $"[Zone] radius={_radius:F1}  elapsed={_elapsed:F1}s");
    // }

    private void CreateOverlayPlane()
    {
        if (overlayMaterial == null)
        {
            Debug.LogWarning("ZoneManager: no overlay material assigned.");
            return;
        }

        var go = new GameObject("ZoneOverlay");
        go.transform.SetParent(transform);
        go.transform.position = new Vector3(zoneCenter.x, zoneCenter.y, zoneCenter.z);

        var mf = go.AddComponent<MeshFilter>();
        var mr = go.AddComponent<MeshRenderer>();
        mf.sharedMesh         = BuildQuad(overlayExtent);
        mr.sharedMaterial     = overlayMaterial;
        mr.shadowCastingMode  = UnityEngine.Rendering.ShadowCastingMode.Off;
        mr.receiveShadows     = false;
    }

    private void PushToShader()
    {
        Shader.SetGlobalVector(PropCenter, new Vector4(zoneCenter.x, zoneCenter.y, zoneCenter.z, 0f));
        Shader.SetGlobalFloat(PropRadius, _radius);
    }

    private static Mesh BuildQuad(float halfSize)
    {
        float h  = halfSize;
        var mesh = new Mesh
        {
            name = "ZoneOverlayQuad",
            vertices = new[] { new Vector3(-h, 0, -h), new Vector3(h, 0, -h), new Vector3(h, 0, h), new Vector3(-h, 0, h) },
            triangles = new[] { 0, 2, 1, 0, 3, 2 },
            uv = new[] { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1) }
        };
        mesh.RecalculateNormals();
        return mesh;
    }
}
