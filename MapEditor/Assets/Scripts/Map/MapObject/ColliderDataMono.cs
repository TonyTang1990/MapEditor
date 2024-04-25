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
        /// 碰撞器类型
        /// </summary>
        [Header("碰撞器类型")]
        public ColliderType ColliderType = ColliderType.BOX;

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
            var localScale = gameObject.transform.localScale;
            var preGizmoColor = Gizmos.color;
            Gizmos.color = Color.green;
            if(ColliderType == ColliderType.BOX)
            {
                var cubeSize = Size;
                cubeSize.x = cubeSize.x * localScale.x;
                cubeSize.y = cubeSize.y * localScale.y;
                cubeSize.z = cubeSize.z * localScale.z;
                Gizmos.DrawWireCube(Center, cubeSize);
            }
            else if(ColliderType == ColliderType.SPHERE)
            {
                var maxLocalScaleValue = Mathf.Max(localScale.x, localScale.y, localScale.z);
                var sphereRadius = Radius * maxLocalScaleValue;
                Gizmos.DrawWireSphere(Center, sphereRadius);
            }
            Gizmos.color = preGizmoColor;
        }
    }
}