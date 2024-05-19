/*
 * Description:             Collision3DUtilities.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/23
 */

using System;
using UnityEngine;

namespace TH.Module.Collision3D
{
    /// <summary>
    /// 3D碰撞单例工具类
    /// </summary>
    public static class Collision3DUtilities
    {
        /// <summary>
        /// 判断球体与点之间的交叉检测
        /// </summary>
        /// <param name="sphere1"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public static bool SphereAndPointIntersection3D(Sphere3D sphere1, Vector3 point)
        {
            return (sphere1.Center - point).sqrMagnitude < sphere1.Radius * sphere1.Radius;
        }

        /// <summary>
        /// 判断球体与球体之间的交叉检测
        /// </summary>
        /// <param name="sphere1"></param>
        /// <param name="sphere2"></param>
        /// <returns></returns>
        public static bool SphereAndSphereIntersection3D(Sphere3D sphere1, Sphere3D sphere2)
        {
            return (sphere1.Center - sphere2.Center).sqrMagnitude < (sphere1.Radius + sphere2.Radius) * (sphere1.Radius + sphere2.Radius);
        }

        /// <summary>
        /// 判断球体与AABB之间的交叉检测
        /// </summary>
        /// <param name="sphere"></param>
        /// <param name="bound"></param>
        /// <returns></returns>
        public static bool SphereAndAABBIntersection3D(Sphere3D sphere, Bounds bound)
        {
            var closestpointaabb = bound.ClosestPoint(sphere.Center);
            return (closestpointaabb - sphere.Center).sqrMagnitude < sphere.Radius * sphere.Radius;
        }
    }
}