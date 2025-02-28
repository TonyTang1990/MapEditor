/*
 * Description:             MonsterMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using System.Collections.Generic;
using System.Drawing;

namespace MapEditor
{
    /// <summary>
    /// MapDataTypeDisplayData.cs
    /// 地图埋点类型UI显示数据
    /// </summary>
    [Serializable]
    public class MapDataTypeDisplayData
    {
        /// <summary>
        /// 地图埋点类型
        /// </summary>
        public MapDataType MapDataType
        {
            get;
            private set;
        }

        /// <summary>
        /// 地图折叠类型
        /// </summary>
        public MapFoldType MapFoldType
        {
            get;
            private set;
        }

        /// <summary>
        /// 折叠描述
        /// </summary>
        public string FoldDes
        {
            get;
            private set;
        }

        /// <summary>
        /// 显示颜色
        /// </summary>
        public Color DisplayColor
        {
            get;
            private set;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="mapFoldType"></param>
        /// <param name="foldDes"></param>
        /// <param name="displayColor"></param>
        public MapDataTypeDisplayData(MapDataType mapDataType, MapFoldType mapFoldType, string foldDes, Color displayColor)
        {
            MapDataType = mapDataType;
            MapFoldType = mapFoldType;
            FoldDes = foldDes;
            DisplayColor = displayColor;
        }
    }
}
