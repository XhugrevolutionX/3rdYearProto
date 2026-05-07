using Rodak.Hexagons.HexDebug;
using Rodak.Hexagons.HexGrid;
using UnityEngine;
using Rodak.Hexagons.HexUtils;

namespace Rodak.Hexagons.Demo.Grid
{
    /// <summary>
    /// This demo shows how you can generate a grid. And how to place any hexagon on a 3D plane.
    /// </summary>
    public class HexagonGridDemo : MonoBehaviour
    {
        [SerializeField, Min(1)] private int size = 6;
        [SerializeField] private Vector3 planeUp = Vector3.up;

        private void Update()
        {
            PlacementPlane plane = new(planeUp);

            HexagonGrid<int> hexagonGrid = new(size, CreateHexagonGridValue);
            hexagonGrid.DebugDraw(plane, Color.red);
        }

        private int CreateHexagonGridValue(Hexagon position)
        {
            return position.q + position.r * size;
        }
    }
}
