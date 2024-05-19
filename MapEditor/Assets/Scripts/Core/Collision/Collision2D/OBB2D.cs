/*
 * Description:             AABB2D.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// 2D OBB定义
    /// </summary>
    [Serializable]
    public struct OBB2D
    {
        /// <summary>
        /// 中心点位置
        /// </summary>
        [Header("中心点位置")]
        public Vector2 Center;

        /// <summary>
        /// 宽和高
        /// </summary>
        [Header("宽和高")]
        public Vector2 Extents;

        /// <summary>
        /// 旋转角度
        /// </summary>
        [Header("旋转角度")]
        public float Angle;

        public OBB2D(Vector2 center, Vector2 extents, float angle)
        {
            Center = center;
            Extents = extents;
            Angle = angle;
        }
    }
}