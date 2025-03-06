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
    public const string UIRootPrefabPath = "Common/UIRoot";

    /// <summary>
    /// 地图预制体路径
    /// </summary>
    public const string MapPrefabPath = "Maps/Level1/Level1";

    /// <summary>
    /// 地图配置路径
    /// </summary>
    public const string LevelConfigPath = "MapExport/Json/Level1";

    /// <summary>
    /// UI窗口预制件目录路径
    /// </summary>
    public const string UIPrefabFolderPath = "UI";

    /// <summary>
    /// 玩家预制件路径
    /// </summary>
    public const string PlayerPrefabPath = "MapObjects/Player/Player";

    /// <summary>
    /// 怪物预制件路径
    /// </summary>
    public const string MonsterPrefabPath = "MapObjects/Monster/Monster";

    /// <summary>
    /// 宝箱预制件路径
    /// </summary>
    public const string TreasureBoxPrefabPath = "MapObjects/TreasureBox/TreasureBox";

    /// <summary>
    /// 陷阱预制件路径
    /// </summary>
    public const string TrapPrefabPath = "MapObjects/Trap/Trap";

    /// <summary>
    /// 角色移动最大Hit距离
    /// </summary>
    public const float ActorNavMeshHitDistance = 2;

    /// <summary>
    /// 区域顶点
    /// </summary>
    public static readonly Vector3 AreaPoint = Vector3.zero;

    /// <summary>
    /// 区域法线
    /// </summary>
    public static readonly Vector3 AreaNormal = Vector3.up;
}