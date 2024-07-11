/*
 * Description:             BaseMapDynamicExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using System.Collections.Generic;
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

        /// <summary>
        /// 旋转信息
        /// </summary>
        public Vector3 Rotation;

        /// <summary>
        /// 缩放信息
        /// </summary>
        public Vector3 LocalScale;

        /// <summary>
        /// 所属九宫格UID列表
        /// </summary>
        public List<int> OwnerGridUIDs;

        public BaseMapDynamicExport(MapObjectType mapObjectType, int confId, Vector3 position, Vector3 rotation, Vector3 localScale, List<int> ownerGridUIDs = null) 
        {
            MapObjectType = mapObjectType;
            ConfId = confId;
            Position = position;
            Rotation = rotation;
            LocalScale = localScale;
            OwnerGridUIDs = ownerGridUIDs;
        }
    }
}
