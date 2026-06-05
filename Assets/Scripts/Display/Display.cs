using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Display : MonoBehaviour
{
    [Header("Health Bar")]
    [SerializeField] protected Slider healthBarSlider;
    [SerializeField] protected TextMeshProUGUI healthTmp;
    [SerializeField] private float healthBarLerpSpeed = 0.1f;

    private Health _health;
    private Animator _animator;

    protected virtual void Start()
    {
        _animator = GetComponentInChildren<Animator>();
        _health = GetComponent<Health>();
        
        healthBarSlider.maxValue = _health.MaxHealth;
        healthBarSlider.value = _health.CurrentHealth;

        if (!healthTmp) return;
            
        _health.OnHealthChanged += UpdateHealthTmp;
        UpdateHealthTmp();
        
    }
    
    private void Update() => healthBarSlider.value = Mathf.Lerp(healthBarSlider.value, _health.CurrentHealth, healthBarLerpSpeed);

    public void Attack() => _animator.SetTrigger("Attack");
    
    private void UpdateHealthTmp() => healthTmp.SetText(_health.CurrentHealth.ToString());
}
