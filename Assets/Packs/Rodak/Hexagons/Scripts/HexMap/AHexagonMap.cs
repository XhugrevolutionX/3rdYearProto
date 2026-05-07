using System;
using System.Collections.Generic;
using System.Linq;
using Rodak.Hexagons.HexGeometry;

namespace Rodak.Hexagons.HexMap
{
    /// <summary>
    /// Base class for a map that manages hexagon tiles organized into chunks.
    /// This allows for maps that are potentially infinite or very large.
    /// </summary>
    /// <typeparam name="TTile">The type of value stored in each hexagon tile.</typeparam>
    /// <typeparam name="GChunk">The type of the hexagon chunk, which must inherit from HexagonChunk of type TTile.</typeparam>
    public abstract class AHexagonMap<TTile, GChunk> where GChunk : HexagonChunk<TTile>
    {
        private readonly Dictionary<Hexagon, GChunk> chunks = new();

        /// <summary>
        /// Gets a list of the Hexagon coordinates for all loaded chunks.
        /// </summary>
        public List<Hexagon> ChunkHexagons => chunks.Keys.ToList();

        /// <summary>
        /// The size parameter used to define the dimensions of each chunk.
        /// </summary>
        public readonly int ChunkSize;

        /// <summary>
        /// Gets the chunk associated with a specific <see cref="Hexagon"/> chunk position.
        /// </summary>
        /// <returns>The chunk.</returns>
        /// <exception cref="IndexOutOfRangeException">When the <see cref="Hexagon"/> chunk position is not in this map.</exception>
        public GChunk this[Hexagon chunkPosition]
        {
            get
            {
                if (TryGetChunk(chunkPosition, out GChunk value))
                    return value;
                throw new IndexOutOfRangeException($"{chunkPosition} is not in this map");
            }
        }

        /// <summary>
        /// Initializes a new instance of the AHexagonMap class.
        /// </summary>
        public AHexagonMap(int chunkSize)
        {
            ChunkSize = chunkSize;
        }

        /// <summary>
        /// Creates a new chunk instance at a specified position.
        /// </summary>
        /// <returns>A new chunk object.</returns>
        protected abstract GChunk CreateChunk(Hexagon chunkPosition);

        /// <summary>
        /// Adds a chunk to the map's dictionary.
        /// </summary>
        protected void AddChunk(Hexagon chunkPosition, GChunk chunk)
        {
            chunks.Add(chunkPosition, chunk);
        }

        /// <summary>
        /// Removes a chunk from the map's dictionary.
        /// </summary>
        protected void RemoveChunk(Hexagon chunkPosition)
        {
            chunks.Remove(chunkPosition);
        }

        /// <summary>
        /// Performs an action on every loaded chunk in the map.
        /// </summary>
        public void ForEach(Action<GChunk> action)
        {
            ChunkHexagons.ForEach((Hexagon chunkPosition) =>
            {
                action(chunks[chunkPosition]);
            });
        }

        /// <summary>
        /// Checks if a chunk at the specified position is currently loaded.
        /// </summary>
        /// <returns>True if the chunk is loaded, false otherwise.</returns>
        public bool ContainsChunkPosition(Hexagon chunkPosition)
        {
            return chunks.ContainsKey(chunkPosition);
        }

        /// <summary>
        /// Gets the chunk at the specified position, creating and loading it if it does not exist.
        /// </summary>
        /// <returns>The existing or newly created chunk.</returns>
        public GChunk GetOrCreateChunk(Hexagon chunkPosition, out bool wasCreated)
        {
            if (ContainsChunkPosition(chunkPosition))
            {
                wasCreated = false;
                return chunks[chunkPosition];
            }

            wasCreated = true;
            GChunk chunk = CreateChunk(chunkPosition);
            AddChunk(chunkPosition, chunk);
            return chunk;
        }

        /// <summary>
        /// Attempts to retrieve a loaded chunk at the specified position.
        /// </summary>
        /// <returns>True if the chunk was found, false otherwise.</returns>
        public bool TryGetChunk(Hexagon chunkPosition, out GChunk chunk)
        {
            if (!ContainsChunkPosition(chunkPosition))
            {
                chunk = default;
                return false;
            }

            chunk = chunks[chunkPosition];
            return true;
        }

        /// <summary>
        /// Tries to remove a loaded chunk from the specified position.
        /// </summary>
        /// <returns>True if the chunk was removed, false otherwise.</returns>
        public bool TryRemoveChunk(Hexagon chunkPosition)
        {
            if (!ContainsChunkPosition(chunkPosition))
                return false;

            RemoveChunk(chunkPosition);
            return true;
        }

        /// <summary>
        /// Calculates the Hexagon coordinate of the chunk that contains a given hexagon position.
        /// </summary>
        /// <returns>The Hexagon coordinate of the containing chunk.</returns>
        public Hexagon GetChunkPosition(Hexagon hexagonPosition)
        {
            int sizeAcross = HexagonChunkExtensions.GetSizeAcross(ChunkSize);

            Hexagon nearestChunkPosition = hexagonPosition / sizeAcross;

            List<Hexagon> possibleChunkOffsets = new() {
                Hexagon.Zero,
            };
            possibleChunkOffsets.AddRange(HexagonGeometryExtensions.DirectionsClockwise);

            foreach (Hexagon chunkOffset in possibleChunkOffsets)
            {
                Hexagon chunkPosition = nearestChunkPosition + chunkOffset;
                Hexagon chunkCenter = chunkPosition * sizeAcross;
                for (int i = -ChunkSize; i <= ChunkSize; i++)
                {
                    for (int j = -ChunkSize; j <= ChunkSize; j++)
                    {
                        Hexagon currentHexagonPosition = chunkCenter + new Hexagon(i, j);
                        if (currentHexagonPosition == hexagonPosition)
                            return chunkPosition;
                    }
                }
            }

            return nearestChunkPosition;
        }

        /// <summary>
        /// Attempts to retrieve the chunk that contains the specified hexagon position.
        /// </summary>
        /// <returns>True if the containing chunk was found, false otherwise.</returns>
        public bool TryGetHexagonsChunk(Hexagon hexagonPosition, out GChunk chunk)
        {
            Hexagon chunkPosition = GetChunkPosition(hexagonPosition);
            return TryGetChunk(chunkPosition, out chunk);
        }
    }
}