/*
 * Description:             Sector2DVisible.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// Sector2DVisible.cs
    /// Sector 2D可视化绘制
    /// </summary>
    public class Sector2DVisible : MonoBehaviour
    {
        /// <summary>
        /// 顶点数量
        /// </summary>
        [Header("顶点数量")]
        public int VerticesCount = 100;

        /// <summary>
        /// 顶点Color
        /// </summary>
        [Header("顶点Color")]
        public Color MeshColor = Color.red;

        /// <summary>
        /// 2D扇形信息
        /// </summary>
        [Header("2D扇形信息")]
        public Sector2D Sector2D;

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
            if (mDrawMesh == null || (mDrawMesh != null && mMeshVerticesList.Count != VerticesCount))
            {
                mMeshVerticesList = new List<Vector3>(VerticesCount);
                for (int i = 0; i < VerticesCount; i++)
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
            // -2是因为要排除原点和扇形第一个边的点
            float eachangle = Sector2D.Angle / (VerticesCount - 2);
            Vector3 forward = new Vector3(Sector2D.Direction.x, 0, Sector2D.Direction.y);
            Vector3 startPos = new Vector3(Sector2D.StartPoint.x, 0, Sector2D.StartPoint.y);
            mMeshVerticesList[0] = startPos;
            for (int i = 1; i < VerticesCount; i++)
            {
                Vector3 pos = Quaternion.Euler(0f, -Sector2D.Angle / 2 + eachangle * (i - 1), 0f) * forward * Sector2D.Radius + startPos;
                mMeshVerticesList[i] = pos;
            }
            mDrawMesh.SetVertices(mMeshVerticesList);
            mDrawMesh.RecalculateNormals();
        }
    }
}