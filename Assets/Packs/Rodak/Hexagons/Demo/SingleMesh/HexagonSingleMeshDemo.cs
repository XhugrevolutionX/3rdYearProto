using Rodak.Hexagons.HexGeometry3D;
using Rodak.Hexagons.HexUtils;
using UnityEngine;

namespace Rodak.Hexagons.Demo.SingleMesh
{
    /// <summary>
    /// This demo shows how to generate a mesh for a single hexagon. 
    /// </summary>
    public class HexagonSingleMeshDemo : MonoBehaviour
    {
        [SerializeField] private DemoPlacementPlane placementPlane = DemoPlacementPlane.XZ;
        [SerializeField, Min(0.001f)] private float height = 1;

        [Header("Meshes")]
        [SerializeField] private MeshFilter topCapMesh;
        [SerializeField] private MeshFilter prismWallsMesh;
        [SerializeField] private MeshFilter bottomCapMesh;

        private void Awake()
        {
            PlacementPlane plane = DemoPlacementPlanes.GetPlacementPlane(placementPlane);
            Hexagon hexagon = Hexagon.Zero;

            MeshBuilder topCapMeshBuilder = hexagon.GetMesh(flipped: true, yOffset: 0, plane);
            topCapMesh.mesh = topCapMeshBuilder.ToMesh();

            MeshBuilder prismWallsMeshBuilder = hexagon.GetPrismWallsMesh(height, null, plane);
            prismWallsMesh.mesh = prismWallsMeshBuilder.ToMesh();

            MeshBuilder bottomCapMeshBuilder = hexagon.GetMesh(flipped: false, yOffset: height, plane);
            bottomCapMesh.mesh = bottomCapMeshBuilder.ToMesh();
        }
    }
}
