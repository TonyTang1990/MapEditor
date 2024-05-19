/*
 * Description:             Vector3Utilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Vector3Utilities.cs
/// Vector3静态工具类
/// </summary>
public static class Vector3Utilities
{
    /// <summary>
    /// 获取指定射线和平面数据的交叉点(返回null表示没有交叉点)
    /// </summary>
    /// <param name="rayOrigin"></param>
    /// <param name="rayDirection"></param>
    /// <param name="planePoint"></param>
    /// <param name="planeNormal"></param>
    /// <returns></returns>
    public static Vector3? GetRayAndPlaneIntersect(Vector3 rayOrigin, Vector3 rayDirection, Vector3 planePoint, Vector3 planeNormal)
    {
        // 计算法向量和方向向量的点积
        float ndotu = Vector3.Dot(planeNormal, rayDirection);

        // 向量几乎平行，可能没有交点或者射线在平面内
        if (Mathf.Approximately(Math.Abs(ndotu), Mathf.Epsilon))
        {
            return null;
        }

        // 计算 t
        Vector3 w = rayOrigin - planePoint;
        float t = -Vector3.Dot(planeNormal, w) / ndotu;

        // 交点在射线起点的后面
        if (t < 0)
        {
            return null;
        }

        // 计算交点
        Vector3 intersectionPoint = rayOrigin + t * rayDirection;
        return intersectionPoint;
    }
}