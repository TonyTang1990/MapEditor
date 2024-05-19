/*
 * Description:             Sphere3D.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision3D
{
    /// <summary>
    /// 球体定义
    /// </summary>
    [Serializable]
    public struct Sphere3D
    {
        /// <summary>
        /// 中心点
        /// </summary>
        [Header("中心点")]
        public Vector3 Center;

        /// <summary>
        /// 半径
        /// </summary>
        [Header("半径")]
        public float Radius;

        public Sphere3D(Vector3 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}