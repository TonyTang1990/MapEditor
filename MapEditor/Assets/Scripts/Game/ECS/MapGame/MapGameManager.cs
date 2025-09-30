/*
 * Description:             MapGameManager.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using Cinemachine;
using MapEditor;
using System;
using System.Web.UI;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// MapGameManager.cs
/// 地图游戏管理单例类
/// </summary>
public class MapGameManager : SingletonTemplate<MapGameManager>
{
    /// <summary>
    /// 世界
    /// </summary>
    public BaseWorld World
    {
        get;
        private set;
    }

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
        MainUI?.Update();
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    /// <param name="logicFrameTime"></param>
    public void LogicUpdate(float logicFrameTime)
    {
        WorldManager.Singleton.LogicUpdate(logicFrameTime);
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
        MainCamera = GameObject.Find("MainCamera").GetComponent<Camera>();
        LoadUIRoot();
    }

    /// <summary>
    /// 退出游戏
    /// </summary>
    public void ExistGame()
    {
        WorldManager.Singleton.RemoveWorld(WorldNames.MapGameWorldName);
        World = null;
    }

    /// <summary>
    /// 加载UI根节点
    /// </summary>
    private void LoadUIRoot()
    {
        var uiRootAsset = Resources.Load<GameObject>(MapGameConst.UIRootPrefabPath);
        UIRoot = GameObject.Instantiate(uiRootAsset);
        UIRoot.name = uiRootAsset.name;
        var uiRootTransform = UIRoot.transform;
        uiRootTransform.position = new Vector3(0, 500, 0);
        UICamera = uiRootTransform.Find("UICamera").GetComponent<Camera>();
        UICanvas = uiRootTransform.Find("UICanvas");
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
        if (MainUI != null)
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
        var mapAsset = Resources.Load<GameObject>(MapGameConst.MapPrefabPath);
        MapInstanceGo = GameObject.Instantiate(mapAsset);
        MapInstanceGo.name = mapAsset.name;
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
        if (MapInstanceGo == null)
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
        CreateWorld();
    }

    /// <summary>
    /// 初始化关卡数据
    /// </summary>
    private void InitLevelDatas()
    {
        if (LevelConfig != null)
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
    /// 创建世界
    /// </summary>
    private void CreateWorld()
    {
        var worldBasicData = new WorldBasicData(WorldNames.MapGameWorldName);
        WorldManager.Singleton.CreateWrold<MapGameWorld>(worldBasicData);
    }

    /// <summary>
    /// 获取空待NavMeshAgent的预制件路径
    /// </summary>
    /// <returns></returns>
    public string GetEmptyNavPrefabPath()
    {
        return ECSManager.Singleton.GetCommonPrefabPath(MapGameConst.EmptyNavPrefabName);
    }

    /// <summary>
    /// 根据路径加载Entity预制体
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parent"></param>
    /// <param name="worldPos"></param>
    /// <param name="eulerAngles"></param>
    /// <param name="loadCompleteCb"></param>
    public void LoadObjectEntityEmptyNavPrefab(BaseObjectEntity entity, Transform parent, Vector3 worldPos, Vector3 eulerAngles, Action<BaseEntity> loadCompleteCb = null)
    {
        var emptyNavPrefabPath = GetEmptyNavPrefabPath();
        ECSManager.Singleton.LoadObjectEntityPrefabByPath(emptyNavPrefabPath, entity, parent, worldPos, eulerAngles, loadCompleteCb);
    }
}