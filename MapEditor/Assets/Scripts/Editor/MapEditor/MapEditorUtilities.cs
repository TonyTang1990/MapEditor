﻿/*
 * Description:             MapEditorUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Unity.AI.Navigation;
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
                var selectionGos = new Object[dumplicatedNum];
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
        private static bool CheckSelfOrParentHasMapScript(Object[] objects)
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
            return CheckSelfOrParentHasMapScript(Selection.objects);
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
        /// 游戏地图埋点移除所有批量选中菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapData/ClearAllBatch #&E", false, 0)]
        static void MapDataClearAllBatch(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataClearAllBatch()");
            if (Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                Debug.Log($"同一时刻触发多次MapDataClearAllBatch()，忽略调用！");
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
                TryClearAllBatch(targetGameObjects[i] as GameObject);
            }
        }

        /// <summary>
        /// 游戏地图埋点移除所有批量选中菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapData/ClearAllBatch #&E", true, 0)]
        static bool MapDataClearAllBatchValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataClearAllBatchValidateFunction()");
            return CheckSelfHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 检查指定对象数组是否自身包含Map脚本
        /// </summary>
        /// <param name="objects"></param>
        /// <returns></returns>
        private static bool CheckSelfHasMapScript(Object[] objects)
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
            var map = go.GetComponent<Map>(true);
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
                        dumplicatedMapDatas.Add(newMapData);
                        Debug.Log($"选中对象:{go.name}复制勾选批量的地图埋点索引:{i}数据！");
                    }
                }
            }
            map.ClearAllMapDataBatchOperation();
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
        /// 尝试清除指定对象的批量地图埋点选中数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private static bool TryClearAllBatch(GameObject go)
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
            map.ClearAllMapDataBatchOperation();
            return true;
        }

        /// <summary>
        /// 获取指定地图埋点属性列表和指定索引的标签显示名
        /// </summary>
        /// <param name="mapDataPropertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static string GetMapDataPropertyLabelName(SerializedProperty mapDataPropertyList, int index)
        {
            if(mapDataPropertyList == null)
            {
                return "无效的地图属性";
            }
            if(!mapDataPropertyList.isArray)
            {
                return "非数组地图属性";
            }
            var mapDataProperty = mapDataPropertyList.GetArrayElementAtIndex(index);
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if(mapDataConfig == null)
            {
                return $"未知的UID:{uid}";
            }
            var mapDataDes = mapDataConfig.Des;
            var mapDataType = mapDataConfig.DataType;
            if(mapDataType == MapDataType.MONSTER || mapDataType == MapDataType.MONSTER_GROUP)
            {
                var groupIdProperty = mapDataProperty.FindPropertyRelative("GroupId");
                return $"[{index}]{mapDataDes}({groupIdProperty.intValue}组)";
            }
            else
            {
                return $"[{index}]{mapDataDes}";
            }
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
        /// 获取或创建指定地图GameObject的地图对象父挂点
        /// </summary>
        /// <param name="mapGO"></param>
        /// <returns></returns>
        public static Transform GetOrCreateMapObjectParentNode(GameObject mapGO)
        {
            if(mapGO == null)
            {
                Debug.LogError($"不允许传空地图GameObject，获取或创建地图对象父挂点失败！");
                return null;
            }
            var mapObjectParentTransform = mapGO.transform.Find(MapConst.MapObjectParentNodeName);
            if(mapObjectParentTransform == null)
            {
                mapObjectParentTransform = new GameObject(MapConst.MapObjectParentNodeName).transform;
                mapObjectParentTransform.SetParent(mapGO.transform);
            }
            return mapObjectParentTransform;
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
        /// 获取或创建指定地图地形节点
        /// </summary>
        /// <param name="mapGO"></param>
        /// <param name="customAsset"></param>
        /// <returns></returns>
        public static Transform GetOrCreateMapTerrianNode(GameObject mapGO, GameObject customAsset = null)
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
                var mapTerrianPrefab = customAsset != null ? customAsset : AssetDatabase.LoadAssetAtPath<GameObject>(MapConst.DetaulMapTerrianPrefabPath);
                if(mapTerrianPrefab == null)
                {
                    return null;
                }
                var mapTerrianNodeGo = PrefabUtility.InstantiatePrefab(mapTerrianPrefab) as GameObject;
                mapTerrianNodeTransform = mapTerrianNodeGo.transform;
                mapTerrianNodeTransform.name = mapTerrianNodeName;
                mapTerrianNodeTransform.SetParent(mapGO.transform);
            }
            return mapTerrianNodeTransform;
        }

        /// <summary>
        /// 获取或创建指定地图GameObject的寻路组件
        /// </summary>
        /// <param name="mapGO"></param>
        /// <returns></returns>
        public static NavMeshSurface GetOrCreateNavMeshSurface(GameObject mapGO)
        {
            if(mapGO == null)
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
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            return navMeshSurface;
        }

        /// <summary>
        /// 指定GameObject添加或更新指定地图对象UID的MapObjectDataMono数据
        /// </summary>
        /// <param name="go"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static MapObjectDataMono AddOrUpdateMapObjectDataMono(GameObject go, int uid)
        {
            if(go == null)
            {
                Debug.LogError($"不允许给空GameObject添加MapObjectDataMono脚本！");
                return null;
            }
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if(mapObjectConfig == null)
            {
                Debug.LogError($"找不到UID:{uid}的地图对象配置数据，GameObject:{go.name}添加MapObjectDataMono脚本失败！");
                return null;
            }
            var mapObjectDataMono = go.GetComponent<MapObjectDataMono>();
            if(mapObjectDataMono == null)
            {
                mapObjectDataMono = go.AddComponent<MapObjectDataMono>();
            }
            mapObjectDataMono.UID = uid;
            mapObjectDataMono.ObjectType = mapObjectConfig.ObjectType;
            mapObjectDataMono.ConfId = mapObjectDataMono.ConfId;
            return mapObjectDataMono;
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
        /// 地图埋点数据类型和默认显示颜色Map
        /// </summary>
        private static Dictionary<MapDataType, Color> MapDataTypeColorMap = new Dictionary<MapDataType, Color>()
        {
            { MapDataType.PLAYER_SPAWN, Color.yellow },
            { MapDataType.MONSTER, Color.magenta },
            { MapDataType.MONSTER_GROUP, Color.red },
        };

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
        /// 检查指定Map脚本是否满足导出条件
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool CheckIsGameMapAvalibleExport(Map map)
        {
            if(map == null)
            {
                Debug.LogError($"空Map脚本不符合导出条件!");
                return false;
            }
            if(CheckHasInvalideMapDataUID(map))
            {
                return false;
            }
            if(CheckHasInvalideMapObjectUID(map))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查Map脚本是否有无效埋点类型数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool CheckHasInvalideMapDataUID(Map map)
        {
            if(map == null)
            {
                return false;
            }
            MapDataConfig mapDataConfig;
            foreach(var mapData in map.MapDataList)
            {
                mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if(mapDataConfig == null)
                {
                    Debug.LogError($"地图埋点数据有配置不支持的地图埋点UID:{mapData.UID}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查Map脚本是否有无效地图对象UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool CheckHasInvalideMapObjectUID(Map map)
        {
            if(map == null)
            {
                return false;
            }
            MapObjectConfig mapObjectConfig;
            foreach(var mapObjectData in map.MapObjectDataList)
            {
                mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if(mapObjectConfig == null)
                {
                    Debug.LogError($"地图对象数据有配置不支持的地图对象UID:{mapObjectData.UID}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取地图导出目录全路径
        /// </summary>
        /// <returns></returns>
        public static string GetGameMapExportFolderFullPath(ExportType exportType)
        {
            var gameMapExportFolderFullPath = PathUtilities.GetAssetFullPath(MapEditorConst.MapExportRelativePath);
            var folderName = exportType.ToString();
            gameMapExportFolderFullPath = Path.Combine(gameMapExportFolderFullPath, folderName);
            return gameMapExportFolderFullPath;
        }

        /// <summary>
        /// 获取制定导出类型和导出文件名的导出全路径
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetGameMapExportFileFullPath(ExportType exportType, string fileName)
        {
            var gameMapExportFolderFullPath = GetGameMapExportFolderFullPath(exportType);
            var filePostFix = GetExportFilePostFix(exportType);
            var fileFullName = $"{fileName}{filePostFix}";
            var gameMapExportFileFullPath = Path.Combine(gameMapExportFolderFullPath, fileFullName);
            return gameMapExportFileFullPath;
        }

        /// <summary>
        /// 导出类型和文件后缀名Map
        /// </summary>
        private static Dictionary<ExportType, string> ExportTypePostFixMap = new Dictionary<ExportType, string>()
        {
            { ExportType.JSON, ".json" },
        };

        /// <summary>
        /// 获取制定导出类型的文件后缀名
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public static string GetExportFilePostFix(ExportType exportType)
        {
            string postFix = string.Empty;
            if(!ExportTypePostFixMap.TryGetValue(exportType, out postFix))
            {
                Debug.LogError($"找不到导出类型:{exportType.ToString()}的文件后缀名!");
            }
            return postFix;
        }

        /// <summary>
        /// 检查或创建地图导出目录
        /// </summary>
        public static void CheckOrCreateGameMapExportFolder(ExportType exportType)
        {
            var gameMapExportFolderFullPath = GetGameMapExportFolderFullPath(exportType);
            FolderUtilities.CheckAndCreateSpecificFolder(gameMapExportFolderFullPath);
        }

        /// <summary>
        /// 临时StringBuild(优化字符串拼接问题)
        /// </summary>
        private static StringBuilder TempStringBuild = new StringBuilder();

        /// <summary>
        /// 获取地图脚本的所有地图对象类型和地图对象数据列表Map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static Dictionary<MapObjectType, List<MapObjectData>> GetMapObjecTypeDatas(Map map)
        {
            if(map == null)
            {
                return null;
            }
            Dictionary<MapObjectType, List<MapObjectData>> mapObjectTypeDatasMap = new Dictionary<MapObjectType, List<MapObjectData>>();
            MapObjectConfig mapObjectConfig;
            foreach(var mapObjectData in map.MapObjectDataList)
            {
                mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if(mapObjectConfig == null)
                {
                    continue;
                }
                List<MapObjectData> mapObjectDataList;
                if(!mapObjectTypeDatasMap.TryGetValue(mapObjectConfig.ObjectType, out mapObjectDataList))
                {
                    mapObjectDataList = new List<MapObjectData>();
                    mapObjectTypeDatasMap.Add(mapObjectConfig.ObjectType, mapObjectDataList);
                }
                mapObjectDataList.Add(mapObjectData);
            }
            return mapObjectTypeDatasMap;
        }

        /// <summary>
        /// 获取地图脚本的所有地图埋点类型和地图埋点数据列表Map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static Dictionary<MapDataType, List<MapData>> GetMapDataTypeDatas(Map map)
        {
            if (map == null)
            {
                return null;
            }
            Dictionary<MapDataType, List<MapData>> mapDataTypeDatasMap = new Dictionary<MapDataType, List<MapData>>();
            MapDataConfig mapDataConfig;
            foreach (var mapData in map.MapDataList)
            {
                mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if (mapDataConfig == null)
                {
                    continue;
                }
                List<MapData> mapDataList;
                if (!mapDataTypeDatasMap.TryGetValue(mapDataConfig.DataType, out mapDataList))
                {
                    mapDataList = new List<MapData>();
                    mapDataTypeDatasMap.Add(mapDataConfig.DataType, mapDataList);
                }
                mapDataList.Add(mapData);
            }
            return mapDataTypeDatasMap;
        }

        /// <summary>
        /// 获取Vector3的Lua Table表达
        /// </summary>
        /// <param name="vec3"></param>
        /// <returns></returns>
        private static string GetVector3LuaTableContent(Vector3 vec3)
        {
            return $"{{x = {vec3.x}, y = {vec3.y}, z = {vec3.z}}}";
        }

        /// <summary>
        /// 导出指定地图数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="exportType"></param>
        public static void ExportGameMapData(Map map)
        {
            if(map == null)
            {
                Debug.LogError($"不允许导出空的Map脚本地图数据，导出地图数据失败！");
                return;
            }
            CheckOrCreateGameMapExportFolder(map.ExportType);
            if(map.ExportType == ExportType.JSON)
            {
                DoExportGameMapJsonData(map);
            }
            else
            {
                Debug.LogError($"不支持的导出类型:{map.ExportType.ToString()}，导出地图数据失败！");
            }
        }

        /// <summary>
        /// 导出指定地图组件的Json格式数据
        /// </summary>
        /// <param name="map"></param>
        private static void DoExportGameMapJsonData(Map map)
        {
            var mapExport = GetMapExport(map);
            var mapDataJsonContent = JsonUtility.ToJson(mapExport, true);
            var fileName = map.gameObject.name;
            var exportFileFullPath = GetGameMapExportFileFullPath(map.ExportType, fileName);
            using(var sw = File.CreateText(exportFileFullPath))
            {
                sw.Write(mapDataJsonContent);
            }
            Debug.Log($"导出地图数据:{exportFileFullPath}成功！");
        }
        
        /// <summary>
        /// 获取指定Map组件的导出数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static MapExport GetMapExport(Map map)
        {
            MapExport mapExport = new MapExport();
            var mapDataTypeDatasMap = GetMapDataTypeDatas(map);
            mapExport.MapData.Width = map.MapWidth;
            mapExport.MapData.Height = map.MapHeight;
            mapExport.MapData.StartPos = map.MapStartPos;
            List<MapData> playerSpawnDatas;
            if(mapDataTypeDatasMap.TryGetValue(MapDataType.PLAYER_SPAWN, out playerSpawnDatas))
            {
                foreach(var playerSpawnData in playerSpawnDatas)
                {
                    mapExport.MapData.BirthPos.Add(playerSpawnData.Position);
                }
            }

            foreach(var mapObjectData in map.MapObjectDataList)
            {
                var mapObjectUID = mapObjectData.UID;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
                if(mapObjectConfig.IsDynamic)
                {
                    var mapDataExport = GetColliderMapDynamicExport(mapObjectData);
                    mapExport.AllColliderMapDynamicExportDatas.Add(mapDataExport);
                }
                else
                {
                    var mapDynamicExport = GetBaseMapDynamicExport(mapObjectData);
                    mapExport.AllBaseMapObjectExportDatas.Add(mapDynamicExport);
                }
            }

            Dictionary<int, MapData> monsterGroupMap = new Dictionary<int, MapData>();
            List<MapData> monsterGroupDatas;
            if(mapDataTypeDatasMap.TryGetValue(MapDataType.MONSTER_GROUP, out monsterGroupDatas))
            {
                foreach(var mapData in monsterGroupDatas)
                {
                    var monsterGroupData = mapData as MonsterGroupMapData;
                    if (!monsterGroupMap.ContainsKey(monsterGroupData.GroupId))
                    {
                        monsterGroupMap.Add(monsterGroupData.GroupId, monsterGroupData);
                    }
                    else
                    {
                        Debug.LogError($"有重复组Id:{monsterGroupData.GroupId}的怪物组埋点数据，请检查埋点配置！");
                    }
                }
            }

            Dictionary<int, List<MapData>> monsterMap = new Dictionary<int, List<MapData>>();
            List<MapData> monsterDatas;
            List<MapData> noGroupMonsterDatas = new List<MapData>();
            if(mapDataTypeDatasMap.TryGetValue(MapDataType.MONSTER, out monsterDatas))
            {
                foreach(var mapData in monsterDatas)
                {
                    var monsterData = mapData as MonsterMapData;
                    var monsterGroupId = monsterData.GroupId;
                    if(!monsterGroupMap.ContainsKey(monsterGroupId))
                    {
                        noGroupMonsterDatas.Add(monsterData);
                    }

                    List<MapData> groupMonsterDatas;
                    if(!monsterMap.TryGetValue(monsterData.GroupId, out groupMonsterDatas))
                    {
                        groupMonsterDatas = new List<MapData>();
                        monsterMap.Add(monsterData.GroupId, groupMonsterDatas);
                    }
                    groupMonsterDatas.Add(monsterData);
                }
            }

            foreach(var monsterGroup in monsterGroupMap)
            {
                var monsterGroupMapData = monsterGroup.Value as MonsterGroupMapData;
                var groupId = monsterGroupMapData.GroupId;
                var monsterGroupMapDataExport = GetMonsterGroupMapDataExport(monsterGroupMapData);
                List<MapData> groupMonsterMapDatas;
                if(monsterMap.TryGetValue(groupId, out groupMonsterMapDatas))
                {
                    foreach(var mapData in groupMonsterMapDatas)
                    {
                        var monsterMapDataExport = GetMonsterMapDataExport(mapData);
                        monsterGroupMapDataExport.AllMonsterMapExportDatas.Add(monsterMapDataExport);
                    }
                }
                mapExport.AllMonsterGroupMapDatas.Add(monsterGroupMapDataExport);
            }

            if(noGroupMonsterDatas != null)
            {
                foreach(var noGroupMonsterData in noGroupMonsterDatas)
                {
                    var monsterMapDataExport = GetMonsterMapDataExport(noGroupMonsterData);
                    mapExport.ALlNoGroupMonsterMapDatas.Add(monsterMapDataExport);
                }
            }

            foreach(var mapData in map.MapDataList)
            {
                var mapDataUID = mapData.UID;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
                var mapDataType = mapDataConfig.DataType;
                if(mapDataType == MapDataType.PLAYER_SPAWN || mapDataType == MapDataType.MONSTER || mapDataType == MapDataType.MONSTER_GROUP)
                {
                    continue;
                }
                var mapDataExport = GetBaseMapDataExport(mapData);
                mapExport.AllOtherMapDatas.Add(mapDataExport);
            }
            return mapExport;
        }

        /// <summary>
        /// 获取指定地图对象数据的地图动态对象碰撞体导出数据
        /// </summary>
        /// <param name="mapObjectData"></param>
        /// <returns></returns>
        private static ColliderMapDynamicExport GetColliderMapDynamicExport(MapObjectData mapObjectData)
        {
            if(mapObjectData == null)
            {
                Debug.LogError("不允许获取空地图对象数据的地图动态对象碰撞体导出数据失败！");
                return null;
            }
            var mapObjectUID = mapObjectData.UID;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if(mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，获取地图动态对象数据碰撞体导出数据失败！");
                return null;
            }
            return new ColliderMapDynamicExport(mapObjectConfig.ObjectType, mapObjectConfig.ConfId, mapObjectData.Position, mapObjectData.Rotation,
                                                    mapObjectData.ColliderCenter, mapObjectData.ColliderSize, mapObjectData.ColliderRadius);
        }

        /// <summary>
        /// 获取指定地图对象数据的地图动态对象基础导出数据
        /// </summary>
        /// <param name="mapObjectData"></param>
        /// <returns></returns>
        private static BaseMapDynamicExport GetBaseMapDynamicExport(MapObjectData mapObjectData)
        {
            if (mapObjectData == null)
            {
                Debug.LogError("不允许获取空地图对象数据的地图动态对象基础导出数据失败！");
                return null;
            }
            var mapObjectUID = mapObjectData.UID;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，获取地图动态对象数据基础导出数据失败！");
                return null;
            }
            return new BaseMapDynamicExport(mapObjectConfig.ObjectType, mapObjectConfig.ConfId, mapObjectData.Position, mapObjectData.Rotation);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点怪物组导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static MonsterGroupMapDataExport GetMonsterGroupMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点怪物组导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点怪物组导出数据失败！");
                return null;
            }
            var monsterGroupMapData = mapData as MonsterGroupMapData;
            return new MonsterGroupMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, monsterGroupMapData.Position, monsterGroupMapData.Rotation,
                                                    monsterGroupMapData.GroupId, monsterGroupMapData.MonsterCreateRadius, monsterGroupMapData.MonsterActiveRadius);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点怪物导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static MonsterMapDataExport GetMonsterMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点怪物导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点怪物导出数据失败！");
                return null;
            }
            var monsterMapData = mapData as MonsterMapData;
            return new MonsterMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, monsterMapData.Position, monsterMapData.Rotation, monsterMapData.GroupId);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点基础导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static BaseMapDataExport GetBaseMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点基础导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点基础导出数据失败！");
                return null;
            }
            return new BaseMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, mapData.Position, mapData.Rotation);
        }

        /// <summary>
        /// 指定目标int值是否在指定值数组范围内
        /// </summary>
        /// <param name="targetValue"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        public static bool IsIntValueInArrays(int targetValue, int[] values)
        {
            if(values == null || values.Length == 0)
            {
                return false;
            }
            foreach(var value in values)
            {
                if(targetValue == value)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
