using System.Collections.Generic;
using UnityEngine;

public enum GridType { Hex, Square, Octagon }

/// <summary>
/// A named biome type with separate passable and blocking mesh pools.
/// During generation each tile rolls against blockingChance to decide which
/// pool to draw from; a random mesh is then picked within that pool.
/// </summary>
[System.Serializable]
public class HexBiomeType
{
    public string name = "Biome";

    [Tooltip("Color identity for this biome, assigned to the Area component on every tile.")]
    public TypeColor color = TypeColor.WHITE;

    [Tooltip("Meshes used for tiles the player can walk through.")]
    public Mesh[] passableMeshes = {};

    [Tooltip("Meshes used for tiles that block movement.")]
    public Mesh[] blockingMeshes = {};

    [Range(0f, 1f)]
    [Tooltip("Probability (0–1) that a tile in this biome will be blocking. " +
             "Keep this low (e.g. 0.2) for sparse obstacles.")]
    public float blockingChance = 0.2f;
}

public class GridGenerator : MonoBehaviour
{
    [SerializeField] public GridType gridType = GridType.Hex;

    // ── Hex ──────────────────────────────────────────────────────────────────
    // One shared prefab is instantiated for every cell.
    // Biomes are seeded with Voronoi; each biome owns its own mesh variants.

    [SerializeField] private GameObject     hexPrefab;
    [SerializeField] private HexBiomeType[] hexBiomeTypes = {};
    [SerializeField] private int            hexRadius = 5;
    [SerializeField] private float          tileScale = 1.5f;

    [Header("Biome Generation")]
    [Tooltip("Number of Voronoi regions. 0 = one region per biome type. " +
             "Use a multiple of biome types to get several blobs per type.")]
    [SerializeField] private int biomeCount = 0;
    [Tooltip("Minimum hex-distance enforced between two seed points. 0 = auto.")]
    [SerializeField] private int biomeSeedSeparation = 0;

    [Header("Mini Boss Spawn Points")]
    [Tooltip("Create one spawn-point Transform per biome type, placed at the centroid of that type's largest cluster.")]
    [SerializeField] private bool spawnBiomeSpawnPoints = true;
    [Tooltip("Optional prefab instantiated at each spawn point. Leave empty for a plain empty GameObject.")]
    [SerializeField] private GameObject spawnPointPrefab;
    [Tooltip("Height above the ground at which spawn points are placed.")]
    [SerializeField] private float spawnPointYOffset = 0.5f;

    [Header("Neutral Zone (Boss Arena)")]
    [Tooltip("Biome used for the 7-hex neutral zone (center + 6 neighbours).")]
    [SerializeField] private HexBiomeType neutralBiome;
    [Tooltip("Optional prefab instantiated at the boss spawn point. Leave empty for a plain empty GameObject.")]
    [SerializeField] private GameObject bossSpawnPointPrefab;
    [Tooltip("Pick a random valid position each generation instead of using the coordinates below.")]
    [SerializeField] private bool randomizeNeutralPosition = false;
    [Tooltip("Axial Q coordinate of the neutral zone center (ignored when Randomize is on).")]
    [SerializeField] private int neutralCenterQ = 0;
    [Tooltip("Axial R coordinate of the neutral zone center (ignored when Randomize is on).")]
    [SerializeField] private int neutralCenterR = 0;

    // ── Square ───────────────────────────────────────────────────────────────
    [SerializeField] private GameObject squarePrefab;
    [SerializeField] private int squareColumns = 10;
    [SerializeField] private int squareRows = 10;

    // ── Octagon ──────────────────────────────────────────────────────────────
    [SerializeField] private GameObject octagonPrefab;
    [SerializeField] private GameObject octagonFillPrefab;
    [SerializeField] private int octagonRadius = 5;

    // ── Shared ───────────────────────────────────────────────────────────────
    [SerializeField] private Color[] palette = { Color.white };

    // ── Runtime state ────────────────────────────────────────────────────────
    private readonly Dictionary<Hex, GameObject> _hexCells     = new();
    private readonly Dictionary<Hex, int>        _hexTileTypes = new(); // biome type index per cell
    private readonly Dictionary<Hex, bool>       _hexBlocking  = new(); // true = blocks movement
    private readonly HashSet<Hex>                _neutralZone  = new();
    private readonly Dictionary<Vector2Int, GameObject> _squareCells  = new();
    private readonly Dictionary<Vector2Int, GameObject> _octagonCells = new();

