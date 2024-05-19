/*
 * Description:             Sector2D.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// 2D扇形定义
    /// </summary>
    [Serializable]
    public struct Sector2D
    {
        /// <summary>
        /// 扇形起点
        /// </summary>
        [Header("扇形起点")]
        public Vector2 StartPoint;

        /// <summary>
        /// 扇形半径
        /// </summary>
        [Header("扇形半径")]
        public float Radius;

        /// <summary>
        /// 扇形朝向
        /// </summary>
        [Header("扇形朝向")]
        public Vector2 Direction;

        /// <summary>
        /// 扇形角度
        /// </summary>
        [Header("扇形角度")]
        public float Angle;

        public Sector2D(Vector2 startpoint, float radius, Vector2 direction, float angle)
        {
            StartPoint = startpoint;
            Radius = radius;
            Direction = direction;
            Angle = angle;
        }
    }
}