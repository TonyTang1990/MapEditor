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
        MonsterGroup = 2,          // 怪物组数据
        Template = 3,              // 模板数据
    }
}