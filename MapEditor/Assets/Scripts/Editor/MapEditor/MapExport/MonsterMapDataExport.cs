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
        /// 怪物组Id
        /// </summary>
        public int GroupId;

        public MonsterMapDataExport(MapDataType mapDataType, int confId, Vector3 position,  Vector3 rotation, int groupId)
                                        : base(mapDataType, confId, position, rotation)
        {
            GroupId = groupId;
        }
    }
}
