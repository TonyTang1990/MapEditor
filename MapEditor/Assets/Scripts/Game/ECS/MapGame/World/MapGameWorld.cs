/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Resources;

/// <summary>
/// MapGameWorld.cs
/// 地图游戏世界
/// </summary>
public class MapGameWorld : BaseWorld
{
    /// <summary>
    /// 地图实例GameObject
    /// </summary>
    public GameObjectExtension MapInstanceGo
    {
        get;
        private set;
    }

    /// <summary>
    /// 地图配置
    /// </summary>
    private MapExport mLevelConfig;

    /// <summary>
    /// 响应地图游戏世界创建
    /// </summary>
    public override void OnCreate()
    {
        base.OnCreate();
        LoadMap();
    }

    /// <summary>
    /// 响应地图游戏世界销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 加载地图
    /// </summary>
    private void LoadMap()
    {
        MapInstanceGo = Resources.Load<GameObject>(MapGameConst.MapPrefabPath);
        OnMapLoadComplete();
    }

    /// <summary>
    /// 地图加载完成
    /// </summary>
    private void OnMapLoadComplete()
    {
        var levelTxtAsset = Resources.Load<TextAsset>(MapGameConst.LevelConfigPath);
        if (levelTxtAsset == null)
        {
            Debug.LogError($"关卡配置:{MapGameConst.LevelConfigPath}加载失败！");
        }
        else
        {
            mLevelConfig = JsonUtility.FromJson<MapExport>(levelTxtAsset.text);
        }
        OnLevelConfigLoadComplete();
    }

    /// <summary>
    /// 关卡配置加载完成
    /// </summary>
    private void OnLevelConfigLoadComplete()
    {
        CreateAllSystem();
    }

    /// <summary>
    /// 创建所有系统
    /// </summary>
    private void CreateAllSystem()
    {

    }
}