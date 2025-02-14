/*
 * Description:             MapUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.IO;
using UnityEngine;
using Unity.AI.Navigation;
using System.Threading.Tasks;
using Unity.AI.Navigation.Editor;
using UnityEngine.AI;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
            if(mapSetting != null)
            {
                Debug.Log($"加载MapSetting:{MapConst.MapSettingSavePath}");
            }
            else
            {
                mapSetting = ScriptableObject.CreateInstance<MapSetting>();
                var mapSettingAssetFullPath = PathUtilities.GetAssetFullPath(MapConst.MapSettingSavePath);
                var mapSettingAssetFolderFullPath = Path.GetDirectoryName(mapSettingAssetFullPath);
                if(!Directory.Exists(mapSettingAssetFolderFullPath))
                {
                    Directory.CreateDirectory(mapSettingAssetFolderFullPath);
                }
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
        /// 添加指定地图埋点数据到指定地图埋点数据列表
        /// </summary>
        /// <param name="mapDataList">地图埋点数据列表</param>
        /// <param name="uid">插入地图埋点UID</param>
        /// <param name="startPos">初始位置</param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        public static MapData AddMapDataToList(List<MapData> mapDataList, int uid, Vector3 startPos,
                                                int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            if (mapDataList == null)
            {
                Debug.LogError($"不允许添加埋点数据到空埋点数据列表，添加地图埋点数据到埋点数据列表失败！");
                return null;
            }
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (mapDataConfig == null)
            {
                Debug.LogError($"未配置地图埋点UID:{uid}配置数据，不支持添加此地图埋点数据！");
                return null;
            }
            var mapDataType = mapDataConfig.DataType;
            var mapDataTotalNum = mapDataList.Count;
            var maxInsertIndex = mapDataTotalNum == 0 ? 0 : mapDataTotalNum;
            var insertPos = 0;
            if (insertIndex == -1)
            {
                insertPos = maxInsertIndex;
            }
            else
            {
                insertPos = Math.Clamp(insertIndex, 0, maxInsertIndex);
            }
            var mapDataPosition = startPos;
            var mapDataRotation = mapDataConfig.Rotation;
            if (mapDataTotalNum != 0)
            {
                var insertMapDataPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapData = mapDataList[insertMapDataPos];
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
                if (copyRotation)
                {
                    mapDataRotation = insertMapData != null ? insertMapData.Rotation : mapDataConfig.Rotation;
                }
            }
            if (positionOffset != null)
            {
                mapDataPosition += (Vector3)positionOffset;
            }
            var newMapData = CreateMapDataByType(mapDataType, uid, mapDataPosition, mapDataRotation);
            mapDataList.Insert(insertPos, newMapData);
            return newMapData;
        }

        /// <summary>
        /// 从指定地图埋点数据列表移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="mapDataList">地图埋点数据列表</param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool RemoveMapDataFromListByIndex(List<MapData> mapDataList, int index)
        {
            if (mapDataList == null)
            {
                Debug.LogError($"不允许从空埋点数据列表移除埋点数据，移除指定索引地图埋点数据失败！");
                return false;
            }
            var mapDataNum = mapDataList.Count;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},移除地图埋点数据失败！");
                return false;
            }
            mapDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="mapDataList">地图埋点数据列表</param>
        /// <param name="isOn"></param>
        public static bool UpdateAllMapDataBatchOperationByList(List<MapData> mapDataList, bool isOn)
        {
            if (mapDataList == null)
            {
                Debug.LogError($"不允许更新空埋点数据列表批量操作数据，更新地图埋点批量操作数据失败！");
                return false;
            }
            for (int i = 0, length = mapDataList.Count; i < length; i++)
            {
                mapDataList[i].BatchOperationSwitch = isOn;
            }
            return true;
        }

        /// <summary>
        /// 创建指定地图埋点数据类型，指定uid和指定位置的埋点数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="monsterCreateRadius"></param>
        /// <param name="monsterActiveRadius"></param>
        /// <returns></returns>
        public static MapData CreateMapDataByType(MapDataType mapDataType, int uid, Vector3 position, Vector3 rotation, float monsterCreateRadius, float monsterActiveRadius)
        {
            if (mapDataType == MapDataType.Monster)
            {
                return new MonsterMapData(uid, position, rotation, monsterCreateRadius, monsterActiveRadius);
            }
            else if (mapDataType == MapDataType.MonsterGroup)
            {
                return new MonsterGroupMapData(uid, position, rotation);
            }
            else if (mapDataType == MapDataType.PlayerSpawn)
            {
                return new PlayerSpawnMapData(uid, position, rotation);
            }
            else if (mapDataType == MapDataType.TreasureBox)
            {
                return new TreasureBoxMapData(uid, position, rotation);
            }
            else if (mapDataType == MapDataType.Trap)
            {
                return new TrapMapData(uid, position, rotation);
            }
            else
            {
                Debug.LogWarning($"地图埋点类型:{mapDataType}没有创建自定义类型数据，可能不方便未来扩展！");
                return new MapData(uid, position, rotation);
            }
        }

    }
}