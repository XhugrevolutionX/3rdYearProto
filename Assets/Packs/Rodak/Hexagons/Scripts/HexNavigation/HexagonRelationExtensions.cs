
using System.Collections.Generic;
using Rodak.Hexagons.HexGeometry;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Rodak.Hexagons.HexNavigation
{
    /// <summary>
    ///  Extends the hexagon class with many relation related functions.
    /// </summary>
    public static class HexagonRelationExtensions
    {
        /// <summary>
        /// Neighboring hexagons to the origin hexagon.
        /// </summary>
        public static readonly ReadOnlyArray<Hexagon> Neighbours = new Hexagon[] {
            new(1, 0, -1),
            new(0, 1, -1),
            new(-1, 1, 0),
            new(-1, 0, 1),
            new(0, -1, 1),
            new(1, -1, 0)
        };

        /// <summary>
        /// Diagonal hexagons to the origin hexagon.
        /// </summary>
        public static readonly ReadOnlyArray<Hexagon> Diagonals = new Hexagon[] {
            new(1, 1, -2),
            new(-1, 2, -1),
            new(-2, 1, 1),
            new(-1, -1, 2),
            new(1, -2, 1),
            new(2, -1, -1)
        };

        /// <summary>
        /// Calculates the neighbors to the hexagon.
        /// </summary>
        /// <returns>All neighbors.</returns>
        public static List<Hexagon> GetNeighbors(this Hexagon hexagon)
        {
            List<Hexagon> neighbors = new();
            foreach (Hexagon direction in Neighbours)
                neighbors.Add(hexagon + direction);
            return neighbors;
        }

        /// <summary>
        /// Calculates the diagonals to the hexagon.
        /// </summary>
        /// <returns>All diagonals.</returns>
        public static List<Hexagon> GetDiagonals(this Hexagon hexagon)
        {
            List<Hexagon> neighbors = new();
            foreach (Hexagon direction in Diagonals)
                neighbors.Add(hexagon + direction);
            return neighbors;
        }

        /// <summary>
        /// Calculates the ring at a specified radius.
        /// </summary>
        /// <returns>All hexagons on that ring.</returns>
        public static List<Hexagon> GetRing(this Hexagon hexagon, int radius)
        {
            if (radius <= 0)
                return new() { hexagon };

            Hexagon start = hexagon + HexagonGeometryExtensions.DirectionsClockwise[4] * radius;

            List<Hexagon> ring = new();
            Hexagon position = start;
            for (int i = 0; i < HexagonGeometryExtensions.DirectionsClockwise.Count; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    ring.Add(position);
                    position += HexagonGeometryExtensions.DirectionsClockwise[i];
                }
            }

            return ring;
        }

        /// <summary>
        /// Calculates the spiral up to the specified radius.
        /// </summary>
        /// <returns>All hexagons on that spiral.</returns>
        public static List<Hexagon> GetSpiral(this Hexagon hexagon, int radius)
        {
            if (radius <= 0)
                return new() { hexagon };

            List<Hexagon> spiral = new();
            for (int r = 1; r <= radius; r++)
            {
                spiral.AddRange(hexagon.GetRing(r));
            }

            return spiral;
        }

        /// <summary>
        /// Calculates movement range of n steps.
        /// </summary>
        /// <returns>All hexagons that are n steps away.</returns>
        public static List<Hexagon> GetRange(this Hexagon hexagon, int n)
        {
            List<Hexagon> range = new();
            for (int q = -n; q <= n; q++)
            {
                for (int r = Mathf.Max(-n, -q - n); r <= Mathf.Min(n, -q + n); r++)
                {
                    Hexagon position = new Hexagon(q, r) + hexagon;
                    range.Add(position);
                }
            }
            return range;
        }
    }
}