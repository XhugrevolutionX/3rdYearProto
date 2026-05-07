using Rodak.Hexagons.HexGeometry;
using Rodak.Hexagons.HexUtils;
using UnityEngine;

namespace Rodak.Hexagons.HexGeometry3D
{
    /// <summary>
    /// Extends the hexagon class with many 3D geomtery related functions.
    /// </summary>
    public static class HexagonGeometry3DExtensions
    {
        /// <summary>
        /// Default PlacementPlane used when none specified.
        /// </summary>
        public static PlacementPlane DefaultPlacementPlane = PlacementPlane.XZPlane;

        public static PlacementPlane Fallback(PlacementPlane placementPlane) => placementPlane ?? DefaultPlacementPlane;

        /// <summary>
        /// Calculates a hexagon's center on a 3D plane.
        /// </summary>
        /// <returns>Center position</returns>
        public static Vector3 GetCenter3D(this Hexagon hexagon, PlacementPlane placementPlane = null)
        {
            return Fallback(placementPlane).LayOnPlane(hexagon.GetCenter());
        }

        /// <summary>
        /// Calculates a hexagon's corner on a 3D plane.
        /// </summary>
        /// <returns>Corner position</returns>
        public static Vector3 GetCorner3D(this Hexagon hexagon, int index, PlacementPlane placementPlane = null)
        {
            return Fallback(placementPlane).LayOnPlane(hexagon.GetCorner(index));
        }

        /// <summary>
        /// Calculates a hexagon's side on a 3D plane.
        /// </summary>
        /// <returns>Side center position</returns>
        public static Vector3 GetSide3D(this Hexagon hexagon, int index, PlacementPlane placementPlane = null)
        {
            return Fallback(placementPlane).LayOnPlane(hexagon.GetSide(index));
        }

        /// <summary>
        /// Calculates a hexagon from a 3D point.
        /// </summary>
        /// <returns>Hexagon containing this point</returns>
        public static Hexagon GetHexagonAt(Vector3 point, PlacementPlane placementPlane = null)
        {
            Vector2 planePosition = Fallback(placementPlane).Get2DPosition(point);
            return HexagonGeometryExtensions.GetHexagonAt(planePosition.x, planePosition.y);
        }
    }
}