    private float _hexCellSize;

    /// <summary>Index used in _hexTileTypes for neutral-zone tiles (one past the regular biome array).</summary>
    public int NeutralBiomeIndex => hexBiomeTypes.Length;

    /// <summary>Center hex of the neutral zone, set after each generation.</summary>
    public Hex NeutralZoneCenter { get; private set; }

    /// <summary>
    /// Contiguous same-biome clusters found after generation, sorted largest-first.
    /// Each inner list contains the Hex coords belonging to that cluster.
    /// </summary>
    public List<List<Hex>> BiomeGroups { get; private set; } = new();

    /// <summary>
    /// One spawn-point Transform per regular biome type, placed at the centroid of that
    /// type's largest contiguous cluster. Populated after each <see cref="GenerateGrid"/> call.
    /// Indexed in the same order as <see cref="hexBiomeTypes"/>.
    /// </summary>
    public List<Transform> BiomeSpawnPoints { get; private set; } = new();

    /// <summary>Spawn-point Transform at the center of the neutral boss arena. Null when no neutral biome is set.</summary>
    [SerializeField] private Transform _neutralZoneSpawnPoint;
    public Transform NeutralZoneSpawnPoint
    {
        get => _neutralZoneSpawnPoint;
        private set => _neutralZoneSpawnPoint = value;
    }

    /// <summary>World positions of every passable, non-neutral-zone hex. Populated after each <see cref="GenerateGrid"/> call.</summary>
    public List<Vector3> PassableHexPositions { get; private set; } = new();

    /// <summary>Passable hex positions grouped by biome color. Use this to spawn the right enemies per biome.</summary>
    public Dictionary<TypeColor, List<Vector3>> PassableHexPositionsByBiome { get; private set; } = new();

    /// <summary>Mini boss spawn points keyed by biome color for direct lookup.</summary>
    public Dictionary<TypeColor, Transform> MiniBossSpawnPointsByBiome { get; private set; } = new();

    //void Start() => GenerateGrid();

    public void GenerateGrid()
    {
        ClearGrid();
        switch (gridType)
        {
            case GridType.Hex:     GenerateHex();     break;
            case GridType.Square:  GenerateSquare();  break;
            case GridType.Octagon: GenerateOctagon(); break;
        }
    }

    // ── Hex ──────────────────────────────────────────────────────────────────

    private void GenerateHex()
    {
        if (hexPrefab == null || hexBiomeTypes == null || hexBiomeTypes.Length == 0) return;

        float size = HexCellSize();
        _hexCellSize = size;

        // 1. Collect every hex coordinate.
        var allHexes = new List<Hex>();
        for (int q = -hexRadius; q <= hexRadius; q++)
        {
            int r1 = Mathf.Max(-hexRadius, -q - hexRadius);
            int r2 = Mathf.Min(hexRadius,  -q + hexRadius);
            for (int r = r1; r <= r2; r++)
                allHexes.Add(new Hex(q, r));
        }

        // 2. Carve out the neutral zone so Voronoi seeds never land inside it.
        BuildNeutralZone(allHexes);
        var voronoiHexes = allHexes.FindAll(h => !_neutralZone.Contains(h));

        // 3. Place biome seeds spread across the non-neutral grid.
        var seeds = PlaceBiomeSeeds(voronoiHexes);

        // 4. Assign types: neutral zone gets the sentinel index, rest use Voronoi.
        foreach (var hex in allHexes)
        {
            int biomeIndex = _neutralZone.Contains(hex)
                ? NeutralBiomeIndex
                : NearestSeedType(hex, seeds);

            var pos = HexLayout.HexToWorld(hex, size);
            SpawnHex(pos, hexPrefab.transform.rotation, $"Hex({hex.Q},{hex.R})", biomeIndex,
                     out var cell, out bool blocking);
            _hexCells[hex]     = cell;
            _hexTileTypes[hex] = biomeIndex;
            _hexBlocking[hex]  = blocking;
        }

        PassableHexPositions = new List<Vector3>();
        PassableHexPositionsByBiome = new Dictionary<TypeColor, List<Vector3>>();
        foreach (var (hex, cell) in _hexCells)
        {
            if (_neutralZone.Contains(hex)) continue;
            if (_hexBlocking.TryGetValue(hex, out bool blocking) && blocking) continue;

            var pos = cell.transform.position;
            PassableHexPositions.Add(pos);

            if (_hexTileTypes.TryGetValue(hex, out int biomeIndex) && biomeIndex < hexBiomeTypes.Length)
            {
                var color = hexBiomeTypes[biomeIndex].color;
                if (!PassableHexPositionsByBiome.ContainsKey(color))
                    PassableHexPositionsByBiome[color] = new List<Vector3>();
                PassableHexPositionsByBiome[color].Add(pos);
            }
        }

        BiomeGroups = DetectBiomes();

        if (spawnBiomeSpawnPoints)
            PlaceBiomeSpawnPoints();

        PlaceNeutralZoneSpawnPoint();

        // TODO: once fused-tile meshes exist, iterate BiomeGroups here and
        // swap tiles above a minimum cluster size for their fused mesh variant.
    }

