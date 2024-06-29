/*
 * Description:             MapObjectSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapObjectSetting.cs
    /// 地图对象设置数据
    /// </summary>
    [Serializable]
    public class MapObjectSetting
    {
        /// <summary>
        /// 地图对象类型配置是否展开
        /// </summary>
        [Header("地图对象类型配置是否展开")]
        public bool IsMapObjectTypeConfigUnfold = false;

        /// <summary>
        /// 所有地图对象类型配置数据
        /// </summary>
        [Header("所有地图对象类型配置数据")]
        [SerializeReference]
        public List<MapObjectTypeConfig> AllMapObjectTypeConfigs = new List<MapObjectTypeConfig>();

        /// <summary>
        /// 所有地图对象配置数据
        /// </summary>
        [Header("所有地图对象配置数据")]
        [SerializeReference]
        public List<MapObjectConfig> AllMapObjectConfigs = new List<MapObjectConfig>();

        /// <summary>
        /// 指定地图对象类型是否是动态类型
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public bool IsDynamicMapObjectType(MapObjectType mapObjectType)
        {
            var findMapObjectTypeConfig = AllMapObjectTypeConfigs.Find(mapObjectTypeConfig => mapObjectTypeConfig.ObjectType == mapObjectType);
            return findMapObjectTypeConfig != null ? findMapObjectTypeConfig.IsDynamic : false;
        }

        /// <summary>
        /// 添加指定地图对象类型配置
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        public bool AddMapObjectTypeConfig(MapObjectType mapObjectType, bool isDynamic = false)
        {
            if(ExistMapObjectTypeConfigByType(mapObjectType))
            {
                Debug.LogError($"已存在地图对象类型:{mapObjectType}的地图对象类型配置数据，添加地图对象类型配置数据失败！");
                return false;
            }
            var mapObjectTypeConfig = new MapObjectTypeConfig(mapObjectType, isDynamic);
            AllMapObjectTypeConfigs.Add(mapObjectTypeConfig);
            return true;
        }

        /// <summary>
        /// 移除指定地图对象类型配置
        /// </summary>
        /// <param name="mapObjectTypeConfig"></param>
        /// <returns></returns>
        public bool RemoveMapObjectTypeConfig(MapObjectTypeConfig mapObjectTypeConfig)
        {
            var result = AllMapObjectTypeConfigs.Remove(mapObjectTypeConfig);
            if(!result)
            {
                Debug.LogError($"找不到地图对象类型:{mapObjectTypeConfig.ObjectType}对应的地图对象类型配置数据，删除地图对象类型配置数据失败！");
            }
            return result;
        }

        /// <summary>
        /// 是否存在指定地图对象类型的地图对象类型配置
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public bool ExistMapObjectTypeConfigByType(MapObjectType mapObjectType)
        {
            var mapObjectTypeConfig = AllMapObjectTypeConfigs.Find(mapObjectConfig => mapObjectConfig.ObjectType == mapObjectType);
            return mapObjectTypeConfig != null;
        }

        /// <summary>
        /// 添加指定地图对象配置数据
        /// </summary>
        /// <param name="mapObjectConfig"></param>
        /// <returns></returns>
        public bool AddMapObjectDataConfig(MapObjectConfig mapObjectConfig)
        {
            if (mapObjectConfig == null)
            {
                Debug.LogError($"不允许添加空的地图对象配置数据，添加地图对象配置数据失败！");
                return false;
            }
            if (ExistMapObjectConfigByUID(mapObjectConfig.UID))
            {
                Debug.LogError($"不允许重复添加地图对象UID:{mapObjectConfig.UID}的配置数据，添加地图对象配置数据失败！");
                return false;
            }
            AllMapObjectConfigs.Add(mapObjectConfig);
            DoSortMapObjectConfigs();
            return true;
        }

        /// <summary>
        /// 移除指定地图对象配置数据
        /// </summary>
        /// <param name="mapObjectConfig"></param>
        /// <returns></returns>
        public bool RemoveMapObjectConfig(MapObjectConfig mapObjectConfig)
        {
            var result = AllMapObjectConfigs.Remove(mapObjectConfig);
            if (result == false)
            {
                Debug.LogError($"找不到地图对象类型:{mapObjectConfig.ObjectType},对象描述:{mapObjectConfig.Des}对应的地图对象配置数据，删除地图对象配置数据失败！");
            }
            return result;
        }

        /// <summary>
        /// 是否已包含指定数据埋点类型的地图对象配置
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public bool ExistMapObjectConfigByUID(int uid)
        {
            return AllMapObjectConfigs.Find(mapDataConfig => mapDataConfig.UID == uid) != null;
        }

        /// <summary>
        /// 执行地图对象配置数据排序
        /// </summary>
        public void DoSortMapObjectConfigs()
        {
            AllMapObjectConfigs.Sort(SortMapObjectConfigs);
        }

        /// <summary>
        /// 地图对象数据排序
        /// </summary>
        /// <param name="mapObjectConfig1"></param>
        /// <param name="mapObjectConfig2"></param>
        /// <returns></returns>
        private int SortMapObjectConfigs(MapObjectConfig mapObjectConfig1, MapObjectConfig mapObjectConfig2)
        {
            return mapObjectConfig1.UID.CompareTo(mapObjectConfig2.UID);
        }

        /// <summary>
        /// 获取指定数据对象UID的地图对象配置
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public MapObjectConfig GetMapObjectConfigByUID(int uid)
        {
            var mapObjectConfig = AllMapObjectConfigs.Find(mapObjectConfig => mapObjectConfig.UID == uid);
            return mapObjectConfig;
        }

        /// <summary>
        /// 获取指定地图对象类型的配置列表
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public List<MapObjectConfig> GetAllMapObjectConfigByType(MapObjectType mapObjectType)
        {
            List<MapObjectConfig> targetMapObjectConfigs = new List<MapObjectConfig>();
            foreach(var mapObjectConfig in AllMapObjectConfigs)
            {
                if(mapObjectConfig.ObjectType == mapObjectType)
                {
                    targetMapObjectConfigs.Add(mapObjectConfig);
                }
            }
            return targetMapObjectConfigs;
        }
    }
}