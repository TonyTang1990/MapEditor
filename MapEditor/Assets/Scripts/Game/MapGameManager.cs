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
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        WorldManager.Singleton.Update(deltaTime);
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
    /// <param name="fixedDeltaTime"></param>
    public void FixedUpdate(float fixedDeltaTime)
    {
        WorldManager.Singleton.FixedUpdate(fixedDeltaTime);
    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void LateUpdate(float deltaTime)
    {
        WorldManager.Singleton.LateUpdate(deltaTime);
    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public void EnterGame()
    {
        WorldManager.Singleton.CreateWrold<MapGameWorld>(WorldNames.MapGameWorldName);
    }
}