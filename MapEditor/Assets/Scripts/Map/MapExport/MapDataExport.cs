/*
 * Description:             MapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapDataExport.cs
    /// 导出地图相关数据结构定义
    /// </summary>
    [Serializable]
    public class MapDataExport
    {
        /// <summary>
        /// 地图横向大小
        /// </summary>
        public int Width;

        /// <summary>
        /// 地图纵向大小
        /// </summary>
        public int Height;

        /// <summary>
        /// 地图起始位置
        /// </summary>
        public Vector3 StartPos;

        /// <summary>
        /// 出生点位置列表
        /// </summary>
        public List<Vector3> BirthPos = new List<Vector3>();

        public MapDataExport()
        {

        }

        public MapDataExport(int width, int height, Vector3 startPos, List<Vector3> birthPos)
        {
            Width = width;
            Height = height;
            StartPos = startPos;
            BirthPos = birthPos;
        }
    }
}
