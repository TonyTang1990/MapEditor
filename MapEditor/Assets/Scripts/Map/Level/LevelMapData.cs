/*
 * Description:             LevelMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/07/27
 */

using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// LevelMapData.cs
    /// 关卡数据ScriptableObject
    /// </summary>
    public class LevelMapData: ScriptableObject
    {
        /// <summary>
        /// 地图埋点数据列表
        /// </summary>
        [Header("地图埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 根据地图脚本初始化数据
        /// </summary>
        /// <param name="map"></param>
        public void InitDataFromMap(Map map)
        {
            if(map == null)
            {
                Debug.LogError($"不允许传空Map脚本，初始化关卡地图数据失败！");
                return;
            }
            ClearAllMapDatas();
            AddMapDatas(map.MapDataList);
        }

        /// <summary>
        /// 清理所有地图数据
        /// </summary>
        public void ClearAllMapDatas()
        {
            MapDataList.Clear();
        }

        /// <summary>
        /// 添加指定地图埋点数据列表
        /// </summary>
        /// <param name="mapDatas"></param>
        public void AddMapDatas(List<MapData> mapDatas)
        {
            MapDataList.AddRange(mapDatas);
        }

        /// <summary>
        /// 移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveMapDataByIndex(int index)
        {
            var mapDataNum = MapDataList.Count;
            if(index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效范围索引:0-{mapDataNum - 1}，移除关卡地图埋点数据失败！");
                return false;
            }
            MapDataList.RemoveAt(index);
            return true;
        }
    }
}
