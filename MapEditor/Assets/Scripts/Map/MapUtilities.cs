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
using UnityEditor.SceneManagement;
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

#if UNITY_EDITOR
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
        /// 创建指定地图埋点数据类型，指定uid和指定位置的埋点数据
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="uid"></param>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <returns></returns>
        public static MapData CreateMapDataByType(MapDataType mapDataType, int uid, Vector3 position, Vector3 rotation)
        {
            if (mapDataType == MapDataType.Monster)
            {
                return new MonsterMapData(uid, position, rotation);
            }
            else if (mapDataType == MapDataType.MonsterGroup)
            {
                return new MonsterGroupMapData(uid, position, rotation);
            }
            else if(mapDataType == MapDataType.PlayerSpawn)
            {
                return new PlayerSpawnMapData(uid, position, rotation);
            }
            else
            {
                Debug.LogWarning($"地图埋点类型:{mapDataType}没有创建自定义类型数据，可能不方便未来扩展！");
                return new MapData(uid, position, rotation);
            }
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
            if(go == null)
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
            if (PrefabStageUtility.GetPrefabStage(go) != null)
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
            if(go == null)
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
            PrefabStageUtility.OpenPrefab(prefabAssetPath);
        }

        /// <summary>
        /// 检查操作是否可用
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public static bool CheckOperationAvalible(GameObject go)
        {
            if(go == null)
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
            navMeshSurface.useGeometry = NavMeshCollectGeometry.PhysicsColliders;
            return navMeshSurface;
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
            var gameObjectStatus = MapUtilities.GetGameObjectStatus(gameObject);
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
                var prefabStage = PrefabStageUtility.GetPrefabStage(gameObject);
                assetPath = prefabStage != null ? prefabStage.assetPath : null;
            }
            return assetPath;
        }

        /// <summary>
        /// 检查指定Map脚本是否满足导出条件
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool CheckIsGameMapAvalibleExport(Map map)
        {
            if (map == null)
            {
                Debug.LogError($"空Map脚本不符合导出条件!");
                return false;
            }
            if (CheckHasInvalideMapDataUID(map))
            {
                return false;
            }
            if (CheckHasInvalideMapObjectUID(map))
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
            if (map == null)
            {
                return false;
            }
            MapDataConfig mapDataConfig;
            foreach (var mapData in map.MapDataList)
            {
                mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if (mapDataConfig == null)
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
            if (map == null)
            {
                return false;
            }
            MapObjectConfig mapObjectConfig;
            foreach (var mapObjectData in map.MapObjectDataList)
            {
                mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    Debug.LogError($"地图对象数据有配置不支持的地图对象UID:{mapObjectData.UID}");
                    return true;
                }
            }
            return false;
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
            if(mapDataList == null)
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
#endif
    }
}