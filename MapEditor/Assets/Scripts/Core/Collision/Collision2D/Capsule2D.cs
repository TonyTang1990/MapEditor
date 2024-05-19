/*
 * Description:             Capsule2D.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// 2D胶囊体定义
    /// </summary>
    [Serializable]
    public struct Capsule2D
    {
        /// <summary>
        /// 起点
        /// </summary>
        [Header("起点")]
        public Vector2 StartPoint;

        /// <summary>
        /// 起点线段
        /// </summary>
        [Header("起点线段")]
        public Vector2 PointLine;

        /// <summary>
        /// 胶囊体半径
        /// </summary>
        [Header("胶囊体半径")]
        public float Radius;

        public Capsule2D(Vector2 startpoint, Vector2 pointline, float radius)
        {
            StartPoint = startpoint;
            PointLine = pointline;
            Radius = radius;
        }
    }
}