    // Picks the neutral zone center (random or fixed), then fills _neutralZone
    // with the center hex and its 6 neighbours.  Only hexes that exist in the
    // grid are added, so an off-center placement near the edge is safe.
    private void BuildNeutralZone(List<Hex> allHexes)
    {
        _neutralZone.Clear();
        if (neutralBiome == null) return;

        var gridSet = new HashSet<Hex>(allHexes);
        var origin  = new Hex(0, 0);

        Hex center;
        if (randomizeNeutralPosition)
        {
            // Valid centers: all 7 tiles must fit, so distance ≤ hexRadius - 1.
            var validCenters = allHexes.FindAll(h => h.DistanceTo(origin) <= hexRadius - 1);
            center = validCenters.Count > 0
                ? validCenters[Random.Range(0, validCenters.Count)]
                : origin;
        }
        else
        {
            center = new Hex(neutralCenterQ, neutralCenterR);
            // Clamp to a valid position if the configured coords are out of range.
            if (center.DistanceTo(origin) > hexRadius - 1)
            {
                Debug.LogWarning($"GridGenerator: neutral zone center ({neutralCenterQ},{neutralCenterR}) " +
                                 "is too close to the edge — falling back to grid origin.");
                center = origin;
            }
        }

        NeutralZoneCenter = center;
        if (gridSet.Contains(center)) _neutralZone.Add(center);
        foreach (var n in center.Neighbours())
            if (gridSet.Contains(n)) _neutralZone.Add(n);
    }

    // Resolves a biome by index, returning the neutral biome for the sentinel value.
    private HexBiomeType ResolveBiome(int biomeIndex) =>
        biomeIndex < hexBiomeTypes.Length ? hexBiomeTypes[biomeIndex] : neutralBiome;

    // Seeds cycle through biome types so every type appears roughly equally.
    // A minimum separation is enforced so seeds cannot cluster together.
    private List<(Hex position, int biomeIndex)> PlaceBiomeSeeds(List<Hex> allHexes)
    {
        int count      = biomeCount > 0 ? biomeCount : hexBiomeTypes.Length;
        int separation = biomeSeedSeparation > 0
            ? biomeSeedSeparation
            : Mathf.Max(1, (hexRadius * 2) / Mathf.Max(1, count));

        // Shuffle the full candidate list for uniform coverage.
        var candidates = new List<Hex>(allHexes);
        for (int i = candidates.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            (candidates[i], candidates[j]) = (candidates[j], candidates[i]);
        }

        var seeds       = new List<(Hex, int)>();
        int typeCounter = 0;

        foreach (var candidate in candidates)
        {
            if (seeds.Count >= count) break;

            bool tooClose = false;
            foreach (var (seedPos, _) in seeds)
            {
                if (candidate.DistanceTo(seedPos) < separation)
                { tooClose = true; break; }
            }

            if (!tooClose)
            {
                seeds.Add((candidate, typeCounter % hexBiomeTypes.Length));
                typeCounter++;
            }
        }

        // Fallback: if separation was too strict, fill remaining slots without constraint.
        foreach (var candidate in candidates)
        {
            if (seeds.Count >= count) break;
            bool alreadySeed = false;
            foreach (var (sp, _) in seeds) if (sp.Equals(candidate)) { alreadySeed = true; break; }
            if (!alreadySeed)
            {
                seeds.Add((candidate, typeCounter % hexBiomeTypes.Length));
                typeCounter++;
            }
        }

        return seeds;
    }

