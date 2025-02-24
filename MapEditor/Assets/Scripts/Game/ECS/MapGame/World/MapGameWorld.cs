/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using MapEditor;
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
    /// 游戏虚拟摄像机组件
    /// </summary>
    public CinemachineVirtualCamera GameVirtualCamera
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

    /// <summary>
    /// 玩家和摄像机位置偏移
    /// </summary>
    public Vector3 PlayerCameraPosOffset
    {
        get;
        private set;
    }

    /// <summary>
    /// 玩家移动速度
    /// </summary>
    public float PlayerMoveSpeed
    {
        get;
        set;
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
        InitMapDatas();
        LoadLevelDatas();
    }

    /// <summary>
    /// 初始化地图数据
    /// </summary>
    private void InitMapDatas()
    {
        if(MapInstanceGo == null)
        {
            Debug.LogError($"找不到地图实例对象，初始化地图数据失败！");
            return;
        }
        var gameVirtualCameraNodeRelativePath = MapUtilities.GetGameVirtualCameraNodeRelativePath();
        var gameVirtualCameraNode = MapInstanceGo.transform.Find(gameVirtualCameraNodeRelativePath);
        GameVirtualCamera = gameVirtualCameraNode != null ? gameVirtualCameraNode.GetComponent<CinemachineVirtualCamera>() : null;
    }

    /// <summary>
    /// 加载关卡数据
    /// </summary>
    private void LoadLevelDatas()
    {
        PlayerMoveSpeed = MapConst.PlayerDefaultMoveSpeed;
        LoadLevelConfig();
    }

    /// <summary>
    /// 加载关卡配置
    /// </summary>
    private void LoadLevelConfig()
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
        InitLevelDatas();
        CreateAllSystem();
    }

    /// <summary>
    /// 初始化关卡数据
    /// </summary>
    private void InitLevelDatas()
    {
        if(LevelConfig != null)
        {
            var playerBirthPos = LevelConfig.MapData.BirthPos.Count > 0 ? LevelConfig.MapData.BirthPos[0] : Vector3.zero;
            PlayerCameraPosOffset = LevelConfig.MapData.GameVirtualCameraInitPos - playerBirthPos;
        }
        else
        {
            PlayerCameraPosOffset = Vector3.zero;
        }
    }

    /// <summary>
    /// 创建所有系统
    /// </summary>
    private void CreateAllSystem()
    {
        CreateSystem<PlayerSpawnSystem>(SystemNames.PlayerSpawnSystemName);
    }
}