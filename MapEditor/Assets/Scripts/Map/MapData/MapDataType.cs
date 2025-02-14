/*
 * Description:             MapDataType.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

namespace MapEditor
{
    /// <summary>
    /// MapDataType.cs
    /// 地图数据埋点类型枚举
    /// </summary>
    public enum MapDataType
    {
        PlayerSpawn = 0,           // 玩家出生点位置数据
        Monster = 1,               // 怪物数据
        TreasureBox = 2,           // 宝箱数据
        Trap = 3,                  // 陷阱数据
    }
}