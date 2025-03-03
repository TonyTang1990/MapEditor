/*
 * Description:             MapFoldTypeData.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/28
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapFoldTypeData.cs
    /// MapFoldType类型折叠数据
    /// </summary>
    [Serializable]
    public class MapFoldTypeData
    {
        /// <summary>
        /// 游戏地图折叠类型
        /// </summary>
        [HideInInspector]
        public MapFoldType MapFoldType = MapFoldType.MapObjectDataFold;

        /// <summary>
        /// 组展开数据列表
        /// </summary>
        [HideInInspector]
        public List<bool> GroupUnfoldList = new List<bool>();

        public MapFoldTypeData(MapFoldType mapFoldType)
        {
            MapFoldType = mapFoldType;
        }
    }
}
