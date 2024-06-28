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
        /// 模板参考位置
        /// </summary>
        [Header("模板参考位置")]
        public Vector3 TemplateReferencePosition;

        /// <summary>
        /// 埋点数据列表
        /// </summary>
        [Header("埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 地图模板策略数据列表
        /// </summary>
        [Header("地图模板策略数据列表")]
        [SerializeReference]
        public List<MapTemplateStrategyData> TemplateStrategyDatas = new List<MapTemplateStrategyData>();

        /// <summary>
        /// 模板策略名数组
        /// </summary>
        [Header("模板策略名数组")]
        public string[] TemplateStrategyNames = new string[0];

        /// <summary>
        /// 模板策略UID数组
        /// </summary>
        [Header("模板策略UID数组")]
        public int[] TemplateStrategyUIDs = new int[0];

        /// <summary>
        /// 根据地图脚本初始化数据
        /// </summary>
        /// <param name="map"></param>
        public void InitDataFromMap(Map map)
        {
            if(map == null)
            {
                Debug.LogError($"不允许传空Map脚本，初始化地图模板数据失败！");
                return;
            }
            ClearAllMapDatas();
            ClearAllMapTemplateStrategyDatas();
            UpdateTemplateReferencePosition(map.TemplateReferencePosition);
            AddMapDatas(map.MapDataList);
            AddMapTemplateStrategyDatas(map.TemplateStrategyDatas);
        }

        /// <summary>
        /// 更新模板参考位置数据
        /// </summary>
        /// <param name="templateReferencePosition"></param>
        public void UpdateTemplateReferencePosition(Vector3 templateReferencePosition)
        {
            TemplateReferencePosition = templateReferencePosition;
            UpdateTemplateLocalDatas();
        }

        /// <summary>
        /// 清理所有埋点数据
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
            UpdateTemplateLocalDatas();
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
                Debug.LogError($"指定索引:{index}不是有效范围:0-{mapDataNum - 1}，移除地图埋模板埋点数据失败！");
                return false;
            }
            MapDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 清除所有埋点模板策略数据
        /// </summary>
        public void ClearAllMapTemplateStrategyDatas()
        {
            TemplateStrategyDatas.Clear();
        }

        /// <summary>
        /// 添加指定地图埋点模板策略数据列表
        /// </summary>
        /// <param name="mapTemplateStrategyDatas"></param>
        public void AddMapTemplateStrategyDatas(List<MapTemplateStrategyData> mapTemplateStrategyDatas)
        {
            TemplateStrategyDatas.AddRange(mapTemplateStrategyDatas);
            UpdateTemplateStrategyOptionDatas();
        }

        /// <summary>
        /// 移除指定索引的地图埋点模板策略数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool RemoveMapTemplateStrategyDataByIndex(int index)
        {
            var mapTemplateStrategyDataNum = TemplateStrategyDatas.Count;
            if(index < 0 || index >= mapTemplateStrategyDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效范围:0-{mapTemplateStrategyDataNum - 1}，移除地图模板埋点策略数据失败！");
                return false;
            }
            TemplateStrategyDatas.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 获取制定模板策略UID的模板策略数据
        /// </summary>
        /// <param name="strategyUID"></param>
        /// <returns></returns>
        public MapTemplateStrategyData GetMapTemlateStrategyDataByUID(int strategyUID)
        {
            foreach(var templateStrategyData in TemplateStrategyDatas)
            {
                if(templateStrategyData.StrategyUID == strategyUID)
                {
                    return templateStrategyData;
                }
            }
            return null;
        }

        /// <summary>
        /// 获取默认策略UID
        /// </summary>
        /// <returns></returns>
        public int GetDefaultStrategyUID()
        {
            if(TemplateStrategyDatas != null && TemplateStrategyDatas.Count > 0)
            {
                return TemplateStrategyDatas[0].StrategyUID;
            }
            return 0;
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="isOn"></param>
        public bool UpdateAllMapDataBatchOperation(bool isOn)
        {
            return MapUtilities.UpdateAllMapDataBatchOperationByList(MapDataList, isOn);
        }

        /// <summary>
        /// 根据指定基准世界坐标和世界旋转更新模板局部数据
        /// </summary>
        private void UpdateTemplateLocalDatas()
        {
            var basePosition = TemplateReferencePosition;
            var baseRotation = Vector3.zero;
            foreach(var mapData in MapDataList)
            {
                var templateLocalPosition = mapData.Position - basePosition;
                var templateLocalRotation = mapData.Rotation - baseRotation;
                mapData.TemplateLocalPosition = templateLocalPosition;
                mapData.TemplateLocalRotation = templateLocalRotation;
            }
        }

        /// <summary>
        /// 更新模板策略选项数据
        /// </summary>
        private void UpdateTemplateStrategyOptionDatas()
        {
            var templateStrategyDataNum = TemplateStrategyDatas.Count;
            TemplateStrategyUIDs = new int[templateStrategyDataNum];
            TemplateStrategyNames = new string[templateStrategyDataNum];
            for(int i = 0; i < templateStrategyDataNum; i++)
            {
                var templateStrategyData = TemplateStrategyDatas[i];
                TemplateStrategyUIDs[i] = templateStrategyData.StrategyUID;
                TemplateStrategyNames[i] = templateStrategyData.StrategyName;
            }
        }
    }
}