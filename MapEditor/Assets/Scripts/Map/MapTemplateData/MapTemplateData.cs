/*
 * Description:             MapTemplateData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/22
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapTemplateData.cs
    /// 地图埋点模版数据ScriptableObject
    /// </summary>
    public class MapTemplateData : ScriptableObject
    {
        /// <summary>
        /// 模版埋点数据列表
        /// </summary>
        [Header("模版埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="initPos">初始位置</param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        public MapData AddMapData(int uid, Vector3 initPos, int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            var newMapData = MapUtilities.AddMapDataToList(MapDataList, uid, initPos, insertIndex, copyRotation, positionOffset);
            return newMapData;
        }

        /// <summary>
        /// 移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveMapDataByIndex(int index)
        {
            var mapDataNum = MapDataList.Count;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},移除地图埋点数据失败！");
                return false;
            }
            MapDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="isOn"></param>
        public bool UpdateAllMapDataBatchOperation(bool isOn)
        {
            return MapUtilities.UpdateAllMapDataBatchOperationByList(MapDataList, isOn);
        }
    }
}