    private static int NearestSeedType(Hex hex, List<(Hex position, int biomeIndex)> seeds)
    {
        int bestType = 0;
        int bestDist = int.MaxValue;
        foreach (var (pos, type) in seeds)
        {
            int d = hex.DistanceTo(pos);
            if (d < bestDist) { bestDist = d; bestType = type; }
        }
        return bestType;
    }

    private void SpawnHex(Vector3 pos, Quaternion rot, string cellName, int biomeIndex,
                          out GameObject cell, out bool blocking)
    {
        cell = Instantiate(hexPrefab, pos, rot, transform);
        cell.name = cellName;
        cell.transform.localScale = hexPrefab.transform.localScale * tileScale;

        var biome = ResolveBiome(biomeIndex);

        // Assign the biome color to the Area component so game logic can read it.
        var area = cell.GetComponentInChildren<Area>();
        if (area != null) area.Type = biome.color;

        // Roll against the biome's blocking chance, but only if blocking meshes exist.
        // If a pool is empty we silently fall back to the other one.
        bool hasPassable = biome.passableMeshes is { Length: > 0 };
        bool hasBlocking = biome.blockingMeshes  is { Length: > 0 };

        blocking = hasBlocking && (!hasPassable || Random.value < biome.blockingChance);

        Mesh[] pool = blocking ? biome.blockingMeshes : biome.passableMeshes;
        if (pool == null || pool.Length == 0) { blocking = false; return; }

        var mesh = pool[Random.Range(0, pool.Length)];

        var mf = cell.GetComponentInChildren<MeshFilter>();
        if (mf != null) mf.sharedMesh = mesh;

        // // MeshCollider.sharedMesh is independent — set it explicitly.
        // var mc = cell.GetComponentInChildren<MeshCollider>();
        // if (mc != null) mc.sharedMesh = mesh;
    }

    // Returns the cell size using the prefab's existing mesh, or the first
    // available biome mesh as a fallback if the prefab's MeshFilter is empty.
    private float HexCellSize()
    {
        var mf   = hexPrefab.GetComponentInChildren<MeshFilter>();
        var mesh = (mf != null && mf.sharedMesh != null) ? mf.sharedMesh : FirstAvailableMesh();
        if (mesh == null) return tileScale;
        // Multiply by both the prefab's base scale and tileScale so spacing
        // always matches the actual rendered size of the tiles.
        var s = hexPrefab.transform.localScale * tileScale;
        return Mathf.Max(mesh.bounds.extents.x * s.x, mesh.bounds.extents.z * s.z);
    }

    private Mesh FirstAvailableMesh()
    {
        foreach (var biome in hexBiomeTypes)
        {
            if (biome.passableMeshes != null)
                foreach (var m in biome.passableMeshes) if (m != null) return m;
            if (biome.blockingMeshes != null)
                foreach (var m in biome.blockingMeshes) if (m != null) return m;
        }
        if (neutralBiome != null)
        {
            if (neutralBiome.passableMeshes != null)
                foreach (var m in neutralBiome.passableMeshes) if (m != null) return m;
            if (neutralBiome.blockingMeshes != null)
                foreach (var m in neutralBiome.blockingMeshes) if (m != null) return m;
        }
        return null;
    }

    // ── Biome / cluster detection ─────────────────────────────────────────────
    // Flood-fills contiguous same-type regions, sorted largest-first.

    private List<List<Hex>> DetectBiomes()
    {
        var visited = new HashSet<Hex>();
        var groups  = new List<List<Hex>>();

        foreach (var hex in _hexCells.Keys)
        {
            if (visited.Contains(hex)) continue;

            int type  = _hexTileTypes[hex];
            var group = new List<Hex>();
            var queue = new Queue<Hex>();
            queue.Enqueue(hex);
            visited.Add(hex);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                group.Add(current);

                foreach (var neighbour in current.Neighbours())
                {
                    if (visited.Contains(neighbour))                                     continue;
                    if (!_hexTileTypes.TryGetValue(neighbour, out int neighbourType))    continue;
                    if (neighbourType != type)                                            continue;

                    visited.Add(neighbour);
                    queue.Enqueue(neighbour);
                }
            }

            groups.Add(group);
        }

