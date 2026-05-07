
// HexGrid.cs
using System.Collections.Generic;
using UnityEngine;

public class HexGrid : MonoBehaviour
{
    [SerializeField] private GameObject hexPrefab;
    [SerializeField] private int radius = 5;
    [SerializeField] private Color[] palette = { Color.white };

    private float _hexSize;
    private readonly Dictionary<Hex, GameObject> _cells = new();
    void Awake()
    {
        var mf = hexPrefab.GetComponentInChildren<MeshFilter>();
        var b = mf.sharedMesh.bounds;
        var scale = hexPrefab.transform.localScale;
        _hexSize = Mathf.Max(b.extents.x * scale.x, b.extents.z * scale.z);
    }

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
                var pos = HexLayout.HexToWorld(hex, _hexSize);
                var cell = Instantiate(hexPrefab, pos, hexPrefab.transform.rotation, transform);
                cell.name = $"Hex({q},{r})";
                ApplyRandomColor(cell);
                _cells[hex] = cell;
            }
        }
    }

    private void ApplyRandomColor(GameObject cell)
    {
        if (palette.Length == 0) return;
        var rend = cell.GetComponentInChildren<Renderer>();
        if (rend == null) return;
        var mat = new Material(rend.sharedMaterial);
        mat.color = palette[Random.Range(0, palette.Length)];
        rend.sharedMaterial = mat;
    }

    public GameObject GetCell(Hex hex) =>
        _cells.TryGetValue(hex, out var cell) ? cell : null;
}
