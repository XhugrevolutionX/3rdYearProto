using System;
using System.Collections.Generic;
using System.Linq;

namespace Rodak.Hexagons.HexGrid
{
    /// <summary>
    /// Defines a grid of hexagons that stores a value of type T for each hexagon.
    /// </summary>
    /// <typeparam name="TTile">The type of value stored in each hexagon cell.</typeparam>
    public class HexagonGrid<TTile>
    {
        private readonly Dictionary<Hexagon, TTile> values = new();

        /// <summary>
        /// Gets a list of all <see cref="Hexagon"/> positions currently in the grid.
        /// </summary>
        public List<Hexagon> Hexagons => values.Keys.ToList();

        /// <summary>
        /// Gets the radial size of the grid, which is the maximum coordinate magnitude
        /// from the center (0, 0). A size of 0 is one hexagon; a size of 1 is 7 hexagons.
        /// </summary>
        public int Size { get; private set; }

        /// <summary>
        /// Gets the number of hexagons across the longest dimension (center to opposite edge).
        /// This is calculated as (Size * 2) + 1.
        /// </summary>
        public int SizeAcross => Size * 2 + 1;

        /// <summary>
        /// Gets the value associated with a specific <see cref="Hexagon"/> position.
        /// </summary>
        /// <returns>The value.</returns>
        /// <exception cref="IndexOutOfRangeException">When the <see cref="Hexagon"/> position is not in this grid.</exception>
        public TTile this[Hexagon hexagonPosition]
        {
            get
            {
                if (TryGetValue(hexagonPosition, out TTile value))
                    return value;
                throw new IndexOutOfRangeException($"{hexagonPosition} is not in this grid");
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HexagonGrid{T}"/> class.
        /// The grid is generated as a rhombus of a given size centered at (0, 0).
        /// </summary>
        /// <exception cref="ArgumentException">Thrown if <paramref name="size"/> is negative.</exception>
        public HexagonGrid(int size, Func<Hexagon, TTile> createValue)
        {
            if (size < 0) throw new ArgumentException($"{nameof(size)} of a {nameof(HexagonGrid<TTile>)} must be positive");
            Size = size;

            for (int i = -Size; i <= Size; i++)
            {
                for (int j = -Size; j <= Size; j++)
                {
                    // This condition ensures the generated shape is a rhombus centered at (0,0,0)
                    // The implicit k coordinate is -(i + j). The condition checks if |k| <= Size.
                    if (i + j > Size || i + j < -Size) continue;
                    Hexagon position = new(i, j);
                    values.Add(position, createValue(position));
                }
            }
        }

        /// <summary>
        /// Executes an action for every hexagon and its associated value in the grid.
        /// </summary>
        public void ForEach(Action<Hexagon, TTile> action)
        {
            Hexagons.ForEach((Hexagon hexagonPosition) =>
            {
                action(hexagonPosition, values[hexagonPosition]);
            });
        }

        /// <summary>
        /// Checks if a specific <see cref="Hexagon"/> position is contained within the grid bounds.
        /// </summary>
        /// <returns><c>true</c> if the position is in the grid; otherwise, <c>false</c>.</returns>
        public bool ContainsHexagon(Hexagon hexagonPosition)
        {
            return values.ContainsKey(hexagonPosition);
        }

        /// <summary>
        /// Attempts to retrieve the value associated with a specific <see cref="Hexagon"/> position.
        /// </summary>
        /// <returns><c>true</c> if the grid contains the position; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(Hexagon hexagonPosition, out TTile value)
        {
            return values.TryGetValue(hexagonPosition, out value);
        }

        /// <summary>
        /// Attempts to set a new value for a specific <see cref="Hexagon"/> position.
        /// </summary>
        /// <returns><c>true</c> if the position exists and was updated; otherwise, <c>false</c>.</returns>
        public bool TrySetValue(Hexagon hexagonPosition, TTile value)
        {
            if (!values.ContainsKey(hexagonPosition))
                return false;
            values[hexagonPosition] = value;
            return true;
        }

        public override string ToString()
        {
            return $"HexGrid[{Size}, {typeof(TTile)}]";
        }
    }
}