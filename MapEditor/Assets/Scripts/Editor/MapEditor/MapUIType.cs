/*
 * Description:             MapUIType.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/20
 */

namespace MapEditor
{
    /// <summary>
    /// MapUIType.cs
    /// 地图数据UI类型
    /// </summary>
    public enum MapUIType
    {
        Batch = 0,                              // 批量UI
        Index = 1,                              // 索引UI
        UID = 2,                                // UID UI
        MapDataType = 3,                        // 地图埋点类型UI
        ConfId = 4,                             // 配置Id UI
        Position = 5,                           // 位置UI
        Rotation = 6,                           // 旋转UI
        Des = 7,                                // 描述UI
        MoveUp = 8,                             // 上移UI
        MoveDown = 9,                           // 下移UI
        Add = 10,                               // 添加UI
        Remove = 11,                            // 移除UI
        MonsterCreateRadius = 12,               // 怪物创建半径UI
        MonsterActiveRadius = 13,               // 怪物警戒半径UI
        GUISwitchOff = 14,                      // GUI关闭开关UI
    }
}
