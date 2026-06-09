using UnityEngine;
using UnityEngine.Rendering.Universal;

public class MeteoriteStrike : MonoBehaviour
{
    [Header("Meteorite")]
    [SerializeField] private Meteorite meteoritePrefab;
    [SerializeField] private float fallDuration = 3f;
    [SerializeField] private float spawnHeight = 20f;

    [Header("Impact")]
    [SerializeField] private float impactRadius = 3f;
    [SerializeField] private int damage = 30;
    [SerializeField] private KnockBackData knockBackData;
    [SerializeField] private LayerMask playerLayer;

    [Header("References")]
    [SerializeField] private DecalProjector decalProjector;
    
    [Header("Colors")]
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material greenMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material yellowMaterial;
    [SerializeField] private Material orangeMaterial;
    [SerializeField] private Material purpleMaterial;

    private Meteorite _meteorite;
    private Vector3 _start;
    private Vector3 _end;
    private float _elapsed;
    private bool _impacted;
    private Controller _attacker;
    private Color _targetColor;
    private Material _decalMaterial;

    public void Init(Vector3 groundPosition, Controller attacker, TypeColor type)
    {
        _attacker = attacker;
        _end = groundPosition;
        transform.position = groundPosition;

        _start = groundPosition + Vector3.up * spawnHeight;
        _meteorite = Instantiate(meteoritePrefab, _start, Quaternion.identity);

        _meteorite.MeteoriteRenderer.material = TypeToMaterial(type);

        _targetColor = TypeToColor(type);
        _decalMaterial = new Material(decalProjector.material);
        decalProjector.material = _decalMaterial;
        _decalMaterial.SetColor("_BaseColor", Color.white);
    }

    private void Update()
    {
        if (_impacted) return;

        _elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(_elapsed / fallDuration);

        _meteorite.transform.position = Vector3.Lerp(_start, _end, t);
        _decalMaterial.SetColor("_BaseColor", Color.Lerp(Color.white, _targetColor, t));

        if (t >= 1f) Impact();
    }

    private void Impact()
    {
        _impacted = true;

        foreach (var col in Physics.OverlapSphere(_end, impactRadius, playerLayer))
            CombatUtils.ApplyHit<Health>(col.gameObject, _attacker, damage, knockBackData);

        Destroy(_meteorite.gameObject);
        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0f, 0.4f);
        Gizmos.DrawSphere(transform.position, impactRadius);
        Gizmos.color = new Color(1f, 0.3f, 0f, 1f);
        Gizmos.DrawWireSphere(transform.position, impactRadius);
    }

    private Material TypeToMaterial(TypeColor type) => type switch
    {
        TypeColor.RED    => redMaterial,
        TypeColor.GREEN  => greenMaterial,
        TypeColor.BLUE   => blueMaterial,
        TypeColor.YELLOW => yellowMaterial,
        TypeColor.ORANGE => orangeMaterial,
        TypeColor.PURPLE => purpleMaterial,
        _                => redMaterial
    };

    private static Color TypeToColor(TypeColor type) => type switch
    {
        TypeColor.RED    => Color.red,
        TypeColor.GREEN  => Color.green,
        TypeColor.BLUE   => Color.blue,
        TypeColor.YELLOW => Color.yellow,
        TypeColor.ORANGE => new Color(1f, 0.5f, 0f),
        TypeColor.PURPLE => new Color(0.5f, 0f, 0.5f),
        _                => Color.white
    };
}
