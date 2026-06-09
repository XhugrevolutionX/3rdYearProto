using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossController : EnemyController
{
    [Header("Boss - Type Cycling")]
    [SerializeField] private float typeSwitchDuration = 8f;
    [SerializeField] private List<TypeColor> types = new();

    private TypeDisplay _typeDisplay;
    private int _typeIndex;

    protected class BossPhase
    {
        public readonly float HealthThreshold;
        public readonly BossPattern[] Patterns;
        public bool Activated;

        public BossPhase(float healthThreshold, BossPattern[] patterns)
        {
            HealthThreshold = healthThreshold;
            Patterns = patterns;
        }
    }

    public bool AttackAnimationDone { get; private set; }

    private Health _health;
    private BossPhase[] _phases = Array.Empty<BossPhase>();

    protected override void Start()
    {
        PlayerTransform = PlayerController.playerController.transform;
        
        base.Start();
        _typeDisplay = GetComponent<TypeDisplay>();
        _health = GetComponent<Health>();

        _phases = CreatePhases();
        Array.Sort(_phases, (a, b) => b.HealthThreshold.CompareTo(a.HealthThreshold));

        _health.OnHealthChanged += CheckPhaseTransitions;


        if (types.Count > 0) StartCoroutine(TypeSwitchRoutine());
    }

    private void OnDestroy() => _health.OnHealthChanged -= CheckPhaseTransitions;

    protected override void DetectPlayer() { }

    protected virtual BossPhase[] CreatePhases() => Array.Empty<BossPhase>();

    public void PlayAttack(string triggerName)
    {
        AttackAnimationDone = false;
        _typeDisplay.PlayAnimation(triggerName);
    }

    // called by animation event
    public void OnAttackComplete() => AttackAnimationDone = true;

    private void CheckPhaseTransitions()
    {
        float ratio = (float)_health.CurrentHealth / _health.MaxHealth;

        foreach (var phase in _phases)
        {
            if (phase.Activated || ratio > phase.HealthThreshold) continue;
            phase.Activated = true;
            _stateMachine.ChangeState(new BossPatternState(this, _stateMachine, phase.Patterns));
            return;
        }
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
