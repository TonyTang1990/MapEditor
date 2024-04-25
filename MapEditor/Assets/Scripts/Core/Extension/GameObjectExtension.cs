/*
 * Description:             GameObjectExtension.cs
 * Author:                  TONYTANG
 * Create Date:             2019/12/21
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameObjectExtension.cs
/// GameObject扩展方法
/// </summary>
public static class GameObjectExtension
{
    /// <summary>
    /// 获取或添加指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static T GetOrAddComponet<T>(this GameObject go) where T : Component
    {
        var component = go.GetComponent<T>();
        if (component == null)
        {
            component = go.AddComponent<T>();
        }
        return component;
    }
}