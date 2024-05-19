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
    /// 2D AABB定义
    /// </summary>
    [Serializable]
    public struct AABB2D
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

        public AABB2D(Vector2 center, Vector2 extents)
        {
            Center = center;
            Extents = extents;
        }
    }
}