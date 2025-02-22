/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 18:20:56
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 18:21:45
 * @ Description:
 */

/// <summary>
/// MapGameConst.cs
/// 游戏常量
/// </summary>
public static class MapGameConst
{
    /// <summary>
    /// UI根预制件路径
    /// </summary>
    public const string UIRootPrefabPath = "Common/UIRoot.prefab";

    /// <summary>
    /// 地图预制体路径
    /// </summary>
    public const string MapPrefabPath = "Maps/Level1/Level1.prefab";

    /// <summary>
    /// 地图配置路径
    /// </summary>
    public const string LevelConfigPath = "MapExport/Json/Level1.json";

    /// <summary>
    /// UI窗口预制件目录路径
    /// </summary>
    public const string UIPrefabFolderPath = "UI";

    /// <summary>
    /// 玩家预制件路径
    /// </summary>
    public const string PlayerPrefabPath = "MapObjects/Player/Player.prefab";

    /// <summary>
    /// 角色移动最大Hit距离
    /// </summary>
    public const float ActorNavMeshHitDistance = 2;
}