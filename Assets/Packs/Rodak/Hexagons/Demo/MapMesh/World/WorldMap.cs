using Rodak.Hexagons.HexMap;
using UnityEngine;

namespace Rodak.Hexagons.Demo.MapMesh.World
{
    /// <summary>
    /// Simple world map definition.
    /// </summary>
    public class WorldMap : AHexagonMap<WorldMapTile, WorldMapChunk>
    {
        private readonly float noiseScale;
        private readonly Vector2 seedOffset;
        private readonly int maxValue;

        public WorldMap(int chunkSize, float noiseScale, Vector2 seedOffset, int maxValue) : base(chunkSize)
        {
            this.noiseScale = noiseScale;
            this.seedOffset = seedOffset;
            this.maxValue = maxValue;
        }

        private WorldMapTile CreateHexagonValue(Hexagon hexagonPosition, Hexagon chunkPosition)
        {
            Vector2 noisePosition = new Vector2(hexagonPosition.q, hexagonPosition.r) * noiseScale;
            float noiseValue = Mathf.Clamp01(Mathf.PerlinNoise(noisePosition.x + seedOffset.y, noisePosition.y + seedOffset.x));

            int value = Mathf.FloorToInt(noiseValue * maxValue);
            if (value == maxValue) value = maxValue - 1;

            return new(value);
        }

        protected override WorldMapChunk CreateChunk(Hexagon chunkPosition)
        {
            return new(chunkPosition, ChunkSize, CreateHexagonValue);
        }
    }
}