        groups.Sort((a, b) => b.Count.CompareTo(a.Count));
        return groups;
    }

    // For each regular biome type, finds its largest contiguous cluster in BiomeGroups
    // (BiomeGroups is already sorted largest-first) and places one spawn point at the
    // centroid of that cluster. The neutral zone is skipped.
    private void PlaceBiomeSpawnPoints()
    {
        BiomeSpawnPoints.Clear();
        MiniBossSpawnPointsByBiome.Clear();

        var spawnRoot = new GameObject("BiomeSpawnPoints");
        spawnRoot.transform.SetParent(transform);
        spawnRoot.transform.localPosition = Vector3.zero;

        for (int typeIndex = 0; typeIndex < hexBiomeTypes.Length; typeIndex++)
        {
            // BiomeGroups is sorted largest-first, so the first match is the biggest cluster.
            List<Hex> largestCluster = null;
            foreach (var group in BiomeGroups)
            {
                if (_hexTileTypes.TryGetValue(group[0], out int t) && t == typeIndex)
                {
                    largestCluster = group;
                    break;
                }
            }

            if (largestCluster == null) continue;

            var centroid = Vector3.zero;
            foreach (var hex in largestCluster)
                centroid += HexLayout.HexToWorld(hex, _hexCellSize);
            centroid /= largestCluster.Count;

            // Pick the non-blocking hex whose world position is nearest to the centroid.
            // Falls back to the raw centroid if every tile in the cluster is blocking.
            var spawnPos = centroid;
            float bestDist = float.MaxValue;
            bool found = false;
            foreach (var hex in largestCluster)
            {
                if (_hexBlocking.TryGetValue(hex, out bool blocking) && blocking) continue;
                var worldPos = HexLayout.HexToWorld(hex, _hexCellSize);
                float dist = (worldPos - centroid).sqrMagnitude;
                if (dist < bestDist) { bestDist = dist; spawnPos = worldPos; found = true; }
            }
            if (!found)
                Debug.LogWarning($"GridGenerator: all tiles in biome '{hexBiomeTypes[typeIndex].name}' are blocking — spawn point placed at centroid.");

            spawnPos.y += spawnPointYOffset;

            GameObject spawnGo;
            if (spawnPointPrefab != null)
            {
                spawnGo = Instantiate(spawnPointPrefab, spawnPos, Quaternion.identity, spawnRoot.transform);
            }
            else
            {
                spawnGo = new GameObject();
                spawnGo.transform.SetParent(spawnRoot.transform);
                spawnGo.transform.position = spawnPos;
            }
            spawnGo.name = $"SpawnPoint_{hexBiomeTypes[typeIndex].name}";
            BiomeSpawnPoints.Add(spawnGo.transform);
            MiniBossSpawnPointsByBiome[hexBiomeTypes[typeIndex].color] = spawnGo.transform;
        }
    }

    private void PlaceNeutralZoneSpawnPoint()
    {
        NeutralZoneSpawnPoint = null;
        if (neutralBiome == null) return;

        var pos = HexLayout.HexToWorld(NeutralZoneCenter, _hexCellSize);
        pos.y += spawnPointYOffset;

        GameObject go;
        if (bossSpawnPointPrefab != null)
        {
            go = Instantiate(bossSpawnPointPrefab, pos, Quaternion.identity, transform);
        }
        else
        {
            go = new GameObject();
            go.transform.SetParent(transform);
            go.transform.position = pos;
        }
        go.name = "SpawnPoint_NeutralZone";
        NeutralZoneSpawnPoint = go.transform;
    }

    // ── Accessors ─────────────────────────────────────────────────────────────

    /// <summary>Returns the biome type index (hexBiomeTypes slot) of the given hex, or -1 if absent.</summary>
    public int GetHexTileType(Hex hex) =>
        _hexTileTypes.TryGetValue(hex, out int t) ? t : -1;

    /// <summary>Returns true if the tile at <paramref name="hex"/> belongs to the neutral boss-arena zone.</summary>
    public bool IsNeutralZone(Hex hex) => _neutralZone.Contains(hex);

    /// <summary>Returns true if the tile at <paramref name="hex"/> was spawned as a blocking tile.</summary>
    public bool IsBlocking(Hex hex) =>
        _hexBlocking.TryGetValue(hex, out bool b) && b;

    /// <summary>Returns all neighbour GameObjects that share the same biome type as <paramref name="hex"/>.</summary>
    public List<GameObject> GetSameTypeNeighbours(Hex hex)
    {
        var result = new List<GameObject>();
        if (!_hexTileTypes.TryGetValue(hex, out int type)) return result;

        foreach (var n in hex.Neighbours())
        {
            if (_hexTileTypes.TryGetValue(n, out int nt) && nt == type && _hexCells.TryGetValue(n, out var go))
                result.Add(go);
        }
        return result;
    }

    // ── Square ───────────────────────────────────────────────────────────────

    private void GenerateSquare()
    {
        if (squarePrefab == null) return;
        FullExtent(squarePrefab, out float cellW, out float cellH);

        float offsetX = (squareColumns - 1) * cellW * 0.5f;
        float offsetZ = (squareRows    - 1) * cellH * 0.5f;

        for (int col = 0; col < squareColumns; col++)
        for (int row = 0; row < squareRows;    row++)
        {
            var pos = new Vector3(col * cellW - offsetX, 0f, row * cellH - offsetZ);
            Spawn(squarePrefab, pos, squarePrefab.transform.rotation, $"Square({col},{row})", out var cell);
            _squareCells[new Vector2Int(col, row)] = cell;
        }
    }

    // ── Octagon ──────────────────────────────────────────────────────────────

    private void GenerateOctagon()
    {
        if (octagonPrefab == null) return;
        FullExtent(octagonPrefab, out float w, out float h);
        float spacing = Mathf.Max(w, h);

        for (int col = -octagonRadius; col <= octagonRadius; col++)
        for (int row = -octagonRadius; row <= octagonRadius; row++)
        {
            var pos = new Vector3(col * spacing, 0f, row * spacing);
            Spawn(octagonPrefab, pos, octagonPrefab.transform.rotation, $"Oct({col},{row})", out var cell);
            _octagonCells[new Vector2Int(col, row)] = cell;
        }

        if (octagonFillPrefab == null) return;

        var fillRot = octagonPrefab.transform.rotation * Quaternion.Euler(0f, 45f, 0f);
        float half  = spacing * 0.5f;
        for (int col = -octagonRadius; col < octagonRadius; col++)
        for (int row = -octagonRadius; row < octagonRadius; row++)
        {
            var pos = new Vector3(col * spacing + half, 0f, row * spacing + half);
            Spawn(octagonFillPrefab, pos, fillRot, $"Fill({col},{row})", out _);
        }
    }

    // ── Shared helpers ───────────────────────────────────────────────────────

    public void ClearGrid()
    {
        for (int i = transform.childCount - 1; i >= 0; i--)
            DestroyImmediate(transform.GetChild(i).gameObject);
        _hexCells.Clear();
        _hexTileTypes.Clear();
        _hexBlocking.Clear();
        _neutralZone.Clear();
        _squareCells.Clear();
        _octagonCells.Clear();
        BiomeGroups.Clear();
        BiomeSpawnPoints.Clear();
        NeutralZoneSpawnPoint = null;
        PassableHexPositions.Clear();
        PassableHexPositionsByBiome.Clear();
        MiniBossSpawnPointsByBiome.Clear();
    }

    private void Spawn(GameObject prefab, Vector3 pos, Quaternion rot, string cellName, out GameObject cell)
    {
        cell = Instantiate(prefab, pos, rot, transform);
        cell.name = cellName;
        ApplyRandomColor(cell);
    }

    private static void FullExtent(GameObject prefab, out float width, out float depth)
    {
        var b  = prefab.GetComponentInChildren<MeshFilter>().sharedMesh.bounds;
        var s  = prefab.transform.localScale;
        width  = b.size.x * s.x;
        depth  = b.size.z * s.z;
    }

    private void ApplyRandomColor(GameObject cell)
    {
        if (palette.Length == 0) return;
        var rend = cell.GetComponentInChildren<Renderer>();
        if (rend == null) return;
        var mat   = new Material(rend.sharedMaterial);
        mat.color = palette[Random.Range(0, palette.Length)];
        rend.sharedMaterial = mat;
    }

    // ── Cell accessors ────────────────────────────────────────────────────────

    public GameObject GetHexCell(Hex hex) =>
        _hexCells.TryGetValue(hex, out var c) ? c : null;

    public GameObject GetSquareCell(Vector2Int coord) =>
        _squareCells.TryGetValue(coord, out var c) ? c : null;

    public GameObject GetOctagonCell(Vector2Int coord) =>
        _octagonCells.TryGetValue(coord, out var c) ? c : null;
}
