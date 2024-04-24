/*
 * Description:             MapExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/23
 */

using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapExport.cs
    /// 地图导出数据结构定义
    /// </summary>
    [Serializable]
    public class MapExport
    {
        /// <summary>
        /// 地图导出数据成员
        /// </summary>
        public MapDataExport MapData = new MapDataExport();

        /// <summary>
        /// 所有动态物体导出数据成员
        /// </summary>
        public List<BaseMapDynamicExport> AllDynamicMapObjectDatas = new List<BaseMapDynamicExport>();

        /// <summary>
        /// 所有地图埋点导出数据成员
        /// </summary>
        public List<BaseMapDataExport> AllMapDatas = new List<BaseMapDataExport>();
    }
}
