using System.Collections.Generic;
using Rodak.Hexagons.HexDebug;
using Rodak.Hexagons.HexEditor;
using Rodak.Hexagons.HexNavigation;
using UnityEngine;
using Rodak.Hexagons.HexUtils;

namespace Rodak.Hexagons.Demo.Relations
{
    /// <summary>
    /// This demo shows what relations can you get around a hexagon.
    /// </summary>
    public class HexagonRelationsDemo : MonoBehaviour
    {
        [SerializeField] private EditableHexagon centerHexagon;
        [SerializeField] private DemoPlacementPlane placementPlane;

        [Header("Ring")]
        [SerializeField] private int ringRadius = 6;
        [SerializeField] private int rangeRadius = 4;

        [Header("Colors")]
        [SerializeField] private Color centerColor = Color.white;
        [SerializeField] private Gradient neighborGradient;
        [SerializeField] private Gradient diagonalGradient;
        [SerializeField] private Gradient ringGradient;
        [SerializeField] private Gradient rangeGradient;

        private void Update()
        {
            PlacementPlane plane = DemoPlacementPlanes.GetPlacementPlane(placementPlane);
            Hexagon center = centerHexagon;

            List<Hexagon> range = center.GetRange(rangeRadius);
            DebugDrawList(range, rangeGradient, plane);

            List<Hexagon> ring = center.GetRing(ringRadius);
            DebugDrawList(ring, ringGradient, plane);

            List<Hexagon> diagonals = center.GetDiagonals();
            DebugDrawList(diagonals, diagonalGradient, plane);

            List<Hexagon> neighbors = center.GetNeighbors();
            DebugDrawList(neighbors, neighborGradient, plane);

            center.DebugDraw(plane, centerColor, 0, true);
        }

        private void DebugDrawList(List<Hexagon> hexagons, Gradient gradient, PlacementPlane plane)
        {
            for (int i = 0; i < hexagons.Count; i++)
            {
                Hexagon hexagon = hexagons[i];
                float t = (float)i / hexagons.Count;
                hexagon.DebugDraw(plane, gradient.Evaluate(t), 0, true);
            }
        }
    }
}
