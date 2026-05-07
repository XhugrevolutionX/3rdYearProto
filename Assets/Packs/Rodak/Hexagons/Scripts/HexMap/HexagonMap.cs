using System;

namespace Rodak.Hexagons.HexMap
{
    /// <summary>
    /// Represents a map of hexagon tiles that is automatically organized into chunks.
    /// It inherits basic map functionality from AHexagonMap.
    /// </summary>
    /// <typeparam name="TTile">The type of value stored in each hexagon tile.</typeparam>
    public class HexagonMap<TTile> : AHexagonMap<TTile, HexagonChunk<TTile>>
    {
        private Func<Hexagon, Hexagon, TTile> createValue;

        /// <summary>
        /// Initializes a new instance of the HexagonMap class.
        /// </summary>
        public HexagonMap(
            int chunkSize,
            Func<Hexagon, Hexagon, TTile> createValue
            ) : base(chunkSize)
        {
            this.createValue = createValue;
        }

        /// <summary>
        /// Creates a new HexagonChunk instance at a specified position using the stored creation function.
        /// </summary>
        /// <returns>A new HexagonChunk object.</returns>
        protected override HexagonChunk<TTile> CreateChunk(Hexagon chunkPosition)
        {
            return new(chunkPosition, ChunkSize, createValue);
        }
    }
}