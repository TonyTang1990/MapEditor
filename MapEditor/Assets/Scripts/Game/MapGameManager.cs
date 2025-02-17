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
    /// 更新
    /// </summary>
    public void Update()
    {
        InputManager.Singleton.Update();
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    public void LogicUpdate()
    {

    }

    /// <summary>
    /// 固定更新
    /// </summary>
    public void FixedUpdate()
    {

    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    public void LateUpdate()
    {

    }

    /// <summary>
    /// 进入游戏
    /// </summary>
    public void EnterGame()
    {
        MapInstanceGo = ResourceS.Load<GameObject>(MapGameConst.MapPrefabPath);
        OnMapLoadComplete();
    }

    /// <summary>
    /// 地图加载完成
    /// </summary>
    private void OnMapLoadComplete()
    {
        var levelTxtAsset = Resources.Load<TextAsset>(MapGameConst.LevelConfigPath);
        if(levelTxtAsset == null)
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
        CreateAllEntities();   
    }

    /// <summary>
    /// 创建所有实体
    /// </summary>
    private void CreateAllEntities()
    {
        
    }
}