/*
 * Description:             Sphere3DVisible.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision3D
{
    /// <summary>
    /// 3D球形绘制
    /// </summary>
    public class Sphere3DVisible : MonoBehaviour
    {
        /// <summary>
        /// 顶点Color
        /// </summary>
        [Header("顶点Color")]
        public Color MeshColor = Color.red;

        /// <summary>
        /// 3D球形信息
        /// </summary>
        [Header("3D球形信息")]
        public Sphere3D Sphere3D;

        void OnDrawGizmos()
        {
            Gizmos.color = MeshColor;
            Gizmos.DrawWireSphere(Sphere3D.Center, Sphere3D.Radius);
        }
    }
}