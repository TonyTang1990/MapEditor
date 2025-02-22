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
    /// 主摄像机
    /// </summary>
    public Camera MainCamera
    {
        get;
        private set;
    }

    /// <summary>
    /// UI根节点
    /// </summary>
    public GameObject UIRoot
    {
        get;
        private set;
    }

    /// <summary>
    /// UI摄像机
    /// </summary>
    public Camera UICamera
    {
        get;
        private set;
    }

    /// <summary>
    /// UI Canvas节点
    /// </summary>
    public Transform UICanvas
    {
        get;
        private set;
    }

    /// <summary>
    /// 主界面脚本
    /// </summary>
    public MainUI MainUI
    {
        get;
        private set;
    }

    /// <summary>
    /// 地图实例GameObject
    /// </summary>
    public GameObject MapInstanceGo
    {
        get;
        private set;
    }

    /// <summary>
    /// 地图配置
    /// </summary>
    public MapExport LevelConfig
    {
        get;
        private set;
    }

    public MapGameWorld() : base()
    {

    }

    /// <summary>
    /// 响应地图游戏世界创建
    /// </summary>
    public override void OnCreate()
    {
        base.OnCreate();
        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        LoadUIRoot();
    }

    /// <summary>
    /// 响应地图游戏世界销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 加载UI根节点
    /// </summary>
    private void LoadUIRoot()
    {
        UIRoot = Resoures.Load<GameObject>(MapGameConst.UIRootPrefabPath);
        var uiRootTransform = UIRoot.transform;
        uiRootTransform.position = new Vector3(0, 500, 0);
        UICamera = uiRootTransform.Find("UICamera").GetComponent<Camera>();
        UICanvas = uiRootTransform.Find("Canvas");
        OnLoadUIRootComplete();
    }

    /// <summary>
    /// 响应UI根节点加载完成
    /// </summary>
    private void OnLoadUIRootComplete()
    {
        OpenMainUI();
        LoadMap();
    }

    /// <summary>
    /// 打开MainUI
    /// </summary>
    private void OpenMainUI()
    {
        MainUI = new MainUI("MainUI");
        MainUI.OnOpen(UICanvas);
    }

    /// <summary>
    /// 关闭MainUI
    /// </summary>
    private void CloseMainUI()
    {
        if(MainUI != null)
        {
            MainUI.OnClose();
            MainUI = null;
        }
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
            LevelConfig = JsonUtility.FromJson<MapExport>(levelTxtAsset.text);
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
        CreateSystem<PlayerSpawnSystem>(SystemNames.PlayerSpawnSystemName);
    }
}