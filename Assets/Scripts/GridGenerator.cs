
// HexGrid.cs
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private float hexSize = 1f;
    [SerializeField] private int radius = 5;

    private readonly Dictionary<Hex, GameObject> _cells = new();

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        // Generates a filled hexagonal map of the given radius
        for (int q = -radius; q <= radius; q++)
        {
            int r1 = Mathf.Max(-radius, -q - radius);
            int r2 = Mathf.Min(radius, -q + radius);
            for (int r = r1; r <= r2; r++)
            {
                var hex = new Hex(q, r);
                var pos = HexLayout.HexToWorld(hex, hexSize);
                var cell = Instantiate(hexPrefab, pos, hexPrefab.transform.rotation, transform);
                cell.name = $"Hex({q},{r})";
                _cells[hex] = cell;
            }
        }
    }

    public GameObject GetCell(Hex hex) =>
        _cells.TryGetValue(hex, out var cell) ? cell : null;
}
