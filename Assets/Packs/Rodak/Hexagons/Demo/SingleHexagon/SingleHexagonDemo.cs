using Rodak.Hexagons.HexDebug;
using Rodak.Hexagons.HexEditor;
using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexUtils;
using UnityEngine;

namespace Rodak.Hexagons.Demo.SingleHexagon
{
    /// <summary>
    /// This demo shows the geometry function that can be used to get  various parts of the hexagon.
    /// The placement plane can be anything not just XY.
    /// </summary>
    public class SingleHexagonDemo : MonoBehaviour
    {
        [SerializeField] private EditableHexagon editableHexagon = Hexagon.Zero;
        [SerializeField, HexReadOnly] private EditableHexagon readonlyHexagon = new Hexagon(12, 7);

        [Header("TwoComponent auto correct")]
        [SerializeField, HexAutoCorrect(HexAutoCorrectMode.TwoComponent)] private EditableHexagon twoCompoenentHexagon;

        [Header("Staggered auto correct (default)")]
        [SerializeField, HexAutoCorrect(HexAutoCorrectMode.Staggered)] private EditableHexagon staggeredHexagon;

        [Header("3D Plane")]
        [SerializeField] private DemoPlacementPlane placementPlane = DemoPlacementPlane.XY;

        [Header("Visuals")]
        [SerializeField] private Transform center;
        [SerializeField] private Transform[] vertices;
        [SerializeField] private Transform[] sides;

        private void Update()
        {
            PlacementPlane plane = DemoPlacementPlanes.GetPlacementPlane(placementPlane);
            Hexagon hexagon = editableHexagon;

            hexagon.DebugDraw(plane, Color.red, 0, true);

            Vector3 centerPosition = hexagon.GetCenter3D(plane);
            center.position = centerPosition;

            for (int i = 0; i < 6; i++)
            {
                Vector3 vertexPosition = hexagon.GetCorner3D(i, plane);
                vertices[i].position = vertexPosition;
            }

            for (int i = 0; i < 6; i++)
            {
                Vector3 sidePosition = hexagon.GetSide3D(i, plane);

                Vector3 direction = sidePosition - centerPosition;

                sides[i].SetPositionAndRotation(sidePosition, Quaternion.LookRotation(direction));
            }
        }
    }
}