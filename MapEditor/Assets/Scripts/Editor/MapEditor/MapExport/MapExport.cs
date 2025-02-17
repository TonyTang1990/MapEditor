/*
 * Description:             MapExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/23
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapExport.cs
    /// 地图导出数据结构定义(统一导出结构定义，方便支持导出不同的数据格式)
    /// </summary>
    [Serializable]
    public class MapExport
    {
        /// <summary>
        /// 地图导出数据成员
        /// </summary>
        public MapDataExport MapData = new MapDataExport();
        
        /// <summary>
        /// 所有怪物导出数据列表
        /// </summary>
        public List<MonsterMapDataExport> ALlMonsterMapDatas = new List<MonsterMapDataExport>();

        /// <summary>
        /// 所有宝箱导出数据列表
        /// </summary>
        public List<TreasureBoxMapDataExport> AllTreasureBoxMapDatas = new List<TreasureBoxMapDataExport>();

        /// <summary>
        /// 所有陷阱导出数据列表
        /// </summary>
        public List<TrapMapDataExport> AllTrapMapDatas = new List<TrapMapDataExport>();

        /// <summary>
        /// 剩余其他地图埋点导出数据成员
        /// </summary>
        public List<BaseMapDataExport> AllOtherMapDatas = new List<BaseMapDataExport>();
    }
}
