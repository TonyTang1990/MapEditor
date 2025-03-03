/*
 * Description:             BaseMapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// BaseMapDataExport.cs
    /// 地图埋点数据基类导出定义
    /// </summary>
    [Serializable]
    public class BaseMapDataExport
    {
        /// <summary>
        /// 埋点类型
        /// </summary>
        public MapDataType MapDataType;

        /// <summary>
        /// 关联Id
        /// </summary>
        public int ConfId;

        /// <summary>
        /// 位置信息
        /// </summary>
        public Vector3 Position;

        /// <summary>
        /// 旋转信息
        /// </summary>
        public Vector3 Roation;

        public BaseMapDataExport(MapDataType mapDataType, int confId, Vector3 position, Vector3 rotation)
        {
            MapDataType = mapDataType;
            ConfId = confId;
            Position = position;
            Roation = rotation;
        }
    }
}
