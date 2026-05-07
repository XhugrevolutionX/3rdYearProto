using UnityEngine;
using Rodak.Hexagons.HexUtils;
using System;
using Random = UnityEngine.Random;
using Rodak.Hexagons.HexGeometry3D;
using System.Collections.Generic;
using Rodak.Hexagons.Demo.MapMesh.World;

namespace Rodak.Hexagons.Demo.MapMesh
{
    /// <summary>
    /// This demo shows how you can generate a procedural map mesh.
    /// The map is updated based on a center hexagon position.
    /// Old chunks are destroyed, while new ones are generated.
    /// That way it feels infinite.
    /// </summary>
    public class HexagonMapMeshDemo : MonoBehaviour
    {

        [Header("World Generation")]
        [SerializeField] private bool randomSeed = true;
        [SerializeField] private int seed = 0;
        [SerializeField] private float noiseScale = 0.02f;

        [Space]
        [SerializeField] private float stepHeight = 0.1f;
        [SerializeField, Min(1)] private int chunkSize = 4;
        [SerializeField, Min(2)] private int visibleChunkArea = 4;

        [Header("Mesh Components")]
        [SerializeField] private Material[] meshMaterials;

        private WorldMap map;

        private PlacementPlane placementPlane;

        private readonly Dictionary<Hexagon, MapChunkComponent> generatedChunks = new();

        public float StepHeight => stepHeight;
        public PlacementPlane PlacementPlane => placementPlane;

        private void Awake()
        {
            if (meshMaterials.Length == 0)
                throw new ArgumentException($"{nameof(meshMaterials)} can't be empty");

            if (randomSeed)
                seed = Mathf.FloorToInt(Random.value * int.MaxValue);

            System.Random rng = new(seed);
            const int MaxOffset = 100_000;
            Vector2 seedOffset = new(
                rng.Next(-MaxOffset, MaxOffset),
                rng.Next(-MaxOffset, MaxOffset)
            );

            placementPlane = PlacementPlane.XZPlane;
            map = new(chunkSize, noiseScale, seedOffset, meshMaterials.Length);

            ShowChunksAround(Hexagon.Zero);
        }

        private MapChunkComponent CreateChunkComponent(WorldMapChunk chunk)
        {
            GameObject chunkComponentGO = new("Chunk");
            chunkComponentGO.transform.parent = transform;

            MapChunkComponent chunkComponent = chunkComponentGO.AddComponent<MapChunkComponent>();
            chunkComponent.Init(map, chunk, meshMaterials, placementPlane, stepHeight, gameObject.layer);

            return chunkComponent;
        }

        public bool TryGetTileAt(Hexagon hexagonPosition, out WorldMapTile mapTile)
        {
            if (!map.TryGetHexagonsChunk(hexagonPosition, out WorldMapChunk chunk))
            {
                mapTile = null;
                return false; // Map edge
            }

            mapTile = chunk[hexagonPosition];
            return true;
        }

        public bool TryGetTilePositionOn(Ray ray, out Hexagon hexagonPosition)
        {
            int layerMask = 1 << gameObject.layer;
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                hexagonPosition = default;
                return false;
            }

            Vector3 hitPosition = hit.point - hit.normal * 0.0001f;
            hexagonPosition = HexagonGeometry3DExtensions.GetHexagonAt(hitPosition, placementPlane);
            return true;
        }

        private void RemoveChunks(HashSet<Hexagon> chunkPositionsToRemove)
        {
            foreach (Hexagon chunkPosition in chunkPositionsToRemove)
            {
                MapChunkComponent chunkComponent = generatedChunks[chunkPosition];
                chunkComponent.DestroySelf();
                generatedChunks.Remove(chunkPosition);
            }

        }

        public void ShowChunksAround(Hexagon hexagonPosition)
        {
            Hexagon centerChunkPosition = map.GetChunkPosition(hexagonPosition);

            List<MapChunkComponent> newChunks = new();
            HashSet<Hexagon> visibleChunksPositions = new();

            for (int q = -visibleChunkArea; q <= visibleChunkArea; q++)
            {
                for (int r = -visibleChunkArea; r <= visibleChunkArea; r++)
                {
                    if (q + r > visibleChunkArea || q + r < -visibleChunkArea) continue;
                    Hexagon chunkPosition = new Hexagon(q, r) + centerChunkPosition;
                    visibleChunksPositions.Add(chunkPosition);

                    WorldMapChunk chunk = map.GetOrCreateChunk(chunkPosition, out bool wasCreated);

                    if (!wasCreated) continue;
                    MapChunkComponent chunkComponent = CreateChunkComponent(chunk);
                    generatedChunks.Add(chunkPosition, chunkComponent);
                    newChunks.Add(chunkComponent);
                }
            }

            HashSet<Hexagon> chunkPositionsToRemove = new();
            foreach (var kvp in generatedChunks)
            {
                if (!visibleChunksPositions.Contains(kvp.Key))
                {
                    chunkPositionsToRemove.Add(kvp.Key);
                }
            }

            RemoveChunks(chunkPositionsToRemove);


            foreach (MapChunkComponent chunkComponent in newChunks)
            {
                chunkComponent.GenerateMesh();
            }

        }
    }
}
