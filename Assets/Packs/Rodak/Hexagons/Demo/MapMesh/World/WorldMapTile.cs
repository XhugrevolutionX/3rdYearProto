using System;

namespace Rodak.Hexagons.Demo.MapMesh.World
{
    /// <summary>
    /// Simple world map tile definition.
    /// Stores the tile value.
    /// Calculates height based on its value and step height;
    /// </summary>
    public class WorldMapTile
    {
        public int Value { get; private set; }

        public WorldMapTile(int value)
        {
            if (value < 0)
                throw new ArgumentException($"{nameof(value)} has to be above 0");
            Value = value;
        }

        public float GetHeight(float stepHeight) => (Value + 1) * stepHeight;
    }
}