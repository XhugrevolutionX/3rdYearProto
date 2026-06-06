using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController boss;
    [SerializeField] private ZoneManager zoneManager;
    
    [Header("Parameter")]
    [SerializeField] private float timer = 300f;
    [SerializeField] private Vector3 bossSpawnOffset = new(0, 0, -3f);

    [Header("Grid")]
    [SerializeField] private GridGenerator gridGenerator;
    
    [Header("Events")]
    [SerializeField] private UnityEvent OnGameOver;

    public static GameManager Instance { get; private set; }

    public event Action OnTimerEnd;
    
    public float Timer => timer;
    
    private bool _gameFinished;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    private void Start() => Time.timeScale = 1f;

    private void Update()
    {
        if (_gameFinished) return;
        
        timer -= Time.deltaTime;

        if (timer <= 0)
        {
            timer = 0;
            OnTimerFinished();
        }
    }

    private void OnTimerFinished()
    {
        _gameFinished = true;

        Instantiate(boss, gridGenerator.NeutralZoneSpawnPoint.position + bossSpawnOffset, Quaternion.Euler(0f, 180f, 0f));

        zoneManager?.StartShrink();
        OnTimerEnd?.Invoke();
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        Time.timeScale = 0;
    }
}