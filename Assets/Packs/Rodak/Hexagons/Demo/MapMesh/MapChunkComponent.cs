using System.Collections.Generic;
using Rodak.Hexagons.Demo.MapMesh.World;
using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexNavigation;
using Rodak.Hexagons.HexUtils;
using UnityEngine;

namespace Rodak.Hexagons.Demo.MapMesh
{
    /// <summary>
    /// Manages a map chunk.
    /// Generates its mesh and removes it when destroyed.
    /// </summary>
    public class MapChunkComponent : MonoBehaviour
    {
        private WorldMap map;
        private WorldMapChunk chunk;

        private PlacementPlane placementPlane;
        private float stepHeight;
        private Material[] meshMaterials;

        private MeshFilter meshFilter;
        private MeshCollider meshCollider;

        public WorldMapChunk Chunk => chunk;

        private void OnDestroy()
        {
            map.TryRemoveChunk(chunk.Position);
        }

        private Mesh GenerateMesh(PlacementPlane placementPlane, Material[] meshMaterials, float stepHeight)
        {
            MeshBuilder meshBuilder = new();

            meshBuilder.EnsureSubmesh(meshMaterials.Length);

            MeshBuilder precomuptedTopCap = Hexagon.Zero.GetMesh(flipped: false, yOffset: 0, placementPlane);

            chunk.ForEach((Hexagon hexagonPosition, WorldMapTile tile) =>
            {
                int submeshIndex = tile.Value;
                float height = tile.GetHeight(stepHeight);

                MeshBuilder topCap = hexagonPosition.GetMesh(flipped: false, yOffset: height, placementPlane);
                meshBuilder.Append(topCap, submeshIndex);

                List<Hexagon> excludedDirections = new();
                foreach (Hexagon neighborDirection in HexagonRelationExtensions.Neighbours)
                {
                    Hexagon neighborPosition = hexagonPosition + neighborDirection;

                    if (!map.TryGetHexagonsChunk(neighborPosition, out WorldMapChunk otherChunk))
                        continue; // Map edge

                    WorldMapTile neighbor = otherChunk[neighborPosition];

                    if (tile.Value <= neighbor.Value)
                    {
                        // exlude if neighbor height is the same or larger
                        excludedDirections.Add(neighborDirection);
                    }
                }
                MeshBuilder walls = hexagonPosition.GetPrismWallsMesh(height, excludedDirections, placementPlane);
                meshBuilder.Append(walls, submeshIndex);
            });

            Mesh mesh = meshBuilder.ToMesh();
            mesh.name = $"{gameObject.name} Mesh";
            return mesh;
        }

        public void Init(WorldMap map, WorldMapChunk chunk, Material[] meshMaterials, PlacementPlane placementPlane, float stepHeight, int meshLayer)
        {
            this.chunk = chunk;
            this.map = map;

            this.placementPlane = placementPlane;
            this.stepHeight = stepHeight;
            this.meshMaterials = meshMaterials;

            gameObject.name = chunk.ToString();
            gameObject.layer = meshLayer;

            meshFilter = gameObject.AddComponent<MeshFilter>();
            meshCollider = gameObject.AddComponent<MeshCollider>();
            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();

            meshRenderer.materials = meshMaterials;
        }

        public void GenerateMesh()
        {
            Mesh mesh = GenerateMesh(placementPlane, meshMaterials, stepHeight);
            meshCollider.sharedMesh = mesh;
            meshFilter.mesh = mesh;
        }

        public void DestroySelf()
        {
            Destroy(gameObject);
        }
    }
}