/*
 * Description:             AABB2DVisible.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// AABB2DVisible.cs
    /// AABB 2D可视化绘制
    /// </summary>
    public class AABB2DVisible : MonoBehaviour
    {
        /// <summary>
        /// 顶点Color
        /// </summary>
        [Header("顶点Color")]
        public Color MeshColor = Color.red;

        /// <summary>
        /// 2D AABB信息
        /// </summary>
        public AABB2D AABB2D;

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
            var center = AABB2D.Center;
            var halfextents = AABB2D.Extents / 2;
            mMeshVerticesList[0] = new Vector3(center.x - halfextents.x, 0, center.y - halfextents.y);
            mMeshVerticesList[1] = new Vector3(center.x - halfextents.x, 0, center.y + halfextents.y);
            mMeshVerticesList[2] = new Vector3(center.x + halfextents.x, 0, center.y + halfextents.y);
            mMeshVerticesList[3] = new Vector3(center.x + halfextents.x, 0, center.y - halfextents.y);
            mDrawMesh.SetVertices(mMeshVerticesList);
            mDrawMesh.RecalculateNormals();
        }
    }
}