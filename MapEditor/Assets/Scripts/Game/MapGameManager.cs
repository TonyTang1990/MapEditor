/*
 * Description:             MapGameManager.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Versioning;
using UnityEngine;

/// <summary>
/// MapGameManager.cs
/// 地图游戏管理单例类
/// </summary>
public class MapGameManager : SingletonTemplate<MapGameManager>
{
    public MapGameManager() : base()
    {

    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        InputManager.Singleton.Update();
        WorldManager.Singleton.Update();
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    public void LogicUpdate()
    {
        WorldManager.Singleton.LogicUpdate();
    }

    /// <summary>
    /// 固定更新
    /// </summary>
    public void FixedUpdate()
    {
        WorldManager.Singleton.FixedUpdate();
    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    public void LateUpdate()
    {
        WorldManager.Singleton.LateUpdate();
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public void EnterGame()
    {
        WorldManager.Singleton.CreateWrold<MapGameWorld>(WorldName.MapGameWorldName);
    }
}