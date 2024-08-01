/*
 * Description:             UnityUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/07/29
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UnityUtilities.cs
/// Unity相关静态工具类
/// </summary>
public static class UnityUtilities
{
    /// <summary>
    /// 获取制定GameObject的所有Renderer的Bounds
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static Bounds GetGoRenderersBounds(GameObject gameObject)
    {
        var bounds = new Bounds(gameObject.transform.position, Vector3.zero);
        if (gameObject == null)
        {
            return bounds;
        }
        var renderers = gameObject.GetComponentsInChildren<Renderer>();
        if(renderers == null || renderers.Length == 0)
        {
            return bounds;
        }
        bounds.center = gameObject.transform.position;
        foreach(var renderer in renderers)
        {
            bounds.Encapsulate(renderer.bounds);
        }
        // 忽略位置带来的偏移影响
        bounds.center -= gameObject.transform.position;
        return bounds;
    }
}