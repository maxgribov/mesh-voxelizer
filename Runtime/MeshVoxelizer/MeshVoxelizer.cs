using System.Collections.Generic;
using MathStructs;
using UnityEngine;

namespace MeshVoxelizer
{
    public class MeshVoxelizer
    {
        #region public methods

        public static List<Vector3> DefineAllCenterVertices(Mesh _Mesh, float _EdgeLength)
        {
            return GetCubeCenters(_Mesh, _EdgeLength);
        }

        #endregion

        #region service methods

        private static List<Vector3> GetCubeCenters(Mesh _Mesh, float _EdgeLength)
        {
            var result = new List<Vector3>();

            int xStepsCount = (int)Mathf.Round(2 * _Mesh.bounds.extents.x / _EdgeLength);
            int yStepsCount = (int)Mathf.Round(2 * _Mesh.bounds.extents.y / _EdgeLength);
            int zStepsCount = (int)Mathf.Round(2 * _Mesh.bounds.extents.z / _EdgeLength);
            Vector3 startPoint = _Mesh.bounds.min + Vector3.one * _EdgeLength * 0.5f;
            for (int i = 0; i < xStepsCount; i++)
            for (int j = 0; j < yStepsCount; j++)
            for (int k = 0; k < zStepsCount; k++)
            {
                Vector3 vacantCenter =
                    startPoint + _EdgeLength * (i * Vector3.right + j * Vector3.up + k * Vector3.forward);
                bool isOutside = true;

                foreach (XZQuad quad in CalculateXZQuads(_Mesh))
                {
                    if (quad.ContainsVerticalProjection(vacantCenter))
                        isOutside = !isOutside;
                }

                if (isOutside)
                    continue;

                result.Add(vacantCenter);
            }

            return result;
        }

        private static HashSet<XZQuad> CalculateXZQuads(Mesh _Mesh)
        {
            var quads = new HashSet<XZQuad>();

            for (int i = 0; i < _Mesh.triangles.Length; i += 3)
            {
                Vector3 a = _Mesh.vertices[_Mesh.triangles[i]];
                Vector3 b = _Mesh.vertices[_Mesh.triangles[i + 1]];
                Vector3 c = _Mesh.vertices[_Mesh.triangles[i + 2]];
                
                bool isXZQuad = MathUtility.MathUtility.IsXZQuad(a, b, c);

                if (!isXZQuad)
                    continue;
                
                Vector3 d = MathUtility.MathUtility.GetQuadCorner(a, b, c);

                quads.Add(new XZQuad(new[] { a, b, c, d }));
            }

            return quads;
        }

        #endregion
    }
}