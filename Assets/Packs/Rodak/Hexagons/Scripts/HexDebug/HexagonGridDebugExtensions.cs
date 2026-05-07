using Rodak.Hexagons.HexGrid;
using UnityEngine;
using Rodak.Hexagons.HexUtils;

namespace Rodak.Hexagons.HexDebug
{
    /// <summary>
    /// Provides utility methods for drawing visual representations of HexagonGrid contents in 3D for debugging.
    /// </summary>
    public static class HexagonGridDebugExtensions
    {

        /// <summary>
        /// Draws an outline of each hexagon in the grid. 
        /// </summary>
        /// <typeparam name="TTile">Grid value</typeparam>
        public static void DebugDraw<TTile>(this HexagonGrid<TTile> hexagonGrid, PlacementPlane placementPlane, Color color, float duration = 0.0f, bool drawTriangles = false, bool depthTest = true)
        {
            hexagonGrid.ForEach((Hexagon hexagon, TTile value) =>
            {
                hexagon.DebugDraw(placementPlane, color, duration, drawTriangles, depthTest);
            });
        }
    }

}