using System;
using Rodak.Hexagons.HexMap;

namespace Rodak.Hexagons.Demo.MapMesh.World
{
    /// <summary>
    /// Simple world map chunk definition.
    /// </summary>
    public class WorldMapChunk : HexagonChunk<WorldMapTile>
    {
        public WorldMapChunk(Hexagon position, int size, Func<Hexagon, Hexagon, WorldMapTile> createValue) : base(position, size, createValue)
        { }
    }
}