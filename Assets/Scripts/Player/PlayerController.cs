using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    public static PlayerController playerController;

    [Header("Area Detection")]
    [SerializeField] private float raycastDistance = 1.5f;
    [SerializeField] private LayerMask areaMask;

    private TypeDisplay typeDisplay;
    private List<TypeColor> _unlockedTypes = new();
    private int _typeIndex;

    private TypeColor _selectedType = TypeColor.WHITE;
    private TypeColor SelectedType
    {
        get => _selectedType;
        set { _selectedType = value; typeDisplay?.SetType(Type); }
    }

    private TypeColor _areaType = TypeColor.WHITE;
    private TypeColor AreaType
    {
        get => _areaType;
        set { _areaType = value; if (_selectedType == TypeColor.WHITE) typeDisplay?.SetType(Type); }
    }

    public override TypeColor Type => _selectedType == TypeColor.WHITE ? _areaType : _selectedType;

    private void Awake() => playerController = this;

    private void OnEnable()  => MiniBossHealth.OnDeath += AddType;
    private void OnDisable() => MiniBossHealth.OnDeath -= AddType;

    private void Start()
    {
        typeDisplay = GetComponent<TypeDisplay>();
        _unlockedTypes.Add(TypeColor.WHITE);
    }

    #region Inputs

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started && canAttack) Attack();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _typeIndex = (_typeIndex + 1) % _unlockedTypes.Count;
        SelectedType = _unlockedTypes[_typeIndex];
    }

    #endregion

    protected override void Attack()
    {
        typeDisplay.Attack();
        StartCoroutine(AttackRoutine());
    }

    public void AddType(TypeColor newType)
    {
        if (!_unlockedTypes.Contains(newType)) _unlockedTypes.Add(newType);
    }

    public void ClearTypes()
    {
        _unlockedTypes.RemoveAll(t => t != TypeColor.WHITE);
        _typeIndex = 0;
        SelectedType = TypeColor.WHITE;
    }

    private Collider _lastAreaCollider;
    private Area _cachedArea;

    private void FixedUpdate()
    {
        TypeColor detected = TypeColor.WHITE;
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, raycastDistance, areaMask))
        {
            if (hit.collider != _lastAreaCollider)
            {
                _lastAreaCollider = hit.collider;
                _cachedArea = hit.collider.GetComponent<Area>();
            }
            if (_cachedArea) detected = _cachedArea.Type;
        }
        else _lastAreaCollider = null;
        AreaType = detected;
    }
}
