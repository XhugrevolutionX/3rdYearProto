using System;
using Rodak.Hexagons.HexUtils;

namespace Rodak.Hexagons.Demo
{
    [Serializable]
    public enum DemoPlacementPlane
    {
        XY,
        XZ,
        YZ,
    }

    public static class DemoPlacementPlanes
    {
        public static PlacementPlane GetPlacementPlane(DemoPlacementPlane plane)
        {
            return plane switch
            {
                DemoPlacementPlane.XY => PlacementPlane.XYPlane,
                DemoPlacementPlane.XZ => PlacementPlane.XZPlane,
                DemoPlacementPlane.YZ => PlacementPlane.YZPlane,
                _ => throw new ArgumentException($"Plane {plane} is unknown"),
            };
        }
    }
}
