using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Rodak.Hexagons.HexUtils
{
    /// <summary>
    /// Utility class that helps with mesh creation.
    /// </summary>
    public class MeshBuilder
    {
        /// <summary>
        /// All vertices.
        /// </summary>
        private readonly List<Vector3> vertices = new();

        /// <summary>
        /// All uvs.
        /// </summary>
        private readonly List<Vector2> uv = new();

        /// <summary>
        /// Per-submesh triangle lists.
        /// </summary>
        private readonly List<List<int>> submeshTriangles = new();

        private int vertexOffset = 0;

        /// <summary>
        /// Whether the mesh has no vertices.
        /// </summary>
        public bool IsEmpty => vertices.Count == 0;
        public List<Vector3> Vertices => vertices;
        public List<Vector2> UV => uv;

        /// <summary>
        /// Initializes a new, empty instance.
        /// </summary>
        public MeshBuilder()
        {
            EnsureSubmesh(0); // always at least one submesh
        }

        /// <summary>
        /// Initializes a new copy instance.
        /// </summary>
        public MeshBuilder(MeshBuilder other)
        {
            vertices.AddRange(other.vertices);
            uv.AddRange(other.uv);
            submeshTriangles.AddRange(other.submeshTriangles);
            this.vertexOffset = other.vertexOffset;
        }

        /// <summary>
        /// Initializes a new instance and appends the initial data to submesh 0.
        /// </summary>
        public MeshBuilder(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uv, IEnumerable<int> triangles)
        {
            AppendToSubmesh(0, vertices, uv, triangles);
        }

        /// <summary>
        /// Ensures that the requested submesh index exists.
        /// </summary>
        public void EnsureSubmesh(int submeshIndex)
        {
            if (submeshIndex < 0)
                throw new ArgumentException($"{nameof(submeshIndex)} must be above {0}");

            while (submeshTriangles.Count <= submeshIndex)
                submeshTriangles.Add(new List<int>());
        }

        /// <summary>
        /// Appends new mesh data to submesh 0.
        /// </summary>
        public void Append(IEnumerable<Vector3> vertices, IEnumerable<Vector2> uv, IEnumerable<int> triangles)
        {
            AppendToSubmesh(0, vertices, uv, triangles);
        }

        /// <summary>
        /// Appends new mesh data to the specified submesh.
        /// Triangle indices are automatically offset to account for existing vertices.
        /// </summary>
        public void AppendToSubmesh(int submeshIndex,
                                    IEnumerable<Vector3> vertices,
                                    IEnumerable<Vector2> uv,
                                    IEnumerable<int> triangles)
        {
            if (vertices.Count() != uv.Count())
                throw new ArgumentException($"The counts of {nameof(vertices)} and {nameof(uv)} do not match");

            EnsureSubmesh(submeshIndex);

            this.vertices.AddRange(vertices);
            this.uv.AddRange(uv);

            List<int> triList = submeshTriangles[submeshIndex];
            triList.AddRange(triangles.Select(i => i + vertexOffset));

            vertexOffset += vertices.Count();
        }

        /// <summary>
        /// Merges self with another MeshBuilder into submesh 0.
        /// </summary>
        public void Append(MeshBuilder other) => Append(other);

        /// <summary>
        /// Merges self with another MeshBuilder into a submesh.
        /// </summary>
        public void Append(MeshBuilder other, int submeshIndex)
        {
            EnsureSubmesh(submeshIndex);
            AppendToSubmesh(submeshIndex, other.vertices, other.uv, other.GetTriangles(0));
        }

        /// <summary>
        /// Gets triangles for the requested submesh.
        /// </summary>
        public IReadOnlyList<int> GetTriangles(int submeshIndex)
        {
            if (submeshIndex < 0)
                throw new ArgumentException($"{nameof(submeshIndex)} must be above {0}");

            if (submeshIndex >= submeshTriangles.Count)
                return Array.Empty<int>();

            return submeshTriangles[submeshIndex];
        }

        /// <summary>
        /// Creates a new Unity Mesh object out of the accumulated vertices, uvs and triangles
        /// and assigns all submeshes.
        /// </summary>
        /// <returns>The created mesh.</returns>
        public Mesh ToMesh()
        {
            Mesh mesh = new()
            {
                vertices = vertices.ToArray(),
                uv = uv.ToArray(),
                subMeshCount = submeshTriangles.Count
            };

            for (int i = 0; i < submeshTriangles.Count; i++)
            {
                mesh.SetTriangles(submeshTriangles[i], i);
            }

            mesh.RecalculateBounds();
            mesh.RecalculateNormals();

            return mesh;
        }

        public override string ToString()
        {
            int totalTriangles = submeshTriangles.Sum((val) => val.Count);
            return $"{nameof(MeshBuilder)}[V: {vertices.Count}, UV: {uv.Count}, subT: {submeshTriangles.Count}, T: {totalTriangles}]";
        }

    }
}
