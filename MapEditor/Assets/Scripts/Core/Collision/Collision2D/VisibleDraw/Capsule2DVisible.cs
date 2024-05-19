/*
 * Description:             Capsule2DVisible.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/30
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// Capsule2DVisible.cs
    /// Capsule 2D可视化绘制
    /// </summary>
    public class Capsule2DVisible : MonoBehaviour
    {
        /// <summary>
        /// 半圆顶点数量
        /// </summary>
        [Header("半圆顶点数量")]
        public int VerticesCount = 25;

        /// <summary>
        /// 顶点Color
        /// </summary>
        [Header("顶点Color")]
        public Color MeshColor = Color.red;

        /// <summary>
        /// 2D胶囊体信息
        /// </summary>
        [Header("2D胶囊体信息")]
        public Capsule2D Capsule2D;

        /// <summary>
        /// 需要绘制的中间Mesh
        /// </summary>
        private Mesh mCenterDrawMesh;

        /// <summary>
        /// 需要绘制的顶部半圆Mesh
        /// </summary>
        private Mesh mTopCircleDrawMesh;

        /// <summary>
        /// 需要绘制的底部半圆Mesh
        /// </summary>
        private Mesh mBobbleCircleDrawMesh;

        /// <summary>
        /// 中间Mesh顶点列表
        /// </summary>
        private List<Vector3> mCenterMeshVerticesList;

        /// <summary>
        /// 顶部半圆Mesh顶点列表
        /// </summary>
        private List<Vector3> mTopCircleMeshVerticesList;

        /// <summary>
        /// 底部半圆Mesh顶点列表
        /// </summary>
        private List<Vector3> mBottleCircleMeshVerticesList;

        void OnDrawGizmos()
        {
            if (mCenterDrawMesh == null || (mCenterDrawMesh != null && mTopCircleMeshVerticesList.Count != VerticesCount))
            {
                mCenterMeshVerticesList = new List<Vector3>(4);
                mTopCircleMeshVerticesList = new List<Vector3>(VerticesCount);
                mBottleCircleMeshVerticesList = new List<Vector3>(VerticesCount);
                for (int i = 0; i < 4; i++)
                {
                    mCenterMeshVerticesList.Add(Vector3.zero);
                }
                for (int i = 0; i < VerticesCount; i++)
                {
                    mTopCircleMeshVerticesList.Add(Vector3.zero);
                }
                for (int i = 0; i < VerticesCount; i++)
                {
                    mBottleCircleMeshVerticesList.Add(Vector3.zero);
                }
                mCenterDrawMesh = MeshUtilities.CreateMesh(mCenterMeshVerticesList);
                mTopCircleDrawMesh = MeshUtilities.CreateMesh(mTopCircleMeshVerticesList);
                mBobbleCircleDrawMesh = MeshUtilities.CreateMesh(mTopCircleMeshVerticesList);
            }
            updateMeshData();
            Gizmos.color = MeshColor;
            Gizmos.DrawWireMesh(mCenterDrawMesh);
            Gizmos.DrawWireMesh(mTopCircleDrawMesh);
            Gizmos.DrawWireMesh(mBobbleCircleDrawMesh);
        }

        /// <summary>
        /// 更新Mesh数据
        /// </summary>
        private void updateMeshData()
        {
            // 分两部分绘制
            // 1. 胶囊体中心部分(4个顶点)
            // 2. 胶囊体两头部分(各25个顶点)
            var goForward = transform.forward;
            // 点乘计算旋转了多少度
            var pointLineVector3 = new Vector3(Capsule2D.PointLine.x, 0, Capsule2D.PointLine.y);
            var vectorDot = Vector3.Dot(goForward, pointLineVector3);
            var radians = Mathf.Acos(vectorDot / (goForward.magnitude * pointLineVector3.magnitude));
            var angle = Mathf.Rad2Deg * radians;
            // 叉乘计算旋转方向(即轴)
            var rotateAxis = Vector3.Cross(goForward, pointLineVector3);            
            Vector3 forward = new Vector3(Capsule2D.PointLine.x, 0, Capsule2D.PointLine.y).normalized;
            var lineLength = Capsule2D.PointLine.magnitude;
            var center = Capsule2D.StartPoint;
            center = center + Capsule2D.PointLine / 2;
            var halfextents = new Vector2(Capsule2D.Radius, lineLength / 2);
            // 旋转椭圆中间Size
            // 先旋转后平移
            mCenterMeshVerticesList[0] = new Vector3(-halfextents.x, 0, -halfextents.y);
            mCenterMeshVerticesList[1] = new Vector3(-halfextents.x, 0, halfextents.y);
            mCenterMeshVerticesList[2] = new Vector3(halfextents.x, 0,halfextents.y);
            mCenterMeshVerticesList[3] = new Vector3(halfextents.x, 0, -halfextents.y);
            for(int i = 0, length = mCenterMeshVerticesList.Count; i < length; i++)
            {
                mCenterMeshVerticesList[i] = Quaternion.AngleAxis(angle, rotateAxis) * mCenterMeshVerticesList[i];
            }
            var offset = new Vector3(center.x, 0, center.y);
            mCenterMeshVerticesList[0] += offset;
            mCenterMeshVerticesList[1] += offset;
            mCenterMeshVerticesList[2] += offset;
            mCenterMeshVerticesList[3] += offset;
            mCenterDrawMesh.SetVertices(mCenterMeshVerticesList);
            mCenterDrawMesh.RecalculateNormals();

            if (VerticesCount > 0)
            {
                var right = Quaternion.Euler(0, 90, 0) * forward;
                // 先旋转后平移
                var tophalfcirclecenter = Quaternion.AngleAxis(angle, rotateAxis) * new Vector3(0, 0, halfextents.y);
                var bottlehalfcirclecenter = Quaternion.AngleAxis(angle, rotateAxis) * new Vector3(0, 0, -halfextents.y);
                tophalfcirclecenter += offset;
                bottlehalfcirclecenter += offset;
                float eachangle = 180f / (VerticesCount - 2);
                mTopCircleMeshVerticesList[0] = tophalfcirclecenter;
                for (int i = 1, length = VerticesCount; i < length; i++)
                {
                    Vector3 pos = Quaternion.Euler(0f, eachangle * (i - 1), 0f) * -right * halfextents.x + tophalfcirclecenter;
                    mTopCircleMeshVerticesList[i] = pos;
                }
                mBottleCircleMeshVerticesList[0] = bottlehalfcirclecenter;
                for (int i = 1, length = VerticesCount; i < length; i++)
                {
                    Vector3 pos = Quaternion.Euler(0f, eachangle * (i - 1), 0f) * right * halfextents.x + bottlehalfcirclecenter;
                    mBottleCircleMeshVerticesList[i] = pos;
                }
                mTopCircleDrawMesh.SetVertices(mTopCircleMeshVerticesList);
                mTopCircleDrawMesh.RecalculateNormals();
                mBobbleCircleDrawMesh.SetVertices(mBottleCircleMeshVerticesList);
                mBobbleCircleDrawMesh.RecalculateNormals();
            }
        }
    }
}