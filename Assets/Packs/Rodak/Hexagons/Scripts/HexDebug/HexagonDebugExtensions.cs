using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexUtils;
using UnityEngine;

namespace Rodak.Hexagons.HexDebug
{
    /// <summary>
    /// Provides utility methods for drawing visual representations of Hexagons in 3D for debugging purposes.
    /// </summary>
    public static class HexagonDebugExtensions
    {
        /// <summary>
        /// Draws an outline of the hexagon.
        /// </summary>
        public static void DebugDraw(this Hexagon hexagon, PlacementPlane placementPlane, Color color, float duration = 0.0f, bool drawTriangles = false, bool depthTest = true)
        {
            void DrawLine(Vector3 start, Vector3 end) => Debug.DrawLine(start, end, color, duration, depthTest);

            Vector3 center = hexagon.GetCenter3D(placementPlane);
            for (int i = 0; i < 6; i++)
            {
                Vector3 a = hexagon.GetCorner3D(i + 0, placementPlane);
                Vector3 b = hexagon.GetCorner3D(i + 1, placementPlane);
                DrawLine(a, b);

                if (drawTriangles) DrawLine(a, center);
            }
        }
    }

}