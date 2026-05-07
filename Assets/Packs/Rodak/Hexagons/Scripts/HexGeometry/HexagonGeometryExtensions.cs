using System;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Rodak.Hexagons.HexGeometry
{
    /// <summary>
    /// Extends the hexagon class with many 2D geomtery related functions.
    /// </summary>
    public static class HexagonGeometryExtensions
    {
        public static readonly ReadOnlyArray<Hexagon> DirectionsClockwise = new Hexagon[] {
            new(1, 0, -1),
            new(0, 1, -1),
            new(-1, 1, 0),
            new(-1, 0, 1),
            new(0, -1, 1),
            new(1, -1, 0)
        };

        private static readonly float SqrtOfThree = (float)Math.Sqrt(3);

        /// <summary>
        /// The side width of a hexagon.
        /// </summary>
        public static readonly float SizeSideToSide = SqrtOfThree;
        /// <summary>
        /// The vertex width of a hexagon.
        /// </summary>
        public static readonly float SizeCornerToCorner = 2;

        /// <summary>
        /// Calculates a hexagon's center on a 2D plane.
        /// </summary>
        /// <returns>Center position</returns>
        public static Vector2 GetCenter(this Hexagon hexagon)
        {
            return new Vector2(
                SqrtOfThree * hexagon.q + SqrtOfThree / 2f * hexagon.r,
                3f / 2f * hexagon.r
            );
        }

        /// <summary>
        /// Calculates a hexagon from a 2D point.
        /// </summary>
        /// <returns>Hexagon containing this point</returns>
        public static Hexagon GetHexagonAt(float x, float y)
        {
            float qFloat = SqrtOfThree / 3f * x - 1f / 3f * y;
            float rFloat = 2f / 3f * y;
            return Hexagon.GetNearestHexagonRound(qFloat, rFloat);
        }

        /// <summary>
        /// Contains the vertex index from 0 to 6. 
        /// </summary>
        /// <returns>Vertex index</returns>
        public static int GetAbsoluteIndex(int index)
        {
            return ((index % 6) + 6) % 6;
        }

        /// <summary>
        /// Wheter the distance between these directions is of the offset.
        /// </summary>
        /// <returns>True if indexA + offset = indexB</returns>
        public static bool AreDirectionsOffseted(int indexA, int indexB, int offset)
        {
            int offsetIndex = GetAbsoluteIndex(indexA + offset);
            indexB = GetAbsoluteIndex(indexB);
            return offsetIndex == indexB;
        }

        /// <summary>
        /// Calculates the angle of a hexagon corner.
        /// </summary>
        /// <returns>Angle in radians</returns>
        public static float GetCornerAngle(int index)
        {
            index = GetAbsoluteIndex(index);

            float angleDegrees = 60 * index - 30;
            return Mathf.Deg2Rad * angleDegrees;
        }

        /// <summary>
        /// Calculates the direction of the corner form the center.
        /// </summary>
        /// <returns>Angle in radians</returns>
        public static Vector2 GetCornerDirection(int index)
        {
            float angle = GetCornerAngle(index);

            return new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );
        }

        /// <summary>
        /// Calculates a hexagon's corner on a 2D plane.
        /// </summary>
        /// <returns>Corner position</returns>
        public static Vector2 GetCorner(this Hexagon hexagon, int index)
        {
            Vector2 center = hexagon.GetCenter();
            Vector2 cornerDirection = GetCornerDirection(index);
            return center + cornerDirection;
        }

        /// <summary>
        /// Calculates a hexagon's side direction on a 2D plane.
        /// </summary>
        /// <returns>Direction vector</returns>
        public static Vector2 GetSideDirection(int index)
        {
            float angle = GetCornerAngle(index);

            return new Vector2(
                Mathf.Cos(angle),
                Mathf.Sin(angle)
            );
        }
        /// <summary>
        /// Calculates a hexagon's side on a 2D plane.
        /// </summary>
        /// <returns>Side center position</returns>
        public static Vector2 GetSide(this Hexagon hexagon, int index)
        {
            index = GetAbsoluteIndex(index);

            Vector2 center = hexagon.GetCenter();
            Hexagon direction = DirectionsClockwise[index];
            return center + direction.GetCenter() / 2f;
        }

        /// <summary>
        /// Calculates the direction angle of the hexagon from the (0, 0) center.
        /// </summary>
        /// <returns>Angle in degrees</returns>
        public static float GetDirectionAngle(this Hexagon hexagon)
        {
            Vector2 directionVector = hexagon.GetCenter().normalized;
            return 360f - (Mathf.Atan2(directionVector.y, directionVector.x) * Mathf.Rad2Deg + 360f) % 360f;
        }

        /// <summary>
        /// Calculates a clockwise rotated hexagon.
        /// </summary>
        /// <returns>Clockwise rotated hexagon</returns>
        public static Hexagon RotateClockwise(this Hexagon hexagon) => new(-hexagon.r, -hexagon.s, -hexagon.q);

        /// <summary>
        /// Calculates a clockwise rotated hexagon N times.
        /// </summary>
        /// <returns>N times clockwise rotated hexagon</returns>
        public static Hexagon RotateClockwise(this Hexagon hexagon, int rotations)
        {
            rotations %= 6;
            for (int n = 0; n < rotations; n++)
                hexagon = hexagon.RotateClockwise();
            return hexagon;
        }

    }
}