using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private EnemyController[] enemyPrefabs;

    [Header("Parameters")]
    [SerializeField] private int maximumEnemy = 50;
    [SerializeField] private float minimumRadius = 5f;
    [SerializeField] private float maximumRadius = 100f;

    private readonly List<EnemyController> _enemies = new();

    private void Start() => FillToMax();

    private void FillToMax()
    {
        while (_enemies.Count < maximumEnemy)
            SpawnOne();
    }

    private void SpawnOne()
    {
        if (enemyPrefabs == null || enemyPrefabs.Length == 0) return;

        float angle  = Random.Range(0f, 360f) * Mathf.Deg2Rad;
        float radius = Random.Range(minimumRadius, maximumRadius);
        Vector3 pos  = transform.position + new Vector3(Mathf.Cos(angle) * radius, 0f, Mathf.Sin(angle) * radius);

        var prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];
        var enemy  = Instantiate(prefab, pos, Quaternion.identity, transform);

        var health = enemy.GetComponentInChildren<EnemyHealth>();
        if (health) health.OnDeath += () => { _enemies.Remove(enemy); SpawnOne(); };

        _enemies.Add(enemy);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        DrawCircle(minimumRadius);
        Gizmos.color = Color.red;
        DrawCircle(maximumRadius);
    }

    private void DrawCircle(float radius)
    {
        int segments = 64;
        float step   = 360f / segments * Mathf.Deg2Rad;
        for (int i = 0; i < segments; i++)
        {
            float a0 = i * step, a1 = (i + 1) * step;
            Vector3 p0 = transform.position + new Vector3(Mathf.Cos(a0) * radius, 0f, Mathf.Sin(a0) * radius);
            Vector3 p1 = transform.position + new Vector3(Mathf.Cos(a1) * radius, 0f, Mathf.Sin(a1) * radius);
            Gizmos.DrawLine(p0, p1);
        }
    }
}
