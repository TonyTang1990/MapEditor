/*
 * Description:             Circle2DVisible.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// Circle2DVisible.cs
    /// Circle 2D可视化绘制
    /// </summary>
    public class Circle2DVisible : MonoBehaviour
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
        /// 2D圆形信息
        /// </summary>
        [Header("2D圆形信息")]
        public Circle2D Circle2D;

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
        /// 更新Circle Mesh数据
        /// </summary>
        private void updateMeshData()
        {
            if (VerticesCount > 2)
            {
                float eachangle = 360f / (VerticesCount - 2);
                Vector3 forward = Vector3.forward;
                Vector3 centerPos = new Vector3(Circle2D.Center.x, 0, Circle2D.Center.y);
                mMeshVerticesList[0] = centerPos;
                for (int i = 1, length = mMeshVerticesList.Count; i < length; i++)
                {
                    Vector3 pos = Quaternion.Euler(0f, eachangle * (i - 1), 0f) * forward * Circle2D.Radius + centerPos;
                    mMeshVerticesList[i] = pos;
                }
                mDrawMesh.SetVertices(mMeshVerticesList);
                mDrawMesh.RecalculateNormals();
            }
        }
    }
}