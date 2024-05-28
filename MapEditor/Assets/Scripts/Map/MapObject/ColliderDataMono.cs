/*
 * Description:             ColliderDataMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// ColliderDataMono.cs
    /// 碰撞器数据挂载脚本
    /// </summary>
    public class ColliderDataMono : MonoBehaviour
    {
        /// <summary>
        /// 碰撞体中心位置
        /// </summary>
        [Header("碰撞体中心位置")]
        public Vector3 Center;

        /// <summary>
        /// 碰撞体大小
        /// </summary>
        [Header("碰撞体大小")]
        public Vector3 Size;

        /// <summary>
        /// 碰撞体半径
        /// </summary>
        [Header("碰撞体半径")]
        public float Radius = 1;

        /// <summary>
        /// Gizmos选中绘制
        /// </summary>
        public void OnDrawGizmosSelected()
        {
            var preGizmosMatrix = Gizmos.matrix;
            var colliderMatrix = gameObject.transform.localToWorldMatrix;
            Gizmos.matrix = colliderMatrix;
            var preGizmoColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(Center, Size);
            Gizmos.color = preGizmoColor;
            Gizmos.matrix = preGizmosMatrix;
        }
    }
}