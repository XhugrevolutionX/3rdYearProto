using System;
using System.Collections.Generic;
using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexGrid;
using Rodak.Hexagons.HexUtils;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Rodak.Hexagons.Demo.GridMesh
{
    /// <summary>
    /// This demo uses the HexaonGrid to generate a map of varying heights.
    /// It stores the height value in the grid and uses that value to determine the height and color.
    /// </summary>
    public class HexagonGridMeshDemo : MonoBehaviour
    {
        [SerializeField, Min(1)] private int size = 4;

        [Header("World Generation")]
        [SerializeField] private float stepHeight = 1;

        [Header("Mesh Components")]
        [SerializeField] private MeshFilter meshFilter;
        [SerializeField] private MeshRenderer meshRenderer;

        private HexagonGrid<int> hexagonGrid;

        private int colorCount;

        private void Awake()
        {
            colorCount = meshRenderer.materials.Length;
            if (colorCount == 0)
                throw new ArgumentException($"{nameof(colorCount)} should be above 0");

            hexagonGrid = new(size, (position) => Random.Range(1, colorCount + 1));
            meshFilter.mesh = GenerateMesh();
        }

        private Mesh GenerateMesh()
        {
            PlacementPlane placementPlane = PlacementPlane.XZPlane;
            MeshBuilder meshBuilder = new();

            hexagonGrid.ForEach((Hexagon hexagon, int value) =>
            {
                int submeshIndex = value - 1;
                float height = value * stepHeight;

                MeshBuilder topCap = hexagon.GetMesh(flipped: false, height, placementPlane);
                meshBuilder.Append(topCap, submeshIndex);

                List<Hexagon> excludedDirections = new(); // all walls included
                MeshBuilder walls = hexagon.GetPrismWallsMesh(height, excludedDirections, placementPlane);
                meshBuilder.Append(walls, submeshIndex);
            });

            Mesh mesh = meshBuilder.ToMesh();
            mesh.name = "Grid Mesh Demo";
            return mesh;
        }
    }
}