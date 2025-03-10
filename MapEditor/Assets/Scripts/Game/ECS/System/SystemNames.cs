﻿/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using System;
using System.Collections.Generic;

/// <summary>
/// SystemNames.cs
/// 系统名定义
/// </summary>
public static class SystemNames
{
    /// <summary>
    /// 地图游戏Entity生成系统名
    /// </summary>
    public const string MapGameEntitySpawnSystemName = "MapGameEntitySpawnSystem";

    /// <summary>
    /// 操作控制系统名
    /// </summary>
    public const string InputControlSystemName = "InputControlSystem";

    /// <summary>
    /// 玩家生成系统名
    /// </summary>
    public const string PlayerSpawnSystemName = "PlayerSpawnSystem";

    /// <summary>
    /// 角色同步系统名
    /// </summary>
    public const string ActorSyncSystemName = "ActorSyncSystem";

    /// <summary>
    /// 摄像机跟随系统名
    /// </summary>
    public const string CameraFollowSystemName = "CameraFollowSystem";

    /// <summary>
    /// 地图对象Entity生成系统名
    /// </summary>
    public const string MapObjectEntitySpawnSystemName = "MapObjectEntitySpawnSystem";

    /// <summary>
    /// 地图对象GameObject生成系统名
    /// </summary>
    public const string MapObjectGoSpawnSystemName = "MapObjectGoSpawnSystem";
}