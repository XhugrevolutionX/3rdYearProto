using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController boss;
    [SerializeField] private ZoneManager zoneManager;
    
    [Header("Parameter")]
    [SerializeField] private float timer = 300f;

    [Header("Grid")]
    [SerializeField] private GridGenerator gridGenerator;

    public static GameManager Instance { get; private set; }

    public event Action OnTimerEnd;
    
    public float Timer => timer;
    
    private bool _gameFinished;

    private void Awake()
    {
        if(Instance && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

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

        Instantiate(boss, gridGenerator.NeutralZoneSpawnPoint.position, Quaternion.identity);

        zoneManager?.StartShrink();
        OnTimerEnd?.Invoke();
    }
}