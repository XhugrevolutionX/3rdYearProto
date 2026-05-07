using UnityEngine;

namespace Rodak.Hexagons.HexUtils
{
    /// <summary>
    /// Used to place a 2D hexagon to a 3D world.
    /// </summary>
    public class PlacementPlane
    {
        /// <summary>
        /// A static instance representing the XY-plane (normal is Z-axis).
        /// </summary>
        public static readonly PlacementPlane XYPlane = new(0, 0, 1);
        /// <summary>
        /// A static instance representing the XZ-plane (normal is Y-axis).
        /// </summary>
        public static readonly PlacementPlane XZPlane = new(0, 1, 0);
        /// <summary>
        /// A static instance representing the YZ-plane (normal is X-axis).
        /// </summary>
        public static readonly PlacementPlane YZPlane = new(1, 0, 0);

        /// <summary>
        /// The underlying 3D plane definition.
        /// </summary>
        public readonly Plane Plane;
        /// <summary>
        /// Gets the normal vector of the plane.
        /// </summary>
        public Vector3 Normal => Plane.normal;

        private readonly Vector3 planeRight;
        private readonly Vector3 planeForward;

        /// <summary>
        /// Creates a new PlacementPlane using the components of the normal vector.
        /// The plane passes through the world origin (0, 0, 0).
        /// </summary>
        public PlacementPlane(float upX, float upY, float upZ) : this(new Vector3(upX, upY, upZ))
        { }

        /// <summary>
        /// Creates a new PlacementPlane using the normal vector.
        /// The plane passes through the world origin (0, 0, 0).
        /// </summary>
        public PlacementPlane(Vector3 up) : this(new Plane(up.normalized, Vector3.zero))
        { }

        /// <summary>
        /// Creates a new PlacementPlane from an existing Unity Plane object.
        /// </summary>
        public PlacementPlane(Plane plane)
        {
            Plane = plane;

            bool isVerticalNormal = Mathf.Abs(Vector3.Dot(Plane.normal, Vector3.up)) > 0.99f;

            Vector3 arbitraryAxis = isVerticalNormal ? Vector3.forward : Vector3.up;

            planeRight = Vector3.Cross(Plane.normal, arbitraryAxis).normalized;
            planeForward = Vector3.Cross(planeRight, Plane.normal).normalized;
        }

        /// <summary>
        /// Places the 2D vector to the 3D plane equivalent by scaling the plane's 'right' and 'forward' vectors.
        /// </summary>
        /// <returns>The 3D vector on the plane, relative to the origin.</returns>
        public Vector3 LayOnPlane(Vector2 vector)
        {
            return (vector.x * planeRight) + (vector.y * planeForward);
        }

        /// <summary>
        /// Gets the 2D vector equivalent of a 3D vector that lies on the plane.
        /// </summary>
        /// <returns>The 2D plane vector, where X is the projection onto the plane's right axis and Y is the projection onto its forward axis.</returns>
        public Vector2 Get2DPosition(Vector3 vector)
        {
            // 2D x component is the projection onto planeRight
            float x = Vector3.Dot(vector, planeRight);
            // 2D y component is the projection onto planeForward
            float y = Vector3.Dot(vector, planeForward);

            return new Vector2(x, y);
        }
    }
}