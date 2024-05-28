/*
 * Description:             ColliderMapDynamicExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/24
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// ColliderMapDynamicExport.cs
    /// 碰撞体场景动态物体数据导出
    /// </summary>
    [Serializable]
    public class ColliderMapDynamicExport : BaseMapDynamicExport
    {
        /// <summary>
        /// 碰撞体中心
        /// </summary>
        public Vector3 ColliderCenter;

        /// <summary>
        /// 碰撞体大小
        /// </summary>
        public Vector3 ColliderSize;

        public ColliderMapDynamicExport(MapObjectType mapObjectType, int confId, Vector3 position,
                                            Vector3 rotation, List<int> ownerGridUIDs, Vector3 colliderCenter, Vector3 colliderSize)
                                                : base(mapObjectType, confId, position, rotation, ownerGridUIDs)
        {
            ColliderCenter = colliderCenter;
            ColliderSize = colliderSize;
        }
    }
}
