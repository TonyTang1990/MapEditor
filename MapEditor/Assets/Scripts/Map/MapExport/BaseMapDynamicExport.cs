/*
 * Description:             BaseMapDynamicExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// BaseMapDynamicExport.cs
    /// 地图动态物体数据基类导出定义
    /// </summary>
    [Serializable]
    public class BaseMapDynamicExport
    {
        /// <summary>
        /// 地图对象类型
        /// </summary>
        public MapObjectType MapObjectType;

        /// <summary>
        /// 关联配置Id
        /// </summary>
        public int ConfId;

        /// <summary>
        /// 位置信息
        /// </summary>
        public Vector3 Position;

        public BaseMapDynamicExport(MapObjectType mapObjectType, int confId, Vector3 position)
        {
            MapObjectType = mapObjectType;
            ConfId = confId;
            Position = position;
        }
    }
}
