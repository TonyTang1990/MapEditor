/*
 * Description:             MapDataSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapDataSetting.cs
    /// 地图数据配置数据
    /// </summary>
    [Serializable]
    public class MapDataSetting
    {
        /// <summary>
        /// 所有地图埋点数据配置
        /// </summary>
        [Header("所有地图埋点数据配置")]
        [SerializeReference]
        public List<MapDataConfig> AllMapDataConfigs = new List<MapDataConfig>();

        /// <summary>
        /// 添加指定地图埋点配置数据
        /// </summary>
        /// <param name="mapDataConfig"></param>
        /// <returns></returns>
        public bool AddMapDataConfig(MapDataConfig mapDataConfig)
        {
            if(mapDataConfig == null)
            {
                Debug.LogError($"不允许添加空的地图埋点配置数据，添加地图埋点配置数据失败！");
                return false;
            }
            if(ExistMapDataConfig(mapDataConfig.UID))
            {
                Debug.LogError($"不允许重复添加地图埋点UID:{mapDataConfig.UID}的配置数据，添加地图埋点配置数据失败！");
                return false;
            }
            AllMapDataConfigs.Add(mapDataConfig);
            DoSortMapDataConfigs();
            Debug.Log($"添加地图埋点UID:{mapDataConfig.UID}的配置数据成功！");
            return true;
        }

        /// <summary>
        /// 移除指定地图埋点配置数据
        /// </summary>
        /// <param name="mapDataConfig"></param>
        /// <returns></returns>
        public bool RemoveMapDataConfig(MapDataConfig mapDataConfig)
        {
            var result = AllMapDataConfigs.Remove(mapDataConfig);
            if(result == false)
            {
                Debug.LogError($"找不到地图埋点类型:{mapDataConfig.DataType},埋点描述:{mapDataConfig.Des}对应的地图埋点配置数据，删除地图埋点配置数据失败！");
            }
            return result;
        }

        /// <summary>
        /// 是否已包含指定数据埋点类型的地图埋点配置
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ExistMapDataConfig(int uid)
        {
            return AllMapDataConfigs.Find(mapDataConfig => mapDataConfig.UID == uid) != null;
        }
        
        /// <summary>
        /// 执行地图埋点配置数据排序
        /// </summary>
        public void DoSortMapDataConfigs()
        {
            AllMapDataConfigs.Sort(SortMapDataConfigs);
        }

        /// <summary>
        /// 地图埋点数据排序
        /// </summary>
        /// <param name="mapDataConfig1"></param>
        /// <param name="mapDataConfig2"></param>
        /// <returns></returns>
        private int SortMapDataConfigs(MapDataConfig mapDataConfig1, MapDataConfig mapDataConfig2)
        {
            return mapDataConfig1.UID.CompareTo(mapDataConfig2.UID);
        }

        /// <summary>
        /// 获取指定数据埋点UID的配置
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public MapDataConfig GetMapDataConfigByUID(int uid)
        {
            var mapDataConfig = AllMapDataConfigs.Find(mapDataConfig => mapDataConfig.UID == uid);
            return mapDataConfig;
        }

        /// <summary>
        /// 获取指定地图埋点类型的配置列表
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        public List<MapDataConfig> GetAllMapDataConfigByType(MapDataType mapDataType)
        {
            List<MapDataConfig> targetMapDataConfigs = new List<MapDataConfig>();
            foreach (var mapObjectConfig in AllMapDataConfigs)
            {
                if (mapObjectConfig.DataType == mapDataType)
                {
                    targetMapDataConfigs.Add(mapObjectConfig);
                }
            }
            return targetMapDataConfigs;
        }

        /// <summary>
        /// 获取一个有效的地图埋点UID
        /// </summary>
        /// <returns></returns>
        public int GetAvalibleUID()
        {
            var nextAvalibleUID = 1;
            // 因为AllMapDataConfigs是按UID升序排序
            // 所以按顺序正向匹配不到的UID就表示可以使用
            foreach(var mapDataConfig in AllMapDataConfigs)
            {
                if (mapDataConfig.UID != nextAvalibleUID)
                {
                    break;
                }
                nextAvalibleUID++;
            }
            return nextAvalibleUID;
        }
    }
}