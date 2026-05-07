using Rodak.Hexagons.Demo.MapMesh.World;
using Rodak.Hexagons.HexEditor;
using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexNavigation;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Rodak.Hexagons.Demo.MapMesh
{
    /// <summary>
    /// Wanderer finds a target position when the mouse is clicked on the map.
    /// Then iterates over the entire path to get there.
    /// </summary>
    public class Wanderer : MonoBehaviour
    {
        [SerializeField, HexReadOnly] private EditableHexagon currentPosition;
        [SerializeField, HexReadOnly] private EditableHexagon targetPosition;

        [Space]
        [SerializeField] private HexagonMapMeshDemo map;

        [Header("Movement")]
        [SerializeField] private float stepInterval = 0.2f;

        private Camera cam;

        private HexagonLine path;
        private int pathIndex;

        private float stepTimer;

        private void Awake()
        {
            cam = Camera.main;
        }

        private void Update()
        {
            UpdatePath();

            Walk();

            UpdatePosition();
        }

        private void Walk()
        {
            if (path == null)
                return;

            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0)
            {
                stepTimer += stepInterval;
                NextStep();
            }
        }

        private void NextStep()
        {
            if (pathIndex + 1 >= path.Length)
                return; // end of path
            pathIndex++;
            currentPosition = path[pathIndex];

            map.ShowChunksAround(currentPosition);
        }

        private void UpdatePath()
        {
            if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame)
                return;

            Vector2 mousePosition = Mouse.current.position.ReadValue();
            if (mousePosition.x < 0 || mousePosition.x >= Screen.width ||
                mousePosition.y < 0 || mousePosition.y >= Screen.height)
            {
                return;
            }

            Ray ray = cam.ScreenPointToRay(mousePosition);

            if (!map.TryGetTilePositionOn(ray, out Hexagon hexagonPosition))
            {
                return;
            }

            path = new(currentPosition, hexagonPosition);
            pathIndex = 0;
            targetPosition = hexagonPosition;
        }

        private void UpdatePosition()
        {
            Hexagon hexagonPosition = currentPosition;

            if (!map.TryGetTileAt(hexagonPosition, out WorldMapTile mapTile))
            {
                Debug.Log("Outside of map");
                return;
            }

            Vector3 position = hexagonPosition.GetCenter3D(map.PlacementPlane);
            position.y = mapTile.GetHeight(map.StepHeight);

            transform.position = position;
        }

    }
}