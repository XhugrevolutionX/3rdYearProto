using System;
using UnityEngine;

public class PlayerLevelSystem : MonoBehaviour
{
    [Header("Parameters")] 
    [Tooltip("XP needed to reach level 2, determines the starting difficulty curve")]
    [SerializeField][Min(1)] private int _startXpToNextLevel = 100;
    
    [Tooltip("Multiplier applied to XP required for the next level, higher values make leveling slower over time.")]
    [SerializeField][Min(1.1f)] private float _xpMultiplier = 1.2f;

    [Tooltip("Flat damage added to the player each level up.")]
    [SerializeField][Min(1)] private int _damagePerLevel = 1;
    
    public static PlayerLevelSystem Instance { get; private set; }

    public event Action<int, int> OnGainXP;
    public event Action<int, int> OnLevelUp;
    
    private PlayerController _playerController;
    
    private int _level;
    private int _currentXP;
    private int _xpToNextLevel;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
        
        _xpToNextLevel = _startXpToNextLevel;
    }
    
    private void Start() => _playerController = GetComponent<PlayerController>();

    public void GainXP(int xp)
    {
        _currentXP += xp;
        
        while (_currentXP >= _xpToNextLevel) // if _currentXP is still more than the new _xpToNextLevel, level up again
        {
            _currentXP -= _xpToNextLevel;
            LevelUp();
        }
        
        OnGainXP?.Invoke(_currentXP, _xpToNextLevel);
    }

    private void LevelUp()
    {
        _level++;
        _xpToNextLevel = Mathf.RoundToInt(_xpToNextLevel * _xpMultiplier);
        _playerController?.AddDamage(_damagePerLevel);

        OnLevelUp?.Invoke(_level, _xpToNextLevel);
    }
}