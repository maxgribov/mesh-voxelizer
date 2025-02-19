using UnityEngine;

namespace MeshData
{
    [System.Serializable]
    public class MeshInfo
    {
        #region properties

        public Vector3[] vertices;
        public int[] triangles;
        public Vector3[] normals;
        public float voxelLength;

        #endregion

        #region construction

        public MeshInfo(Mesh _Mesh)
        {
            vertices = _Mesh.vertices;
            triangles = _Mesh.triangles;
            normals = _Mesh.normals;
        }

        #endregion

        #region MyRegion

        public Mesh ToMesh()
        {
            return new Mesh
            {
                vertices = vertices,
                triangles = triangles,
                normals = normals
            };
        }

        #endregion
    }
}