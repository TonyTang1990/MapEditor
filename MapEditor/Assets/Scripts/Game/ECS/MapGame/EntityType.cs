/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:31:56
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-13 18:18:54
 * @ Description:
 */

/// <summary>
/// EntityType.cs
/// Entity类型
/// </summary>
public enum EntityType
{
    Invalide = 0,   // 无效Entity类型
    MapGame,        // 地图游戏Entity(全局Entity)
    Player,         // 玩家Entity
    Monster,        // 怪物Entity
    TreasureBox,    // 宝箱Entity
    Trap,           // 陷阱Entity
    Camera,         // 摄像机Entity
}