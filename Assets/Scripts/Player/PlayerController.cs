using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : Controller
{
    public static PlayerController playerController;
    
    private PlayerDisplay _playerDisplay;
    
    private List<TypeColor> _unlockedTypes = new();
    
    private int _typeIndex;

    private void Awake() => playerController = this;
    
    private void Start()
    {
        _playerDisplay = GetComponent<PlayerDisplay>();
        _unlockedTypes.Add(TypeColor.WHITE);
    }

    #region Inputs

    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started && canAttack) Attack();
    }

    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (!context.started) return;
        _typeIndex = (_typeIndex + 1) % _unlockedTypes.Count;
        SwitchType(_unlockedTypes[_typeIndex]);
    }

    #endregion

    protected override void Attack()
    {
        _playerDisplay.Attack();
        StartCoroutine(AttackRoutine());
    }

    public void SwitchType(TypeColor newType)
    {
        type = newType;
        _playerDisplay.SetType(newType);
        Debug.Log($"Switch color type : {type}");
    }

    public void AddType(TypeColor newType)
    {
        if (!_unlockedTypes.Contains(newType)) _unlockedTypes.Add(newType);
    }

    public void ClearTypes()
    {
        _unlockedTypes.RemoveAll(t => t != TypeColor.WHITE);
        _typeIndex = 0;
        SwitchType(TypeColor.WHITE);
    }
}
