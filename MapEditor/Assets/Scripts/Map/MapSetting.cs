/*
 * Description:             MapSetting.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapSetting.cs
    /// </summary>
    public class MapSetting : ScriptableObject
    {
#if UNITY_EDITOR
        /// <summary>
        /// Editor下的地图配置单例对象
        /// </summary>
        private static MapSetting EditorSingleton;

        /// <summary>
        /// 获取Editor地图配置单例对象
        /// </summary>
        /// <returns></returns>
        public static MapSetting GetEditorInstance()
        {
            if(EditorSingleton == null)
            {
                EditorSingleton = MapUtilities.LoadOrCreateGameMapSetting();
            }
            return EditorSingleton;
        }
#endif
        /// <summary>
        /// 默认地图横向大小
        /// </summary>
        [Header("默认地图横向大小")]
        [Range(1, 1000)]
        public int DefaultMapWidth = MapConst.DefaultMapWidth;

        /// <summary>
        /// 默认地图纵向大小
        /// </summary>
        [Header("默认地图纵向大小")]
        [Range(1, 1000)]
        public int DefaultMapHeight = MapConst.DefaultMapHeight;

        [Header("默认区域九宫格大小")]
        [Range(1f, 100f)]
        public float DefaultGridSize = MapConst.DefaultGridSize;

        /// <summary>
        /// 地图对象配置数据
        /// </summary>
        [Header("地图对象配置数据")]
        public MapObjectSetting ObjectSetting = new MapObjectSetting();

        /// <summary>
        /// 地图埋点配置数据
        /// </summary>
        [Header("地图埋点配置数据")]
        public MapDataSetting DataSetting = new MapDataSetting();
    }
}