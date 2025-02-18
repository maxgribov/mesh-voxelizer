using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MeshData;
using UnityEngine;

namespace Editor.View
{
    public class View : MonoBehaviour
    {
        #region attributes

        [SerializeField] private TextAsset m_JsonFile;
        [SerializeField] private MeshFilter m_ViewObject;
        [SerializeField] private GameObject m_Prefab;
        [SerializeField] private bool m_ShowDemo;

        private MeshInfo m_MeshData;
        private Mesh m_Mesh;
        private List<GameObject> m_Voxels;
        private List<Vector3> m_VoxelCenters;
        private Coroutine m_DemoRoutine;

        #endregion

        #region engine methods

        void Start()
        {
            m_MeshData = JsonUtility.FromJson<MeshInfo>(m_JsonFile.text);

            m_Mesh = m_MeshData.ToMesh();

            m_ViewObject.mesh = m_Mesh;

            m_VoxelCenters = MeshVoxelizer.MeshVoxelizer.DefineAllCenterVertices(m_Mesh, m_MeshData.voxelLength);
        }

        void Update()
        {
            if (!m_ShowDemo && m_DemoRoutine != null)
            {
                StopCoroutine(m_DemoRoutine);
                m_DemoRoutine = null;

                for (int i = 0; i < m_Voxels.Count; i++)
                {
                    m_Voxels[i].transform.position = m_VoxelCenters[i];
                    m_Voxels[i].gameObject.SetActive(false);
                }
                
                m_ViewObject.gameObject.SetActive(true);
            }

            if (m_ShowDemo && m_DemoRoutine == null)
            {
                m_DemoRoutine = StartCoroutine(DemoAnimation());
            }
        }

        private void DrawShape()
        {
            Gizmos.color = Color.magenta;
            for (int i = 0; i < m_Mesh.triangles.Length - 2; i += 3)
            {
                Gizmos.DrawLine(
                    m_Mesh.vertices[m_Mesh.triangles[i]],
                    m_Mesh.vertices[m_Mesh.triangles[i + 1]]);

                Gizmos.DrawLine(
                    m_Mesh.vertices[m_Mesh.triangles[i + 2]],
                    m_Mesh.vertices[m_Mesh.triangles[i + 1]]);
                Gizmos.DrawLine(
                    m_Mesh.vertices[m_Mesh.triangles[i]],
                    m_Mesh.vertices[m_Mesh.triangles[i + 2]]);
            }
        }

        private void DrawVerts()
        {
            Gizmos.color = Color.yellow;
            if (m_VoxelCenters?.Count > 0)
            {
                foreach (Vector3 vector3 in m_VoxelCenters)
                    Gizmos.DrawWireSphere(vector3, m_MeshData.voxelLength * 0.1f);
            }
        }

        private void OnDrawGizmos()
        {
            if (m_Mesh == null)
                return;

            DrawShape();
            DrawVerts();
        }

        #endregion

        #region service methods

        private List<GameObject> CreateVoxels()
        {
            List<GameObject> voxels = new List<GameObject>();
            foreach (Vector3 vector3 in m_VoxelCenters)
            {
                GameObject voxelCube = Instantiate(m_Prefab, vector3, Quaternion.identity);
                voxelCube.transform.localScale = Vector3.one * m_MeshData.voxelLength;
                voxelCube.SetActive(false);
                voxels.Add(voxelCube);
            }

            return voxels;
        }

        private IEnumerator DemoAnimation()
        {
            m_Voxels ??= CreateVoxels();
            var delay = new WaitForSeconds(1);

            while (true)
            {
                m_ViewObject.gameObject.SetActive(true);
                yield return delay;
                m_ViewObject.gameObject.SetActive(false);
                
                m_Voxels.ForEach(_Voxel => _Voxel.SetActive(true));
                yield return StartCoroutine(CubesAnimation(m_Voxels));
                m_Voxels.ForEach(_Voxel => _Voxel.SetActive(false));
            }
        }

        private IEnumerator CubesAnimation(List<GameObject> _Voxels)
        {
            var startPositions = _Voxels.Select(_Voxel => _Voxel.transform.position).ToList();
            var endPositions = startPositions.Select(_Position => _Position * 2).ToList();

            float duration = 1f;
            float t = 0;
            while (true)
            {
                t += Time.deltaTime;
                if (t < duration)
                {
                    for (int i = 0; i < _Voxels.Count; i++)
                        _Voxels[i].transform.position =
                            Vector3.Lerp(startPositions[i], endPositions[i], t / duration);
                }
                else if (t < 2 * duration)
                {
                    for (int i = 0; i < _Voxels.Count; i++)
                        _Voxels[i].transform.position =
                            Vector3.Lerp(endPositions[i], startPositions[i], t - duration);
                }
                else
                {
                    break;
                }

                yield return null;
            }
        }

        #endregion
    }
}