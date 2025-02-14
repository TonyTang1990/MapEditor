/*
 * Description:             MapFoldType.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/20
 */

namespace MapEditor
{
    /// <summary>
    /// MapFoldType.cs
    /// 地图折叠类型
    /// </summary>
    public enum MapFoldType
    {
        MapObjectDataFold = 0,          // 地图对象数据折叠
        PlayerSpawnMapDataFold = 1,     // 地图玩家出生点数据折叠
        MonsterMapDataFold = 2,         // 地图怪物数据折叠
        TreasureBoxMapDataFold = 3,    // 地图宝箱组数据折叠
        TrapMapDataFold = 4,            // 地图陷阱数据折叠
    }
}
