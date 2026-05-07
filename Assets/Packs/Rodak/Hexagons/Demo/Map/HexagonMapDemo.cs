using Rodak.Hexagons.HexDebug;
using Rodak.Hexagons.HexMap;
using UnityEngine;
using Rodak.Hexagons.HexUtils;

namespace Rodak.Hexagons.Demo.Map
{
    /// <summary>
    /// This demo shows how you can generate a map.
    /// </summary>
    public class HexagonMapDemo : MonoBehaviour
    {
        [SerializeField, Min(1)] private int chunkSize = 6;
        [SerializeField, Min(1)] private int initialSize = 6;
        [SerializeField] private DemoPlacementPlane placementPlane = DemoPlacementPlane.XY;

        private HexagonMap<int> hexagonMap;

        private Color[] chunkColors;

        private void Awake()
        {
            hexagonMap = new(chunkSize, CreateHexagonMapValue);

            for (int i = -initialSize; i <= initialSize; i++)
            {
                for (int j = -initialSize; j <= initialSize; j++)
                {
                    if (i + j > initialSize || i + j < -initialSize) continue;
                    Hexagon position = new(i, j);
                    hexagonMap.GetOrCreateChunk(position, out bool wasCreated);
                }
            }

            chunkColors = new Color[]
            {
                Color.red,
                Color.green,
                Color.blue,
                Color.yellow,
                Color.pink,
                Color.cyan,
                Color.white,
            };
        }

        private void Update()
        {
            PlacementPlane plane = DemoPlacementPlanes.GetPlacementPlane(placementPlane);

            int colorIndex = 0;
            hexagonMap.ForEach((hexagonChunk) =>
            {
                hexagonChunk.ForEach((Hexagon hexagon, int value) =>
                {
                    hexagon.DebugDraw(plane, chunkColors[colorIndex], 0);
                });
                colorIndex = (colorIndex + 1) % chunkColors.Length;
            });
        }

        private int CreateHexagonMapValue(Hexagon hexagonPosition, Hexagon chunkPosition)
        {
            return hexagonPosition.q + hexagonPosition.r * initialSize;
        }
    }
}
