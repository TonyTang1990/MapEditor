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
        PLAYER_SPAWN = 0,           // 玩家出生点位置数据
        MONSTER = 1,                // 怪物数据
        MONSTER_GROUP = 2,          // 怪物组数据
    }
}