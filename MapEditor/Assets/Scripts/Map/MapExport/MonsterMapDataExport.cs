/*
 * Description:             MonsterMapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/24
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MonsterMapDataExport.cs
    /// 怪物地图埋点数据导出
    /// </summary>
    [Serializable]
    public class MonsterMapDataExport : BaseMapDataExport
    {
        /// <summary>
        /// 怪物创建半径
        /// </summary>
        public float MonsterCreateRadius;

        /// <summary>
        /// 怪物警戒半径
        /// </summary>
        public float MonsterActiveRadius;

        public MonsterMapDataExport(MapDataType mapDataType, int confId, Vector3 position,  Vector3 rotation, float monsterCreateRadius, float monsterActiveRadius)
                                        : base(mapDataType, confId, position, rotation)
        {
            MonsterCreateRadius = monsterCreateRadius;
            MonsterActiveRadius = monsterActiveRadius;
        }
    }
}
