/*
 * Description:             MapObjectTypeConfig.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/29
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapObjectTypeConfig.cs
    /// 地图对象类型信息配置
    /// </summary>
    [Serializable]
    public class MapObjectTypeConfig
    {
        /// <summary>
        /// 地图对象类型
        /// </summary>
        [Header("地图对象类型")]
        public MapObjectType ObjectType;

        /// <summary>
        /// 地图对象类型描述
        /// </summary>
        [Header("地图对象类型描述")]
        public string Des;

        public MapObjectTypeConfig(MapObjectType mapObjectType)
        {
            ObjectType = mapObjectType;
        }
    }
}
