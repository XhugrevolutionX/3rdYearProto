using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class EnemyHealth : Health
{
    [Header("Drop")] 
    [SerializeField] private XP xpPrefab;
    [SerializeField] private int minDrop;
    [SerializeField] private int maxDrop;
    [SerializeField] private float minForce = 5f;
    [SerializeField] private float maxForce = 15f;
    [SerializeField] private float minUpForce = 5f;
    [SerializeField] private float maxUpForce = 15f;
    
    public event Action OnDeath;

    protected override void Death()
    {
        OnDeath?.Invoke();
        DropXP();
        Destroy(gameObject);
    }

    private void DropXP()
    {
        if (xpPrefab == null) return;

        int count = Random.Range(minDrop, maxDrop + 1);
        for (int i = 0; i < count; i++)
        {
            var xp = Instantiate(xpPrefab, transform.position, Quaternion.identity);

            Vector3 horizontal = Random.insideUnitSphere;
            horizontal.y = 0f;
            horizontal    = horizontal.normalized * Random.Range(minForce, maxForce);

            xp.Rb.AddForce(horizontal + Vector3.up * Random.Range(minUpForce, maxUpForce), ForceMode.Impulse);
        }
    }
}