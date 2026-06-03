using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Parameter")]
    [SerializeField] private float timer = 300f;

    [Header("Grid")]
    [SerializeField] private GridGenerator gridGenerator;

    public static GameManager Instance { get; private set; }

    public float Timer => timer;

    /// <summary>One spawn point per biome type, at the largest passable cluster's centroid. Populated after grid generation.</summary>
    public List<Transform> MiniBossSpawnPoints => gridGenerator.BiomeSpawnPoints;

    /// <summary>Spawn point at the center of the neutral boss arena. Null until grid generation completes.</summary>
    public Transform BossSpawnPoint => gridGenerator.NeutralZoneSpawnPoint;

    /// <summary>World positions of every passable, non-neutral-zone hex. Use these to spawn normal enemies.</summary>
    public List<Vector3> EnemySpawnPositions => gridGenerator.PassableHexPositions;

    /// <summary>Passable hex positions grouped by biome color. Spawn the right enemy type per color.</summary>
    public Dictionary<TypeColor, List<Vector3>> EnemySpawnPositionsByBiome => gridGenerator.PassableHexPositionsByBiome;

    /// <summary>Mini boss spawn points keyed by biome color.</summary>
    public Dictionary<TypeColor, Transform> MiniBossSpawnPointsByBiome => gridGenerator.MiniBossSpawnPointsByBiome;
    
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
    }
}