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
        public MapDataExport MapData;

        /// <summary>
        /// 所有只包含基础数据的动态物体导出数据成员
        /// </summary>
        public List<BaseMapDynamicExport> AllBaseMapObjectExportDatas = new List<BaseMapDynamicExport>();

        /// <summary>
        /// 所有包含碰撞数据的动态物体导出数据成员
        /// </summary>
        public List<ColliderMapDynamicExport> AllColliderMapDynamicExportDatas = new List<ColliderMapDynamicExport>();

        /// <summary>
        /// 所有怪物组的怪物组导出数据列表
        /// </summary>
        public List<MonsterGroupMapDataExport> AllMonsterGroupMapDatas = new List<MonsterGroupMapDataExport>();

        /// <summary>
        /// 所有没有怪物组的怪物导出数据列表
        /// </summary>
        public List<MonsterMapDataExport> ALlNoGroupMonsterMapDatas = new List<MonsterMapDataExport>();

        /// <summary>
        /// 剩余其他(不含怪物和怪物组和出生点)地图埋点导出数据成员
        /// </summary>
        public List<BaseMapDataExport> AllOtherMapDatas = new List<BaseMapDataExport>();
    }
}
