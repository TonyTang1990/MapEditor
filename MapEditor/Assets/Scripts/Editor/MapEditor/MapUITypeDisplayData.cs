/*
 * Description:             MapUITypeDisplayData.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/28
 */

using System;
using System.Collections.Generic;

namespace MapEditor
{
    /// <summary>
    /// MapUITypeDisplayData.cs
    /// 地图UI类型显示数据
    /// </summary>
    public class MapUITypeDisplayData
    {
        /// <summary>
        /// 地图UI类型
        /// </summary>
        public MapUIType MapUIType
        {
            get;
            private set;
        }

        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName
        {
            get;
            private set;
        }

        /// <summary>
        /// 标题文本
        /// </summary>
        public string TitleTxt
        {
            get;
            private set;
        }

        /// <summary>
        /// 标题UI宽度
        /// </summary>
        public float TitleUIWidth
        {
            get;
            private set;
        }

        /// <summary>
        /// 标题UI GUI Style
        /// </summary>
        public GUIStyle TitleGUIStyle
        {
            get;
            private set;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="mapUIType"></param>
        /// <param name="propertyName"></param>
        /// <param name="titleTxt"></param>
        /// <param name="titleUIWidth"></param>
        /// <param name="titleGUIStyle"></param>
        public MapUITypeDisplayData(MapUIType mapUIType, string propertyName, string titleTxt, float titleUIWidth, GUIStyle titleGUIStyle)
        {
            MapUIType = mapUIType;
            PropertyName = propertyName;
            TitleTxt = titleTxt;
            TitleUIWidth = titleUIWidth;
            TitleGUIStyle = titleGUIStyle;
        }
    }
}
