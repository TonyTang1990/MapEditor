﻿/*
 * Description:             MapEditorUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Cinemachine;
using Unity.AI.Navigation;
using Unity.AI.Navigation.Editor;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace MapEditor
{
    /// <summary>
    /// MapEditorUtilities.cs
    /// 地图编辑器Editor工具类
    /// </summary>
    public static class MapEditorUtilities
    {
        /// <summary>
        /// 上一次触发游戏地图复制菜单时间戳(解决多选时MenuItem方法进多次导致问题)
        /// </summary>
        private static float LastDumplicateMenuCallTimestamp = 0f;

        /// <summary>
        /// 复制的地图对象数据列表
        /// </summary>
        private static List<MapObjectData> DumplicatedMapObjectDatas = new List<MapObjectData>();

        /// <summary>
        /// 游戏地图对象复制菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapObjectData/Dumplicate #&D", false, 0)]
        static void MapObjectDataDuplicate(MenuCommand menuCommand)
        {
            Debug.Log($"MapObjectDataDuplicate()");
            if(Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                //Debug.Log($"同一时刻触发多次MapObjectDataDuplicate()，忽略调用！");
                return;
            }
            var targetGameObjects = Selection.objects;
            if(targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return;
            }
            var selectionNum = targetGameObjects.Length;
            Debug.Log($"选中对象数量:{selectionNum}");
            LastDumplicateMenuCallTimestamp = Time.unscaledTime;
            Debug.Log($"LastDumplicateMenuCallTimestamp:{LastDumplicateMenuCallTimestamp}");
            DumplicatedMapObjectDatas.Clear();
            for(int i = 0; i < targetGameObjects.Length; i++)
            {
                var mapObjectData = TryDumplicateMapObject(targetGameObjects[i] as GameObject);
                if(mapObjectData != null)
                {
                    DumplicatedMapObjectDatas.Add(mapObjectData);
                }
            }
            // 重新选中复制出来的实体对象列表
            var dumplicatedNum = DumplicatedMapObjectDatas.Count;
            if(dumplicatedNum == 0)
            {
                var selectionGos = new UnityEngine.Object[dumplicatedNum];
                for(int i = 0; i < dumplicatedNum; i++)
                {
                    selectionGos[i] = DumplicatedMapObjectDatas[i].Go;
                }
                Selection.objects = selectionGos;
            }
        }

        /// <summary>
        /// 游戏地图对象复制菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapObjectData/Dumplicate #&D", true, 0)]
        static bool MapObjectDataDumplicateValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapObjectDumplicateValidateFunction()");
            return CheckSelfOrParentHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 游戏地图对象删除菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapObjectData/Remove #&R", false, 0)]
        static void MapObjectDataRemove(MenuCommand menuCommand)
        {
            Debug.Log($"MapObjectDataRemove()");
            if(Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                Debug.Log($"同一时刻触发多次MapObjectDataRemove()，忽略调用！");
                return;
            }
            var targetGameObjects = Selection.objects;
            if(targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return;
            }
            var selectionNum = targetGameObjects.Length;
            Debug.Log($"选中对象数量:{selectionNum}");
            LastDumplicateMenuCallTimestamp = Time.unscaledTime;
            Debug.Log($"LastDumplicateMenuCallTimestamp:{LastDumplicateMenuCallTimestamp}");
            for(int i = 0; i < targetGameObjects.Length; i++)
            {
                TryRemoveMapObject(targetGameObjects[i] as GameObject);
            }
        }

        /// <summary>
        /// 游戏地图对象删除菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapObjectData/Remove #&R", true, 0)]
        static bool MapObjectDataRemoveValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapObjectRemoveValidateFunction()");
            return CheckSelfOrParentHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 检查指定对象数组是否自身或父节点包含Map脚本
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static bool CheckSelfOrParentHasMapScript(UnityEngine.Object[] objects)
        {
            var targetGameObjects = objects;
            if(targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return false;
            }
            foreach(var targetGameObject in targetGameObjects)
            {
                if(targetGameObject is GameObject targetGo)
                {
                    var map = targetGo.GetComponentInParent<Map>(true);
                    if(map != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 游戏地图埋点复制菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapData/Dumplicate #&Q", false, 0)]
        static void MapDataDuplicate(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataDuplicate()");
            if (Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                //Debug.Log($"同一时刻触发多次MapDataDuplicate()，忽略调用！");
                return;
            }
            var targetGameObjects = Selection.objects;
            if (targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return;
            }
            var selectionNum = targetGameObjects.Length;
            Debug.Log($"选中对象数量:{selectionNum}");
            LastDumplicateMenuCallTimestamp = Time.unscaledTime;
            Debug.Log($"LastDumplicateMenuCallTimestamp:{LastDumplicateMenuCallTimestamp}");
            for (int i = 0; i < targetGameObjects.Length; i++)
            {
                TryDumplicateMapData(targetGameObjects[i] as GameObject);
            }
        }

        /// <summary>
        /// 游戏地图埋点复制菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapData/Dumplicate #&Q", true, 0)]
        static bool MapDataDumplicateValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataDumplicateValidateFunction()");
            return CheckSelfHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 游戏地图埋点删除菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapData/Remove #&W", false, 0)]
        static void MapDataRemove(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataRemove()");
            if (Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                Debug.Log($"同一时刻触发多次MapDataRemove()，忽略调用！");
                return;
            }
            var targetGameObjects = Selection.objects;
            if (targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return;
            }
            var selectionNum = targetGameObjects.Length;
            Debug.Log($"选中对象数量:{selectionNum}");
            LastDumplicateMenuCallTimestamp = Time.unscaledTime;
            Debug.Log($"LastDumplicateMenuCallTimestamp:{LastDumplicateMenuCallTimestamp}");
            for (int i = 0; i < targetGameObjects.Length; i++)
            {
                TryRemoveMapData(targetGameObjects[i] as GameObject);
            }
        }

        /// <summary>
        /// 游戏地图埋点删除菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapData/Remove #&W", true, 0)]
        static bool MapDataRemoveValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataRemoveValidateFunction()");
            return CheckSelfHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 检查指定对象数组是否自身包含Map脚本
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static bool CheckSelfHasMapScript(UnityEngine.Object[] objects)
        {
            var targetGameObjects = objects;
            if (targetGameObjects == null || targetGameObjects.Length == 0)
            {
                return false;
            }
            foreach (var targetGameObject in targetGameObjects)
            {
                if (targetGameObject is GameObject targetGo)
                {
                    var map = targetGo.GetComponent<Map>();
                    if (map != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 尝试实例化指定地图对象实例对象
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static MapObjectData TryDumplicateMapObject(GameObject go)
        {
            if(go == null)
            {
                return null;
            }
            var map = go.GetComponentInParent<Map>(true);
            if(map == null)
            {
                Debug.LogError($"选中对象:{go.name}或其父节点里找不到Map脚本，Map地图对象复制失败！");
                return null;
            }
            var mapObjectDataIndex = map.GetMapObjectDataIndexByGo(go);
            if(mapObjectDataIndex == -1)
            {
                Debug.LogError($"选中对象不属于地图对象数据实体，Map地图对象复制失败！");
                return null;
            }
            var mapObjectData = map.GetMapObjectDataByIndex(mapObjectDataIndex);
            Debug.Log($"当前复制选中对象:{go.name}所处地图对象数据索引:{mapObjectDataIndex}");
            return map.DoAddMapObjectData(mapObjectData.UID, mapObjectDataIndex, true);
        }

        /// <summary>
        /// 尝试删除指定地图对象实例对象
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static bool TryRemoveMapObject(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            var map = go.GetComponentInParent<Map>(true);
            if (map == null)
            {
                Debug.LogError($"选中对象:{go.name}或其父节点里找不到Map脚本，Map地图对象移除失败！");
                return false;
            }
            var mapObjectDataIndex = map.GetMapObjectDataIndexByGo(go);
            if (mapObjectDataIndex == -1)
            {
                Debug.LogError($"选中对象不属于地图对象数据实体，Map地图对象移除失败！");
                return false;
            }
            Debug.Log($"当前移除选中对象:{go.name}所处地图对象数据索引:{mapObjectDataIndex}");
            return map.DoRemoveMapObjectDataByIndex(mapObjectDataIndex);
        }

        /// <summary>
        /// 尝试复制指定对象的地图批量埋点数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static List<MapData> TryDumplicateMapData(GameObject go)
        {
            if (go == null)
            {
                return null;
            }
            var map = go.GetComponent<Map>();
            if (map == null)
            {
                Debug.LogError($"选中对象:{go.name}没有Map脚本，不应该进入这里，游戏地图批量选中埋点数据复制失败！");
                return null;
            }
            var dumplicatedMapDatas = new List<MapData>();
            // 倒叙确保插入后也能真正完成所有地图埋点数据的遍历
            // 默认插入的新数据是未勾选批量的，所以不会无限增加
            for(int i = map.MapDataList.Count - 1; i >= 0; i--)
            {
                var mapData = map.MapDataList[i];
                if(mapData.BatchOperationSwitch)
                {
                    var newMapData = map.DoAddMapData(mapData.UID, i, true, MapEditorConst.MapDataDuplicatePositionOffset);
                    if(newMapData != null)
                    {
                        // 复制自定义数据是复制所有数据，这里需要记录位置并还原复制的最新位置
                        var newPosition = newMapData.Position;
                        dumplicatedMapDatas.Add(newMapData);
                        newMapData.CopyCustomData(mapData);
                        newMapData.Position = newPosition;
                        Debug.Log($"选中对象:{go.name}复制勾选批量的地图埋点索引:{i}数据！");
                    }
                }
            }
            map.UpdateAllMapDataBatchOperation(false);
            foreach(var newMapData in dumplicatedMapDatas)
            {
                newMapData.BatchOperationSwitch = true;
            }
            return dumplicatedMapDatas;
        }

        /// <summary>
        /// 尝试删除指定地图对象批量选中地图数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static bool TryRemoveMapData(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            var map = go.GetComponent<Map>();
            if (map == null)
            {
                Debug.LogError($"当前选中对象:{go.name}没有Map脚本，不应该进入这里，游戏地图批量选中埋点数据移除失败！");
                return false;
            }
            // 倒叙确保删除后也能真正完成所有地图埋点数据的遍历
            for(int i = map.MapDataList.Count - 1; i >= 0; i--)
            {
                var mapData = map.MapDataList[i];
                if(mapData.BatchOperationSwitch)
                {
                    map.DoRemoveMapDataByIndex(i);
                    Debug.Log($"选中对象:{go.name}删除勾选批量的地图索引:{i}数据！");
                }
            }
            return true;
        }

        /// <summary>
        /// 更新清除指定对象的批量地图埋点选中数据
        /// </summary>
        /// <param name="go"></param>
        /// <param name="isSelect"></param>
        /// <returns></returns>
        private static bool UpdateAllBatch(GameObject go, bool isSelect = false)
        {
            if (go == null)
            {
                return false;
            }
            var map = go.GetComponent<Map>();
            if (map == null)
            {
                Debug.LogError($"当前选中对象:{go.name}没有Map脚本，不应该进入这里，游戏地图埋点移除所有批量选中数据失败！");
                return false;
            }
            map.UpdateAllMapDataBatchOperation(isSelect);
            return true;
        }

        /// <summary>
        /// 获取指定地图埋点属性列表和指定索引的标签显示名
        /// Note:
        /// 直接传递mapDataConfig匹配mapDataPropertyList对应索引的配置来减少不必要的FindPropertyRelative调用
        /// </summary>
        /// <param name="mapDataConfig"></param>
        /// <param name="mapDataPropertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMapDataPropertyLabelName(MapDataConfig mapDataConfig, SerializedProperty mapDataPropertyList, int index)
        {
            if(mapDataConfig == null)
            {
                return "无效配置";
            }
            if(mapDataPropertyList == null)
            {
                return "无效的地图属性";
            }
            if(!mapDataPropertyList.isArray)
            {
                return "非数组地图属性";
            }
            var mapDataDes = mapDataConfig.Des;
            return $"[{index}]{mapDataDes}";
        }

        /// <summary>
        /// 获取指定地图埋点数据，指定索引和指定埋点模板策略数据的标签显示名
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMapDataLabelName(MapData mapData, int index)
        {
            if (mapData == null)
            {
                return "无效的地图数据";
            }
            var uid = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (mapDataConfig == null)
            {
                return $"未知的UID:{uid}";
            }
            var mapDataDes = mapDataConfig.Des;
            return $"[{index}]{mapDataDes}";
        }

        /// <summary>
        /// 获取或创建指定地图地形节点
        /// </summary>
        /// <param name="mapGO"></param>
        /// <returns></returns>
        public static Transform GetOrCreateMapTerrianNode(GameObject mapGO)
        {
            if (mapGO == null)
            {
                Debug.LogError($"不允许传空地图GameObject，获取或创建地图地块节点失败！");
                return null;
            }
            var mapTerrianNodeName = MapConst.MapTerrianNodeName;
            var mapTerrianNodeTransform = mapGO.transform.Find(mapTerrianNodeName);
            if (mapTerrianNodeTransform == null)
            {
                var mapTerrianPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(MapConst.DetaulMapTerrianPrefabPath);
                if(mapTerrianPrefab == null)
                {
                    return null;
                }
                var mapTerrianNodeGo = PrefabUtility.InstantiatePrefab(mapTerrianPrefab) as GameObject;
                mapTerrianNodeTransform = mapTerrianNodeGo.transform;
                mapTerrianNodeTransform.name = mapTerrianNodeName;
                mapTerrianNodeTransform.SetParent(mapGO.transform);
                // 默认隐藏创建，寻路烘焙时显示，烘焙完再隐藏，避免地形被错误看到问题
                mapTerrianNodeGo.SetActive(false);
            }
            return mapTerrianNodeTransform;
        }

        /// <summary>
        /// 获取制定Asset的预览纹理
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static Texture2D GetAssetPreview(UnityEngine.Object asset)
        {
            if(asset == null)
            {
                return null;
            }
            return AssetPreview.GetAssetPreview(asset);
        }

        /// <summary>
        /// 获取指定地图埋点类型的默认颜色信息
        /// </summary>
        /// <param name="dataType"></param>
        /// <returns></returns>
        public static Color GetMapDataColor(MapDataType dataType)
        {
            Color color;
            if(MapDataTypeColorMap.TryGetValue(dataType, out color))
            {
                return color;
            }
            return Color.grey;
        }

        /// <summary>
        /// 获取所有地图预制件Asset路径
        /// </summary>
        /// <returns></returns>
        public static List<string> GetAllMapAssetPath()
        {
            var allMapAssetsPath = new List<string>();
            var searchFolders = new string[1] { MapEditorConst.MapPrefabFolderRelativePath };
            var allPrefabGUIDsPath = AssetDatabase.FindAssets("t:Prefab", searchFolders);
            foreach(var prefabGUID in allPrefabGUIDsPath)
            {
                var prefabPath = AssetDatabase.GUIDToAssetPath(prefabGUID);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                if(prefab == null)
                {
                    continue;
                }
                var map = prefab.GetComponent<Map>();
                if(map == null)
                {
                    continue;
                }
                allMapAssetsPath.Add(prefabPath);
            }
            return allMapAssetsPath;
        }

        /// <summary>
        /// 指定目标int值是否在指定值数组范围内
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIntValueInArrays(int targetValue, int[] values)
        {
            if (values == null || values.Length == 0)
            {
                return false;
            }
            foreach (var value in values)
            {
                if (targetValue == value)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取制定数据索引的折叠数据索引
        /// Note:
        /// 默认折叠数据分组和折叠数据索引一致
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public static int GetMapFoldIndex(int index)
        {
            return index / MapEditorConst.MapFoldNumLimit;
        }

        /// <summary>
        /// 获取指定地图对象类型的选项数据
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public static (string[] options, int[] values) GetMapObjectChoiceInfosByType(MapObjectType mapObjectType)
        {
            var objectSetting = MapSetting.GetEditorInstance().ObjectSetting;
            var mapObjectTypeAllConfigs = objectSetting.GetAllMapObjectConfigByType(mapObjectType);
            var mapObjectTypeConfigNum = mapObjectTypeAllConfigs.Count;
            string[] allChoiceOptions = new string[mapObjectTypeConfigNum];
            int[] allValueOptions = new int[mapObjectTypeConfigNum];
            for (int i = 0; i < mapObjectTypeConfigNum; i++)
            {
                var mapObjectTypeAllConfig = mapObjectTypeAllConfigs[i];
                allChoiceOptions[i] = mapObjectTypeAllConfig.GetOptionName();
                allValueOptions[i] = mapObjectTypeAllConfig.UID;
            }
            return (allChoiceOptions, allValueOptions);
        }

        /// <summary>
        /// 获取指定地图埋点类型的选项数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        public static (string[] options, int[] values) GetMapDataChoiceInfosByType(MapDataType mapDataType)
        {
            var dataSetting = MapSetting.GetEditorInstance().DataSetting;
            var mapDataTypeAllConfigs = dataSetting.GetAllMapDataConfigByType(mapDataType);
            var mapDataTypeConfigNum = mapDataTypeAllConfigs.Count;
            string[] allChoiceOptions = new string[mapDataTypeConfigNum];
            int[] allValueOptions = new int[mapDataTypeConfigNum];
            for (int i = 0; i < mapDataTypeConfigNum; i++)
            {
                var mapObjectTypeAllConfig = mapDataTypeAllConfigs[i];
                allChoiceOptions[i] = mapObjectTypeAllConfig.GetOptionName();
                allValueOptions[i] = mapObjectTypeAllConfig.UID;
            }
            return (allChoiceOptions, allValueOptions);
        }

        /// <summary>
        /// 将指定属性对象和指定索引的数据向上移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool MovePropertyDataUpByIndex(SerializedProperty propertyList, int index)
        {
            if (propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，向上移动属性数据失败！");
                return false;
            }
            var mapDataNum = propertyList.arraySize;
            if (index < 0 || index > mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum},向上移动属性数据失败！");
                return false;
            }
            var newIndex = Math.Clamp(index - 1, 0, mapDataNum);
            ExchangeMapDataByIndex(propertyList, index, newIndex);
            return true;
        }

        /// <summary>
        /// 将指定属性对象和指定索引的数据向下移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool MovePropertyDataDownByIndex(SerializedProperty propertyList, int index)
        {
            if (propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，向下移动属性数据失败！");
                return false;
            }
            var mapDataNum = propertyList.arraySize;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},向下移动属性数据失败！");
                return false;
            }
            var newIndex = Math.Clamp(index + 1, 0, mapDataNum - 1);
            ExchangeMapDataByIndex(propertyList, index, newIndex);
            return true;
        }

        /// <summary>
        /// 交换指定属性和交换索引数据
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="exchangeIndex1"></param>
        /// <param name="exchangeIndex2"></param>
        /// <returns></returns>
        public static bool ExchangeMapDataByIndex(SerializedProperty propertyList, int exchangeIndex1, int exchangeIndex2)
        {
            if (propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，交换属性数据位置失败！");
                return false;
            }
            if (exchangeIndex1 == exchangeIndex2)
            {
                return true;
            }
            var dataNum = propertyList.arraySize;
            if (exchangeIndex1 < 0 || exchangeIndex2 >= dataNum || exchangeIndex2 < 0 || exchangeIndex2 >= dataNum)
            {
                Debug.LogError($"指定交换索引1:{exchangeIndex1}或交换索引2:{exchangeIndex2}不是有效索引范围:0-{dataNum - 1},交换属性数据位置失败！");
                return false;
            }
            var mapDataIndex2Property = propertyList.GetArrayElementAtIndex(exchangeIndex2);
            var mapData2 = mapDataIndex2Property.managedReferenceValue;
            var mapDataIndex1Property = propertyList.GetArrayElementAtIndex(exchangeIndex1);
            var mapData1 = mapDataIndex1Property.managedReferenceValue;
            mapDataIndex1Property.managedReferenceValue = mapData2;
            mapDataIndex2Property.managedReferenceValue = mapData1;
            //Debug.Log($"交换属性数据索引1:{exchangeIndex1}和索引2:{exchangeIndex2}成功！");
            return true;
        }

        /// <summary>
        /// 获取或创建指定地图GameObject的指定地图对象类型挂在节点
        /// </summary>
        /// <param name="mapGO"></param>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public static Transform GetOrCreateMapObjectTypeParentNode(GameObject mapGO, MapObjectType mapObjectType)
        {
            if (mapGO == null)
            {
                Debug.LogError($"不允许传空地图GameObject，获取或创建指定地图对象类型的父挂在节点失败！");
                return null;
            }
            var mapObjectParentNode = GetOrCreateMapObjectParentNode(mapGO);
            var mapObjectTypeParentNodeName = GetMapObjectTypeParentNodeName(mapObjectType);
            var mapObjectTypeParentNodeTransform = mapObjectParentNode.Find(mapObjectTypeParentNodeName);
            if (mapObjectTypeParentNodeTransform == null)
            {
                mapObjectTypeParentNodeTransform = new GameObject(mapObjectTypeParentNodeName).transform;
                mapObjectTypeParentNodeTransform.SetParent(mapObjectParentNode);
            }
            return mapObjectTypeParentNodeTransform;
        }

        /// <summary>
        /// 获取或创建指定地图GameObject的地图对象父挂点
        /// </summary>
        /// <param name="mapGO"></param>
        /// <returns></returns>
        public static Transform GetOrCreateMapObjectParentNode(GameObject mapGO)
        {
            if (mapGO == null)
            {
                Debug.LogError($"不允许传空地图GameObject，获取或创建地图对象父挂点失败！");
                return null;
            }
            var mapObjectParentTransform = mapGO.transform.Find(MapConst.MapObjectParentNodeName);
            if (mapObjectParentTransform == null)
            {
                mapObjectParentTransform = new GameObject(MapConst.MapObjectParentNodeName).transform;
                mapObjectParentTransform.SetParent(mapGO.transform);
            }
            return mapObjectParentTransform;
        }

        /// <summary>
        /// 获取制定地图对象类型的父节点挂点名
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        public static string GetMapObjectTypeParentNodeName(MapObjectType mapObjectType)
        {
            var parentNodeName = Enum.GetName(MapConst.MapObjectType, mapObjectType);
            return parentNodeName;
        }

        /// <summary>
        /// 指定GameObject添加或更新指定地图对象UID的MapObjectDataMono数据
        /// </summary>
        /// <param name="go"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static MapObjectDataMono AddOrUpdateMapObjectDataMono(GameObject go, int uid)
        {
            if (go == null)
            {
                Debug.LogError($"不允许给空GameObject添加MapObjectDataMono脚本！");
                return null;
            }
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到UID:{uid}的地图对象配置数据，GameObject:{go.name}添加MapObjectDataMono脚本失败！");
                return null;
            }
            var mapObjectDataMono = go.GetComponent<MapObjectDataMono>();
            if (mapObjectDataMono == null)
            {
                mapObjectDataMono = go.AddComponent<MapObjectDataMono>();
            }
            mapObjectDataMono.UID = uid;
            mapObjectDataMono.ObjectType = mapObjectConfig.ObjectType;
            mapObjectDataMono.ConfId = mapObjectConfig.ConfId;
            return mapObjectDataMono;
        }

        /// <summary>
        /// 指定GameObject根据指定碰撞器数据更新
        /// </summary>
        /// <param name="go"></param>
        /// <param name="center"></param>
        /// <returns></returns>
        public static void UpdateColliderByColliderData(GameObject go, Vector3 center, Vector3 size, float radius = 0f)
        {
            if (go == null)
            {
                return;
            }
            var boxCollider = go.GetComponent<BoxCollider>();
            // 没有碰撞体有ColliderDataMono则根据对应信息更新碰撞器数据
            // 反之则根据传入的碰撞体数据初始化碰撞体数据
            if (boxCollider == null)
            {
                var colliderDataMono = go.GetComponent<ColliderDataMono>();
                if (colliderDataMono == null)
                {
                    return;
                }
                boxCollider = go.AddComponent<BoxCollider>();
                boxCollider.center = colliderDataMono.Center;
                boxCollider.size = colliderDataMono.Size;
            }
            else
            {
                if (boxCollider != null)
                {
                    boxCollider.center = center;
                    boxCollider.size = size;
                }
            }
        }

        /// <summary>
        /// 指定GameObject根据挂载的ColliderDataMono更新碰撞体数据
        /// </summary>
        /// <param name="go"></param>
        public static void AddOrUpdateColliderByColliderDataMono(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            var colliderDataMono = go.GetComponent<ColliderDataMono>();
            if (colliderDataMono == null)
            {
                // 没有统一添加矩形碰撞体
                go.GetOrAddComponet<BoxCollider>();
            }
            else
            {
                var boxCollider = go.GetOrAddComponet<BoxCollider>();
                boxCollider.center = colliderDataMono.Center;
                boxCollider.size = colliderDataMono.Size;
            }
        }

        /// <summary>
        /// 指定GameObject根据挂载的ColliderDataMono更新碰撞体数据
        /// </summary>
        /// <param name="go"></param>
        public static void UpdateColliderByColliderDataMono(GameObject go)
        {
            if (go == null)
            {
                return;
            }
            var colliderDataMono = go.GetComponent<ColliderDataMono>();
            if (colliderDataMono == null)
            {
                return;
            }
            var boxCollider = go.GetOrAddComponet<BoxCollider>();
            boxCollider.center = colliderDataMono.Center;
            boxCollider.size = colliderDataMono.Size;
        }

        /// <summary>
        /// 获取当前脚本GameObject状态
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static GameObjectStatus GetGameObjectStatus(GameObject go)
        {
            if (go == null)
            {
                Debug.LogError($"传入空GameObject，获取GameObject状态失败！");
                return GameObjectStatus.INVALIDE;
            }
            // 未做成预制件的所有操作可用
            // 做成预制件的必须进入预制件编辑模式才可行
            var assetPath = AssetDatabase.GetAssetPath(go);
            if (!string.IsNullOrEmpty(assetPath))
            {
                return GameObjectStatus.Asset;
            }
            if (UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(go) != null)
            {
                return GameObjectStatus.PrefabContent;
            }
            else
            {
                if (PrefabUtility.IsPartOfPrefabInstance(go))
                {
                    return GameObjectStatus.PrefabInstance;
                }
            }
            return GameObjectStatus.Normal;
        }

        /// <summary>
        /// 操作是否可用
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool IsOperationAvalible(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            var gameObjectStatus = GetGameObjectStatus(go);
            if (gameObjectStatus == GameObjectStatus.Normal || gameObjectStatus == GameObjectStatus.PrefabContent)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 尝试打开预制件编辑模式
        /// </summary>
        /// <param name="go"></param>
        public static void TryOpenPrefabContent(GameObject go)
        {
            var gameObjectStatus = GetGameObjectStatus(go);
            if (gameObjectStatus == GameObjectStatus.INVALIDE || gameObjectStatus == GameObjectStatus.Normal || gameObjectStatus == GameObjectStatus.PrefabContent)
            {
                return;
            }
            var prefabAssetPath = string.Empty;
            if (gameObjectStatus == GameObjectStatus.Asset)
            {
                prefabAssetPath = AssetDatabase.GetAssetPath(go);
            }
            else if (gameObjectStatus == GameObjectStatus.PrefabInstance)
            {
                var prefabAsset = PrefabUtility.GetCorrespondingObjectFromSource(go);
                prefabAssetPath = AssetDatabase.GetAssetPath(prefabAsset);
            }
            if (string.IsNullOrEmpty(prefabAssetPath))
            {
                return;
            }
            UnityEditor.SceneManagement.PrefabStageUtility.OpenPrefab(prefabAssetPath);
        }

        /// <summary>
        /// 检查操作是否可用
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool CheckOperationAvalible(GameObject go)
        {
            if (go == null)
            {
                return false;
            }
            if (!IsOperationAvalible(go))
            {
                var gameObjectStatus = GetGameObjectStatus(go);
                EditorUtility.DisplayDialog("地图编辑器", $"当前操作对象处于:{gameObjectStatus.ToString()}状态下不允许操作！", "确认");
                TryOpenPrefabContent(go);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取或创建指定地图GameObject的寻路组件
        /// </summary>
        /// <param name="mapGO"></param>
        /// <returns></returns>
        public static NavMeshSurface GetOrCreateNavMeshSurface(GameObject mapGO)
        {
            if (mapGO == null)
            {
                Debug.LogError($"不允许传空地图GameObject，获取或创建指定地图寻路组件失败！");
                return null;
            }
            var navMeshSurface = mapGO.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                navMeshSurface = mapGO.AddComponent<NavMeshSurface>();
            }
            navMeshSurface.collectObjects = CollectObjects.Children;
            navMeshSurface.useGeometry = NavMeshCollectGeometry.RenderMeshes;
            navMeshSurface.layerMask = LayerMask.GetMask("NavigationBake");
            return navMeshSurface;
        }

        /// <summary>
        /// 获取指定地图GameObject的游戏虚拟摄像机节点
        /// </summary>
        /// <param name="mapGo"></param>
        /// <returns></returns>
        public static Transform GetGameVirtualCameraNode(GameObject mapGo)
        {
            if(mapGo == null)
            {
                Debug.LogError($"不允许传空地图GameObject,获取指定地图游戏虚拟摄像机节点失败！");
                return null;
            }
            var gameVirtualCameraNodeRelativePath = MapUtilities.GetGameVirtualCameraNodeRelativePath();
            return mapGo.transform.Find(gameVirtualCameraNodeRelativePath);
        }

        /// <summary>
        /// 获取或创建指定地图GameObject游戏虚拟摄像机节点
        /// </summary>
        /// <param name="mapGo"></param>
        /// <returns></returns>
        public static Transform GetOrCreateGameVirtualCameraNode(GameObject mapGo)
        {
            if(mapGo == null)
            {
                Debug.LogError($"不允许传空地图GameObject,获取或创建指定地图游戏虚拟摄像机节点失败！");
                return null;
            }
            var gameVirtualCameraParentNodeName = MapConst.GameVirtualCameraParentNodeName;
            var gameVirtualCameraParentTransform = mapGo.transform.Find(gameVirtualCameraParentNodeName);
            Transform virtualCameraNodeTransform;
            if (gameVirtualCameraParentTransform == null)
            {
                var virtualCameraParentNodeGo = new GameObject(MapConst.GameVirtualCameraParentNodeName);
                var virtualCameraParentTransform = virtualCameraParentNodeGo.transform;
                virtualCameraParentTransform.SetParent(mapGo.transform);
                var virtualCameraNodeGo = new GameObject(MapConst.GameVirtualCameraNodeName);
                virtualCameraNodeTransform = virtualCameraNodeGo.transform;
                virtualCameraNodeTransform.SetParent(virtualCameraParentTransform);
                var virtualCamera = virtualCameraNodeGo.AddComponent<CinemachineVirtualCamera>();
                virtualCamera.Priority = MapConst.GameVirtualCameraPriority;
                virtualCamera.m_Lens.FarClipPlane = 500f;
                virtualCameraNodeTransform.position = MapConst.GameVirtualCameraDefaultPos;
                virtualCameraNodeTransform.eulerAngles = MapConst.GameVirtualCameraDefaultRot;
            }
            else
            {
                virtualCameraNodeTransform = GetGameVirtualCameraNode(mapGo);
            }
            return virtualCameraNodeTransform;
        }

        /// <summary>
        /// 烘焙寻路任务
        /// </summary>
        /// <param name="navMeshSurface"></param>
        /// <returns></returns>
        public static async Task<bool> BakePathTask(NavMeshSurface navMeshSurface)
        {
            var navMeshSurfaces = new UnityEngine.Object[] { navMeshSurface };
            var navMeshDataAssetPath = navMeshSurface.navMeshData != null ? AssetDatabase.GetAssetPath(navMeshSurface.navMeshData) : null;
            if (!string.IsNullOrEmpty(navMeshDataAssetPath))
            {
                NavMeshAssetManager.instance.ClearSurfaces(navMeshSurfaces);
                AssetDatabase.DeleteAsset(navMeshDataAssetPath);
                // 确保删除成功
                while (AssetDatabase.LoadAssetAtPath<NavMeshData>(navMeshDataAssetPath) != null)
                {
                    await Task.Delay(1);
                }
            }
            NavMeshAssetManager.instance.StartBakingSurfaces(navMeshSurfaces);
            while (NavMeshAssetManager.instance.IsSurfaceBaking(navMeshSurface))
            {
                await Task.Delay(1);
            }
            AssetDatabase.SaveAssets();
            return true;
        }

        /// <summary>
        /// 拷贝寻路数据Asset
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static async Task<bool> CopyNavMeshAsset(GameObject gameObject)
        {
            var navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
            {
                Debug.LogError($"地图:{gameObject.name}找不到寻路NavMeshSurface组件，拷贝寻路数据Asset失败！");
                return false;
            }
            var mapAssetPath = GetMapAssetPath(gameObject);
            if (string.IsNullOrEmpty(mapAssetPath))
            {
                Debug.LogError($"地图:{gameObject.name}未保存成任何本地Asset，复制寻路数据Asset失败！");
                return false;
            }
            var navMeshAssetPath = AssetDatabase.GetAssetPath(navMeshSurface.navMeshData);
            if (navMeshSurface.navMeshData == null || string.IsNullOrEmpty(navMeshAssetPath))
            {
                Debug.LogError($"地图:{gameObject.name}未烘焙任何有效寻路数据Asset，复制寻路数据Asset失败！");
                return false;
            }
            navMeshAssetPath = PathUtilities.GetRegularPath(navMeshAssetPath);
            var targetAssetFolderPath = Path.GetDirectoryName(mapAssetPath);
            var navMeshAssetName = Path.GetFileName(navMeshAssetPath);
            var newNavMeshAssetPath = Path.Combine(targetAssetFolderPath, navMeshAssetName);
            newNavMeshAssetPath = PathUtilities.GetRegularPath(newNavMeshAssetPath);
            if (!string.Equals(navMeshAssetPath, newNavMeshAssetPath))
            {
                AssetDatabase.MoveAsset(navMeshAssetPath, newNavMeshAssetPath);
                Debug.Log($"移动寻路数据Asset:{navMeshAssetPath}到{newNavMeshAssetPath}成功！");
            }
            else
            {
                Debug.Log($"移动寻路数据Asset:{navMeshAssetPath}已经在目标位置，不需要移动！");
            }
            return true;
        }

        /// <summary>
        /// 获取当前脚本GameObject对应Asset
        /// Note:
        /// 未存储到本地Asset返回null
        /// </summary>
        /// <returns></returns>
        public static string GetMapAssetPath(GameObject gameObject)
        {
            string assetPath = null;
            var gameObjectStatus = MapEditorUtilities.GetGameObjectStatus(gameObject);
            if (gameObjectStatus == GameObjectStatus.Normal)
            {
                return null;
            }
            else if (gameObjectStatus == GameObjectStatus.PrefabInstance)
            {
                var asset = PrefabUtility.GetCorrespondingObjectFromSource(gameObject);
                assetPath = AssetDatabase.GetAssetPath(asset);
            }
            else if (gameObjectStatus == GameObjectStatus.Asset)
            {
                assetPath = AssetDatabase.GetAssetPath(gameObject);
            }
            else if (gameObjectStatus == GameObjectStatus.PrefabContent)
            {
                var prefabStage = UnityEditor.SceneManagement.PrefabStageUtility.GetPrefabStage(gameObject);
                assetPath = prefabStage != null ? prefabStage.assetPath : null;
            }
            return assetPath;
        }

        /// <summary>
        /// 获取指定Map脚本导出文件名
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static string GetMapExportFileName(Map map)
        {
            // 有自定义导出文件名则用自定义导出文件名，未设置默认导出预制件同名
            if(map == null)
            {
                Debug.LogError($"不允许传空Map脚本，获取导出文件名失败！");
                return string.Empty;
            }
            if(!string.IsNullOrEmpty(map.CustomExportFileName))
            {
                return map.CustomExportFileName;
            }
            return map.gameObject.name;
        }

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static MapEditorUtilities()
        {
            InitStaticDatas();
        }

        /// <summary>
        /// 初始化静态数据
        /// </summary>
        private static void InitStaticDatas()
        {
            InitMapDataTypeFoldTypeMap();
            InitMapDataTypeColorMap();
            InitMapOneKeyFoldTitleMap();
            InitMapOneKeyUnfoldTitleMap();
            InitMapDataFoldAndUITypeMap();
        }

        #region 地图UI显示相关基础数据定义
        /// <summary>
        /// 地图UI类型显示数据列表
        /// Note:
        /// 标题和属性默认按照这里定义的顺序显示
        /// </summary>
        public static readonly List<MapUITypeDisplayData> MapUITypeDisplayData = new List<MapUITypeDisplayData>()
        {
            new MapUITypeDisplayData(MapUIType.Batch, "BatchOperationSwitch", "批量", MapEditorConst.InspectorDataBatchUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.Index, "", "索引", MapEditorConst.InspectorDataIndexUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.UID, "UID", "UID", MapEditorConst.InspectorDataUIDUIWidth, MapStyles.TabMiddleStyle),
            //new MapUITypeDisplayData(MapUIType.MapDataType, "", "埋点类型", MapEditorConst.InspectorDataTypeUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.ConfId, "", "配置Id", MapEditorConst.InspectorDataConfIdUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.GUISwitchOff, "GUISwitchOff", "GUI关闭", MapEditorConst.InspectorDataGUISwitchOffUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.Position, "Position", "位置", MapEditorConst.InspectorDataPositionUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.Rotation, "Rotation", "旋转", MapEditorConst.InspectorDataRotationUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.MonsterCreateRadius, "MonsterCreateRadius", "创建半径", MapEditorConst.InspectorDataMonsterCreateRadiusUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.MonsterActiveRadius, "MonsterActiveRadius", "警戒半径", MapEditorConst.InspectorDataMonsterActiveRediusUIWidth, MapStyles.TabMiddleStyle),
            //new MapUITypeDisplayData(MapUIType.Des, "", "描述", MapEditorConst.InspectorDataDesUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.MoveUp, "", "上移", MapEditorConst.InspectorDataMoveUpUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.MoveDown, "", "下移", MapEditorConst.InspectorDataMoveDownUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.Add, "", "添加", MapEditorConst.InspectorDataAddUIWidth, MapStyles.TabMiddleStyle),
            new MapUITypeDisplayData(MapUIType.Remove, "", "删除", MapEditorConst.InspectorDataRemoveUIWidth, MapStyles.TabMiddleStyle),
        };

        /// <summary>
        /// 地图埋点类型UI类型显示数据列表
        /// </summary>
        private static readonly List<MapDataTypeDisplayData> MapDataTypeDisplayDatas = new List<MapDataTypeDisplayData>()
        {
            new MapDataTypeDisplayData(MapDataType.PlayerSpawn, MapFoldType.PlayerSpawnMapDataFold, "玩家出生点", Color.yellow),
            new MapDataTypeDisplayData(MapDataType.Monster, MapFoldType.MonsterMapDataFold, "怪物", Color.magenta),
            new MapDataTypeDisplayData(MapDataType.TreasureBox, MapFoldType.TreasureBoxMapDataFold, "宝箱", Color.green),
            new MapDataTypeDisplayData(MapDataType.Trap, MapFoldType.TrapMapDataFold, "陷阱", Color.red),
        };
        #endregion

        #region 折叠部分
        /// <summary>
        /// 地图折叠类型UI类型显示数据列表
        /// </summary>
        private static readonly List<MapFoldTypeDisplayData> MapFoldTypeDisplayDatas = new List<MapFoldTypeDisplayData>()
        {
            new MapFoldTypeDisplayData(MapFoldType.MapObjectDataFold, "通用对象"),
            new MapFoldTypeDisplayData(MapFoldType.PlayerSpawnMapDataFold, "玩家出生点埋点"),
            new MapFoldTypeDisplayData(MapFoldType.MonsterMapDataFold, "怪物埋点"),
            new MapFoldTypeDisplayData(MapFoldType.TreasureBoxMapDataFold, "宝箱埋点"),
            new MapFoldTypeDisplayData(MapFoldType.TrapMapDataFold, "陷阱埋点"),
        };

        /// <summary>
        /// 地图埋点类型和折叠类型映射
        /// </summary>
        private static Dictionary<MapDataType, MapFoldType> MapDataTypeFoldTypeMap = new Dictionary<MapDataType, MapFoldType>(){};

        /// <summary>
        /// 地图埋点数据类型和默认显示颜色Map
        /// </summary>
        private static Dictionary<MapDataType, Color> MapDataTypeColorMap = new Dictionary<MapDataType, Color>() { };

        /// <summary>
        /// 地图折叠类型和一键折叠标题Map<地图折叠类型，一键折叠标题>
        /// </summary>
        private static Dictionary<MapFoldType, string> MapOneKeyFoldTitleMap = new Dictionary<MapFoldType, string>(){};

        /// <summary>
        /// 地图折叠类型和一键展开标题Map<地图折叠类型，一键展开标题>
        /// </summary>
        private static Dictionary<MapFoldType, string> MapOneKeyUnfoldTitleMap = new Dictionary<MapFoldType, string>(){};

        /// <summary>
        /// 所有埋点类型都通用的游戏地图UI类型Map<游戏地图UI类型，是否显示>
        /// </summary>
        private static Dictionary<MapUIType, bool> CommonMapUITypeMap = new Dictionary<MapUIType, bool>()
        {
            { MapUIType.Batch, true },
            { MapUIType.Index, true },
            { MapUIType.UID, true },
            { MapUIType.MapDataType, true },
            { MapUIType.ConfId, true },
            { MapUIType.Position, true },
            { MapUIType.Rotation, true },
            { MapUIType.GUISwitchOff, true },
            { MapUIType.Des, true },
            { MapUIType.MoveUp, true },
            { MapUIType.MoveDown, true },
            { MapUIType.Add, true },
            { MapUIType.Remove, true },
        };

        /// <summary>
        /// 地图埋点类型和UI显示类型Map<地图埋点类型，<地图UI显示类型，是否显示>>
        /// Note:
        /// 所有需要显示的折叠数据都在这里定义相关UI是否显示
        /// 通用必显示的不同定义在这里，详情参见CommonGameMapUITypeMap
        /// 所有MapDataType类型必定会自动初始化到MapFoldAndUITypeMap，没有DIY UI显示的可以不定义在这
        /// </summary>
        private static Dictionary<MapDataType, Dictionary<MapUIType, bool>> MapFoldAndUITypeMap = new Dictionary<MapDataType, Dictionary<MapUIType, bool>>()
        {
            {
                MapDataType.Monster, new Dictionary<MapUIType, bool>()
                {
                    {MapUIType.MonsterCreateRadius, true},
                    {MapUIType.MonsterActiveRadius, true},
                }
            },
        };

        /// <summary>
        /// 初始化MapDataTypeFoldTypeMap
        /// </summary>
        private static void InitMapDataTypeFoldTypeMap()
        {
            foreach(var mapDataTypeUIDisplayData in MapDataTypeDisplayDatas)
            {
                MapDataTypeFoldTypeMap.Add(mapDataTypeUIDisplayData.MapDataType, mapDataTypeUIDisplayData.MapFoldType);
            }
        }

        /// <summary>
        /// 初始化MapDataTypeColorMap
        /// </summary>
        private static void InitMapDataTypeColorMap()
        {
            foreach (var mapDataTypeUIDisplayData in MapDataTypeDisplayDatas)
            {
                MapDataTypeColorMap.Add(mapDataTypeUIDisplayData.MapDataType, mapDataTypeUIDisplayData.DisplayColor);
            }
        }

        /// <summary>
        /// 初始化MapOneKeyFoldTitleMap
        /// </summary>
        private static void InitMapOneKeyFoldTitleMap()
        {
            foreach (var mapFoldTypeDisplayData in MapFoldTypeDisplayDatas)
            {
                var mapFoldType = mapFoldTypeDisplayData.MapFoldType;
                var mapFoldDes = mapFoldTypeDisplayData.FoldDes;
                MapOneKeyFoldTitleMap.Add(mapFoldType, $"一键折叠所有({mapFoldDes})");
            }
        }

        /// <summary>
        /// 初始化MapOneKeyUnfoldTitleMap
        /// </summary>
        private static void InitMapOneKeyUnfoldTitleMap()
        {
            foreach (var mapFoldTypeDisplayData in MapFoldTypeDisplayDatas)
            {
                var mapFoldType = mapFoldTypeDisplayData.MapFoldType;
                var mapFoldDes = mapFoldTypeDisplayData.FoldDes;
                MapOneKeyUnfoldTitleMap.Add(mapFoldType, $"一键展开所有({mapFoldDes})");
            }
        }

        /// <summary>
        /// 初始化MapDataFoldAndUITypeMap
        /// </summary>
        private static void InitMapDataFoldAndUITypeMap()
        {
            foreach(var mapDataType in MapEditorConst.AllMapDataTypes)
            {
                Dictionary<MapUIType, bool> mapUITypeMap;
                if(!MapFoldAndUITypeMap.TryGetValue(mapDataType, out mapUITypeMap))
                {
                    mapUITypeMap = new Dictionary<MapUIType, bool>();
                    MapFoldAndUITypeMap.Add(mapDataType, mapUITypeMap);
                }
                foreach(var commonMapUIType in CommonMapUITypeMap)
                {
                    if(!mapUITypeMap.ContainsKey(commonMapUIType.Key))
                    {
                        mapUITypeMap.Add(commonMapUIType.Key, commonMapUIType.Value);
                    }
                }
            }
        }

        /// <summary>
        /// 获取制定地图埋点类型的折叠类型
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        public static MapFoldType GetMapDataFoldType(MapDataType mapDataType)
        {
            MapFoldType mapFoldType = MapFoldType.PlayerSpawnMapDataFold;
            if (!MapDataTypeFoldTypeMap.TryGetValue(mapDataType, out mapFoldType))
            {
                Debug.LogError($"找不到地图埋点类型:{mapDataType}的对应折叠类型！");
            }
            return mapFoldType;
        }

        /// <summary>
        /// 获取指定地图折叠类型的一键折叠标题
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <returns></returns>
        public static string GetMapOneKeyFoldTitle(MapFoldType mapFoldType)
        {
            string title = "未定义";
            if (MapOneKeyFoldTitleMap.TryGetValue(mapFoldType, out title))
            {

            }
            return title;
        }

        /// <summary>
        /// 获取指定地图折叠类型的一键展开标题
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <returns></returns>
        public static string GetMapOneKeyUnfoldTitle(MapFoldType mapFoldType)
        {
            string title = "未定义";
            if (MapOneKeyUnfoldTitleMap.TryGetValue(mapFoldType, out title))
            {

            }
            return title;
        }

        /// <summary>
        /// 是否显示指定数据埋点类型的地图数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        public static bool IsShowMapDataFoldType(MapDataType mapDataType)
        {
            return MapFoldAndUITypeMap.ContainsKey(mapDataType);
        }

        /// <summary>
        /// 是否显示指定数据埋点类型和地图UI类型的UI
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="mapDataUIType"></param>
        /// <returns></returns>
        public static bool IsShowMapUI(MapDataType mapDataType, MapUIType mapDataUIType)
        {
            Dictionary<MapUIType, bool> mapFoldTypeUIMap;
            if (!MapFoldAndUITypeMap.TryGetValue(mapDataType, out mapFoldTypeUIMap))
            {
                return false;
            }
            bool isShowMapUI;
            if (!mapFoldTypeUIMap.TryGetValue(mapDataUIType, out isShowMapUI))
            {
                return false;
            }
            return isShowMapUI;
        }
        #endregion
    }
}
