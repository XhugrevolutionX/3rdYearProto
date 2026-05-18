using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [Header("Boss Parameters")]
    [SerializeField] private float typeSwitchDuration = 8f;
    [SerializeField] private List<TypeColor> types = new();

    private TypeDisplay _typeDisplay;
    private int _typeIndex;

    protected override void Start()
    {
        base.Start();
        _typeDisplay = GetComponent<TypeDisplay>();

        if (types.Count > 0) StartCoroutine(TypeSwitchRoutine());
    }

    private IEnumerator TypeSwitchRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(typeSwitchDuration);
            _typeIndex = (_typeIndex + 1) % types.Count;
            type = types[_typeIndex];
            _typeDisplay.SetType(type);
        }
    }
}