/*
 * Description:             Circle2D.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// 2D圆形定义
    /// </summary>
    [Serializable]
    public struct Circle2D
    {
        /// <summary>
        /// 中心点位置
        /// </summary>
        [Header("中心点位置")]
        public Vector2 Center;

        /// <summary>
        /// 半径
        /// </summary>
        [Header("半径")]
        public float Radius;

        public Circle2D(Vector2 center, float radius)
        {
            Center = center;
            Radius = radius;
        }
    }
}
