/*
 * Description:             MapFoldTypeDisplayData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using System.Collections.Generic;

namespace MapEditor
{
    /// <summary>
    /// MapFoldTypeDisplayData.cs
    /// 地图折叠类型显示数据
    /// </summary>
    [Serializable]
    public class MapFoldTypeDisplayData
    {
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
        /// 带参构造函数
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="foldDes"></param>
        public MapFoldTypeDisplayData(MapFoldType mapFoldType, string foldDes)
        {
            MapFoldType = mapFoldType;
            FoldDes = foldDes;
        }
    }
}
