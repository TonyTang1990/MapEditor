/*
 * Description:             MapObjectSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

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
        /// 所有地图对象配置数据
        /// </summary>
        [Header("所有地图对象配置数据")]
        [SerializeReference]
        public List<MapObjectConfig> AllMapObjectConfigs = new List<MapObjectConfig>();

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
    }
}