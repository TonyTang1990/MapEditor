/*
 * Description:             MapUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using UnityEditor;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapUtilities.cs
    /// 地图编辑器工具类
    /// </summary>
    public static class MapUtilities
    {
        /// <summary>
        /// 加载或创建地图配置数据
        /// </summary>
        /// <returns></returns>
        public static MapSetting LoadOrCreateGameMapSetting()
        {
#if UNITY_EDITOR
            var mapSetting = AssetDatabase.LoadAssetAtPath<MapSetting>(MapConst.MapSettingSavePath);
            if(mapSetting == null)
            {
                Debug.Log($"加载MapSetting:{MapConst.MapSettingSavePath}");
            }
            else
            {
                mapSetting = ScriptableObject.CreateInstance<MapSetting>();
                AssetDatabase.CreateAsset(mapSetting, MapConst.MapSettingSavePath);
                AssetDatabase.SaveAssets();
                Debug.Log($"创建MapSetting:{MapConst.MapSettingSavePath}");
            }
            return mapSetting;
#else
            Debug.LogError($"非Editor不允许加载地图配置数据，加载地图配置数据失败！");
            return null;
#endif
        }

        /// <summary>
        /// 创建指定地图埋点数据类型，指定uid和指定位置的埋点数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public static MapData CreateMapDataByType(MapDataType mapDataType, int uid, Vector3 position)
        {
            if (mapDataType == MapDataType.MONSTER)
            {
                return new MonsterMapData(uid, position);
            }
            else if(mapDataType == MapDataType.MONSTER_GROUP)
            {
                return new MonsterGroupMapData(uid, position);
            }
            else
            {
                return new MapData(uid, position);
            }
        }
    }
}