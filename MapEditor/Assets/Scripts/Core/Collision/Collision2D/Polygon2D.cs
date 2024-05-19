/*
 * Description:             Polygon2D.cs
 * Author:                  TONYTANG
 * Create Date:             2022/03/20
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TH.Module.Collision2D
{
    /// <summary>
    /// Polygon2D.cs
    /// 多边形定义
    /// </summary>
    [Serializable]
    public struct Polygon2D
    {
        /// <summary>
        /// 多边形顶点(注意保持统一顺时针或者逆时针定义)
        /// </summary>
        [Header("多边形顶点(顺时针)")]
        public Vector2[] Vertexes;

        public Polygon2D(Vector2[] vertexes)
        {
            Vertexes = vertexes;
        }

        /// <summary>
        /// 是否是有效多边形
        /// </summary>
        /// <returns></returns>
        public bool IsValide()
        {
            if (Vertexes == null || Vertexes.Length < 3)
            {
                //Debug.LogWarning("多边形顶点数不应该小于3个!");
                return false;
            }
            return true;
        }
    }
}