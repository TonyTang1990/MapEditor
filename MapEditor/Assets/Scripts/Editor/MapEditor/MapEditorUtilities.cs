/*
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
        /// 游戏地图埋点选中所有批量选中菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapData/SelectAllBatch #&S", false, 0)]
        static void MapDataSelectAllBatch(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataSelectAllBatch()");
            if (Time.unscaledTime.Equals(LastDumplicateMenuCallTimestamp))
            {
                Debug.Log($"同一时刻触发多次MapDataSelectAllBatch()，忽略调用！");
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
                UpdateAllBatch(targetGameObjects[i] as GameObject, true);
            }
        }

        /// <summary>
        /// 游戏地图埋点选中所有批量选中菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapData/SelectAllBatch #&S", true, 0)]
        static bool MapDataSelectAllBatchValidateFunction(MenuCommand menuCommand)
        {
            Debug.Log($"MapDataSelectAllBatchValidateFunction()");
            return CheckSelfHasMapScript(Selection.objects);
        }

        /// <summary>
        /// 游戏地图埋点移除所有批量选中菜单
        /// </summary>
        /// <param name="menuCommand"></param>
        [MenuItem("GameObject/Map/MapData/ClearAllBatch #&C", false, 0)]
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
                UpdateAllBatch(targetGameObjects[i] as GameObject, false);
            }
        }

        /// <summary>
        /// 游戏地图埋点移除所有批量选中菜单验证方法
        /// </summary>
        /// <param name="menuCommand"></param>
        /// <returns></returns>
        [MenuItem("GameObject/Map/MapData/ClearAllBatch #&C", true, 0)]
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
                        dumplicatedMapDatas.Add(newMapData);
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
            if(mapDataType == MapDataType.Monster || mapDataType == MapDataType.MonsterGroup)
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
            { MapDataType.PlayerSpawn, Color.yellow },
            { MapDataType.Monster, Color.magenta },
            { MapDataType.MonsterGroup, Color.red },
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
        /// 绘制指定地图埋点类型的埋点数据标题区域
        /// </summary>
        /// <param name="mapDataType"></param>
        public static void DrawMapDataTitleAreaByType(MapDataType mapDataType)
        {
            EditorGUILayout.BeginHorizontal("box");
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Batch))
            {
                EditorGUILayout.LabelField("批量", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataBatchUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Index))
            {
                EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataIndexUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.UID))
            {
                EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataUIDUIWidth));
            }
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MapDataType))
            //{
            //    EditorGUILayout.LabelField("埋点类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataTypeUIWidth));
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.ConfId))
            {
                EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataConfIdUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterGroupId))
            {
                EditorGUILayout.LabelField("组Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataGroupIdUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterCreateRadius))
            {
                EditorGUILayout.LabelField("创建半径", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataMonsterCreateRadiusUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterActiveRadius))
            {
                EditorGUILayout.LabelField("警戒半径", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataMonsterActiveRediusUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterGroupGUISwitchOff))
            {
                EditorGUILayout.LabelField("GUI关闭", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataMonsterGroupGUISwitchOffUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Position))
            {
                EditorGUILayout.LabelField("位置", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataPositionUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Rotation))
            {
                EditorGUILayout.LabelField("旋转", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataRotationUIWidth));
            }
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Des))
            //{
            //    EditorGUILayout.LabelField("描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataDesUIWidth));
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MoveUp))
            {
                EditorGUILayout.LabelField("上移", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataMoveUpUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MoveDown))
            {
                EditorGUILayout.LabelField("下移", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataMoveDownUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Add))
            {
                EditorGUILayout.LabelField("添加", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataRemoveUIWidth));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Remove))
            {
                EditorGUILayout.LabelField("删除", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataAddUIWidth));
            }
            EditorGUILayout.EndHorizontal();
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

        #region 折叠部分

        /// <summary>
        /// 地图埋点类型和折叠类型映射
        /// </summary>
        private static Dictionary<MapDataType, MapFoldType> MapDataTypeFoldTypeMap = new Dictionary<MapDataType, MapFoldType>()
        {
            {MapDataType.PlayerSpawn, MapFoldType.PlayerSpawnMapDataFold},
            {MapDataType.Monster, MapFoldType.MonsterMapDataFold},
            {MapDataType.MonsterGroup, MapFoldType.MonsterGroupMapDataFold},
        };

        /// <summary>
        /// 地图折叠类型和一键折叠标题Map<地图折叠类型，一键折叠标题>
        /// </summary>
        private static Dictionary<MapFoldType, string> MapOneKeyFoldTitleMap = new Dictionary<MapFoldType, string>()
        {
            {MapFoldType.MapObjectDataFold, "一键折叠所有(通用对象数据)"},
            {MapFoldType.PlayerSpawnMapDataFold, "一键折叠所有(玩家出生点埋点数据)"},
            {MapFoldType.MonsterMapDataFold, "一键折叠所有(怪物埋点数据)"},
            {MapFoldType.MonsterGroupMapDataFold, "一键折叠所有(怪物组埋点数据)"},
        };

        /// <summary>
        /// 地图折叠类型和一键展开标题Map<地图折叠类型，一键展开标题>
        /// </summary>
        private static Dictionary<MapFoldType, string> MapOneKeyUnfoldTitleMap = new Dictionary<MapFoldType, string>()
        {
            {MapFoldType.MapObjectDataFold, "一键展开所有(通用对象数据)"},
            {MapFoldType.PlayerSpawnMapDataFold, "一键展开所有(玩家出生点埋点数据)"},
            {MapFoldType.MonsterMapDataFold, "一键展开所有(怪物埋点数据)"},
            {MapFoldType.MonsterGroupMapDataFold, "一键展开所有(怪物组埋点数据)"},
        };

        /// <summary>
        /// 地图埋点类型和UI显示类型Map<地图埋点类型，<地图UI显示类型，是否显示>>
        /// </summary>
        private static Dictionary<MapDataType, Dictionary<MapDataUIType, bool>> MapDataFoldAndUITypeMap = new Dictionary<MapDataType, Dictionary<MapDataUIType, bool>>()
        {
            {
                MapDataType.PlayerSpawn, new Dictionary<MapDataUIType, bool>()
                {
                    {MapDataUIType.Batch, true},
                    {MapDataUIType.Index, true},
                    {MapDataUIType.UID, true},
                    {MapDataUIType.MapDataType, true},
                    {MapDataUIType.ConfId, true},
                    {MapDataUIType.Position, true},
                    {MapDataUIType.Rotation, true},
                    {MapDataUIType.Des, true},
                    {MapDataUIType.MoveUp, true},
                    {MapDataUIType.MoveDown, true},
                    {MapDataUIType.Add, true},
                    {MapDataUIType.Remove, true},
                }
            },
            {
                MapDataType.Monster, new Dictionary<MapDataUIType, bool>()
                {
                    {MapDataUIType.Batch, true},
                    {MapDataUIType.Index, true},
                    {MapDataUIType.UID, true},
                    {MapDataUIType.MapDataType, true},
                    {MapDataUIType.ConfId, true},
                    {MapDataUIType.MonsterGroupId, true},
                    {MapDataUIType.Position, true},
                    {MapDataUIType.Rotation, true},
                    {MapDataUIType.Des, true},
                    {MapDataUIType.MoveUp, true},
                    {MapDataUIType.MoveDown, true},
                    {MapDataUIType.Add, true},
                    {MapDataUIType.Remove, true},
                }
            },
            {
                MapDataType.MonsterGroup, new Dictionary<MapDataUIType, bool>()
                {
                    {MapDataUIType.Batch, true},
                    {MapDataUIType.Index, true},
                    {MapDataUIType.UID, true},
                    {MapDataUIType.MapDataType, true},
                    {MapDataUIType.ConfId, true},
                    {MapDataUIType.MonsterGroupId, true},
                    {MapDataUIType.MonsterCreateRadius, true},
                    {MapDataUIType.MonsterActiveRadius, true},
                    {MapDataUIType.MonsterGroupGUISwitchOff, true},
                    {MapDataUIType.Position, true},
                    {MapDataUIType.Rotation, true},
                    {MapDataUIType.Des, true},
                    {MapDataUIType.MoveUp, true},
                    {MapDataUIType.MoveDown, true},
                    {MapDataUIType.Add, true},
                    {MapDataUIType.Remove, true},
                }
            },
        };

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
            return MapDataFoldAndUITypeMap.ContainsKey(mapDataType);
        }

        /// <summary>
        /// 是否显示指定数据埋点类型和地图UI类型的UI
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="mapDataUIType"></param>
        /// <returns></returns>
        public static bool IsShowMapUI(MapDataType mapDataType, MapDataUIType mapDataUIType)
        {
            Dictionary<MapDataUIType, bool> mapFoldTypeUIMap;
            if (!MapDataFoldAndUITypeMap.TryGetValue(mapDataType, out mapFoldTypeUIMap))
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
