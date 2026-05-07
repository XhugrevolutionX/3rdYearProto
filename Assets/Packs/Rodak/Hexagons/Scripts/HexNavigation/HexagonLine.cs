using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;

namespace Rodak.Hexagons.HexNavigation
{
    /// <summary>
    /// Defines a line from a start hexagon to an end hexagon.
    /// </summary>
    public class HexagonLine
    {
        private readonly ReadOnlyArray<Hexagon> hexagons;

        /// <summary>
        /// First hexagon.
        /// </summary>
        public Hexagon Start => hexagons[0];

        /// <summary>
        /// Last hexagon.
        /// </summary>
        public Hexagon End => hexagons[^1];

        /// <summary>
        /// Total count of hexagons.
        /// </summary>
        public int Length => hexagons.Count;

        /// <summary>
        /// Gets a specific hexagon at the index.
        /// </summary>
        /// <returns>Hexagon on line.</returns>
        /// <exception cref="IndexOutOfRangeException">When the index is not on this line.</exception>
        public Hexagon this[int index]
        {
            get
            {
                if (index < 0 || index >= Length)
                    throw new IndexOutOfRangeException($"{nameof(index)}: '{index}' must be betwen {0} and {Length}.");

                return hexagons[index];
            }
        }

        /// <summary>
        /// New hexagon line. 
        /// </summary>
        public HexagonLine(Hexagon start, Hexagon end)
        {
            int distance = Mathf.CeilToInt(Hexagon.Distance(start, end));

            List<Hexagon> hexagonList = new();
            for (int i = 0; i <= distance; i++)
            {
                float t = (float)i / distance;
                Hexagon hexagon = Hexagon.Lerp(start, end, t);
                hexagonList.Add(hexagon);
            }

            hexagons = new(hexagonList.ToArray());
        }

        public override string ToString()
        {
            return $"HexLine[length: {Length}, start: {Start}, end: {End}]";
        }
    }
}