/*
 * Description:             OBB2DVisible.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// OBB2DVisible.cs
    /// OBB 2D可视化绘制
    /// </summary>
    public class OBB2DVisible : MonoBehaviour
    {
        /// <summary>
        /// 顶点Color
        /// </summary>
        [Header("顶点Color")]
        public Color MeshColor = Color.red;

        /// <summary>
        /// 2D OBB信息
        /// </summary>
        [Header("2D OBB信息")]
        public OBB2D OBB2D;

        /// <summary>
        /// 需要绘制的Mesh
        /// </summary>
        private Mesh mDrawMesh;

        /// <summary>
        /// Mesh顶点列表
        /// </summary>
        private List<Vector3> mMeshVerticesList;

        void OnDrawGizmos()
        {
            if (mDrawMesh == null)
            {
                mMeshVerticesList = new List<Vector3>(4);
                for (int i = 0; i < 4; i++)
                {
                    mMeshVerticesList.Add(Vector3.zero);
                }
                mDrawMesh = MeshUtilities.CreateMesh(mMeshVerticesList);
            }
            updateMeshData();
            Gizmos.color = MeshColor;
            Gizmos.DrawWireMesh(mDrawMesh);
        }

        /// <summary>
        /// 更新Mesh数据
        /// </summary>
        private void updateMeshData()
        {
            var center = new Vector3(OBB2D.Center.x, 0, OBB2D.Center.y);
            var halfextents = OBB2D.Extents / 2;
            mMeshVerticesList[0] = Quaternion.AngleAxis(OBB2D.Angle, Vector3.up) * new Vector3(-halfextents.x, 0, -halfextents.y) + center;
            mMeshVerticesList[1] = Quaternion.AngleAxis(OBB2D.Angle, Vector3.up) * new Vector3(-halfextents.x, 0, halfextents.y) + center;
            mMeshVerticesList[2] = Quaternion.AngleAxis(OBB2D.Angle, Vector3.up) * new Vector3(halfextents.x, 0, halfextents.y) + center;
            mMeshVerticesList[3] = Quaternion.AngleAxis(OBB2D.Angle, Vector3.up) * new Vector3(halfextents.x, 0, -halfextents.y) + center;
            mDrawMesh.SetVertices(mMeshVerticesList);
            mDrawMesh.RecalculateNormals();
        }
    }
}