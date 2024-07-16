/*
 * Description:             Map.cs
 * Author:                  TONYTANG
 * Create Date:             2024/07/11
 */

using System;
using System.Collections.Generic;
using System.Text;

namespace MapEditor
{
    /// <summary>
    /// MapExtensionEditor.cs
    /// Map脚本Editor扩展
    /// </summary>
    public static class MapExtensionEditor
    {
        /// <summary>
        /// 执行添加指定地图对象UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <param name="copyRotation">是否复制旋转</param>
        /// <returns></returns>
        public static MapObjectData DoAddMapObjectData(this Map map, int uid, int insertIndex = -1, bool copyRotation = false)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(gameObject))
            {
                return null;
            }
            return map.AddMapObjectData(uid, insertIndex, copyRotation);
        }

        /// <summary>
        /// 添加指定地图对象UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <param name="copyRotation">是否复制旋转</param>
        /// <returns></returns>
        private static MapObjectData AddMapObjectData(this Map map, int uid, int insertIndex = -1, bool copyRotation = false)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持添加此地图对象数据！");
                return null;
            }
            var mapObjectDataTotalNum = map.MapObjectDataList.Count;
            var maxInsertIndex = mapObjectDataTotalNum == 0 ? 0 : mapObjectDataTotalNum;
            var insertPos = 0;
            if (insertIndex == -1)
            {
                insertPos = maxInsertIndex;
            }
            else
            {
                insertPos = Math.Clamp(insertIndex, 0, maxInsertIndex);
            }
            var mapObjectPosition = map.MapStartPos;
            var mapObjectRotation = Quaternion.identity;
            if (mapObjectDataTotalNum != 0)
            {
                var insertMapObjectPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapObjectData = map.MapObjectDataList[insertMapObjectPos];
                mapObjectPosition = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.position : insertMapObjectData.Position;
                mapObjectRotation = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.rotation : Quaternion.Euler(insertMapObjectData.Rotation);
            }
            var instanceGo = map.CreateGameObjectByUID(uid);
            if (instanceGo != null && map.MapObjectAddedAutoFocus)
            {
                Selection.SetActiveObjectWithContext(instanceGo, instanceGo);
            }
            instanceGo.transform.position = mapObjectPosition;
            if (copyRotation)
            {
                instanceGo.transform.rotation = mapObjectRotation;
            }
            var newMapObjectData = new MapObjectData(uid, instanceGo);
            map.MapObjectDataList.Insert(insertPos, newMapObjectData);
            Debug.Log($"插入地图对象UID:{uid}索引:{insertPos}");
            map.UpdateMapObjectDataLogicDatas();
            return newMapObjectData;
        }

        /// <summary>
        /// 获取制定索引的地图对象数据
        /// Note:
        /// 返回null表示无效索引
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static MapObjectData GetMapObjectDataByIndex(this Map map, int index)
        {
            if (index < 0 || index >= map.MapObjectDataList..Count)
            {
                return null;
            }
            return map.MapObjectDataList[index];
        }

        /// <summary>
        /// 获取指定实例对象对应的地图对象数据索引
        /// Note:
        /// 返回-1表示找不到制定实例对象对应的地图对象数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="go"></param>
        /// <returns></returns>
        public static int GetMapObjectDataIndexByGo(this Map map, GameObject go)
        {
            return map.MapObjectDataList.FindIndex(mapObjectData => mapObjectData.Go == go);
        }

        /// <summary>
        /// 执行移除指定索引的地图对象数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool DoRemoveMapObjectDataByIndex(this Map map, int index)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return false;
            }
            return map.RemoveMapObjectDataByIndex(index);
        }

        /// <summary>
        /// 移除指定索引的地图对象数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool RemoveMapObjectDataByIndex(this Map map, int index)
        {
            var mapObjectDataNum = map.MapObjectDataList.Count;
            if (index < 0 || index >= mapObjectDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapObjectDataNum - 1},移除地图对象数据失败！");
                return false;
            }
            var mapObjectData = map.MapObjectDataList[index];
            if (mapObjectData.Go != null)
            {
                mapObjectData.Position = Vector3.zero;
                GameObject.DestroyImmediate(mapObjectData.Go);
            }
            map.MapObjectDataList..RemoveAt(index);
            map.UpdateMapObjectDataLogicDatas();
            return true;
        }

        /// <summary>
        /// 执行添加指定地图埋点UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        public static MapData DoAddMapData(this Map map, int uid, int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return null;
            }
            return map.AddMapData(uid, insertIndex, copyRotation, positionOffset);
        }

        /// <summary>
        /// 添加指定地图埋点UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        private static MapData AddMapData(this Map map, int uid, int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            return MapUtilities.AddMapDataToList(map.MapDataList, uid, map.MapStartPos, insertIndex, copyRotation, positionOffset);
        }

        /// <summary>
        /// 执行移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool DoRemoveMapDataByIndex(this Map map, int index)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return false;
            }
            return map.RemoveMapDataByIndex(index);
        }

        /// <summary>
        /// 移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private static bool RemoveMapDataByIndex(this Map map, int index)
        {
            return MapUtilities.RemoveMapDataFromListByIndex(map.MapDataList, index);
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="map"></param>
        /// <param name="isOn"></param>
        public static bool UpdateAllMapDataBatchOperation(this Map map, bool isOn)
        {
            return MapUtilities.UpdateAllMapDataBatchOperationByList(map.MapDataList, isOn);
        }

        /// <summary>
        /// 创建之地给你地图对象UID配置的实体对象(未配置Asset返回null)
        /// </summary>
        /// <param name="map"></param>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static GameObject CreateGameObjectByUID(this Map map, int uid)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持创建地图实体对象！");
                return null;
            }
            var instanceGo = mapObjectConfig.Asset != null ? PrefabUtility.InstantiatePrefab(mapObjectConfig.Asset) as GameObject : null;
            if (instanceGo != null)
            {
                var mapObjectType = mapObjectConfig.ObjectType;
                var parentNodeTransform = MapEditorUtilities.GetOrCreateMapObjectTypeParentNode(gameObject, mapObjectType);
                instanceGo.transform.SetParent(parentNodeTransform);
                instanceGo.transform.position = map.MapStartPos;
                var instanceGoName = instanceGo.name.RemoveSubStr("(Clone)");
                instanceGoName = $"{instanceGoName}_{uid}";
                instanceGo.name = instanceGoName;
                // 动态物体碰撞器统一代码创建
                //if (mapObjectConfig.IsDynamic)
                //{
                //    MapUtilities.AddOrUpdateColliderByColliderDataMono(instanceGo);
                //}
                //else
                //{
                //    MapUtilities.UpdateColliderByColliderDataMono(instanceGo);
                //}
                // 优化静态对象碰撞框，所有对象统一代码创建碰撞框
                // 静态碰撞框会在最后删除动态数据时统一清除
                MapEditorUtilities.AddOrUpdateColliderByColliderDataMono(instanceGo);
                MapEditorUtilities.AddOrUpdateMapObjectDataMono(instanceGo, uid);
            }
            return instanceGo;
        }

        /// <summary>
        /// 一键烘焙和导出地图数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static async Task<bool> OneKeyBakeAndExport(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return false;
            }
            if (!map.RecoverDynamicMapDatas())
            {
                return false;
            }
            var navMeshSurface = MapEditorUtilities.GetOrCreateNavMeshSurface(gameObject);
            var bakePathTask = MapEditorUtilities.BakePathTask(navMeshSurface);
            var bakePathResult = await bakePathTask;
            if (!bakePathResult)
            {
                return false;
            }
            var copyNavMeshAssetResult = await MapEditorUtilities.CopyNavMeshAsset(gameObject);
            if (!copyNavMeshAssetResult)
            {
                return false;
            }
            if (!map.CleanDynamicMapDatas())
            {
                return false;
            }
            map.ExportMapData();
            AssetDatabase.SaveAssets();
            Debug.Log($"一键烘焙拷贝导出地图:{map.gameObject.name}数据完成！");
            return true;
        }

        /// <summary>
        /// 恢复动态地图数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool RecoverDynamicMapDatas(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return false;
            }
            map.UpdateMapObjectDataLogicDatas();
            if (!map.RecoverDynamicMapObjects())
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 更新地图对象数据的逻辑数据(对象存在时才更新记录逻辑数据)
        /// </summary>
        /// <param name="map"></param>
        private static void UpdateMapObjectDataLogicDatas(this Map map)
        {
            // 地图对象可能删除还原，所以需要逻辑层面记录数据
            for (int i = 0; i < map.MapObjectDataList.Count; i++)
            {
                var mapObjectData = map.MapObjectDataList[i];
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    continue;
                }
                if (mapObjectData.Go != null)
                {
                    var mapObjectGO = mapObjectData.Go;
                    mapObjectData.Position = mapObjectGO.transform.position;
                    mapObjectData.Rotation = mapObjectGO.transform.rotation.eulerAngles;
                    mapObjectData.LocalScale = mapObjectGO.transform.localScale;
                    var boxCollider = mapObjectGO.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        mapObjectData.ColliderCenter = boxCollider.center;
                        mapObjectData.ColliderSize = boxCollider.size;
                    }
                }
            }
        }

        /// <summary>
        /// 恢复动态地图对象
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool RecoverDynamicMapObjects(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                Debug.LogError($"地图:{map.gameObject.name}恢复动态地图对象失败！");
                return false;
            }
            for (int i = 0; i < map.MapObjectDataList..Count; i++)
            {
                var mapObjectData = map.MapObjectDataList[i];
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    continue;
                }
                var isDynamic = MapSetting.GetEditorInstance().ObjectSetting.IsDynamicMapObjectType(mapObjectConfig.ObjectType);
                if (isDynamic && mapObjectData.Go == null)
                {
                    map.RecreateMapObjectGo(mapObjectData);
                }
            }
            return true;
        }

        /// <summary>
        /// 恢复指定MapObject属性地图对象
        /// </summary>
        /// <param name="map"></param>
        /// <param name="mapObjectData"></param>
        private static void RecreateMapObjectGo(this Map map, MapObjectData mapObjectData)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectData.UID}的配置，重创地图对象失败！");
                return;
            }
            if (mapObjectData.Go != null)
            {
                GameObject.DestroyImmediate(mapObjectData.Go);
            }
            var instanceGo = map.CreateGameObjectByUID(mapObjectData.UID);
            if (instanceGo != null)
            {
                instanceGo.transform.position = mapObjectData.Position;
                instanceGo.transform.rotation = Quaternion.Euler(mapObjectData.Rotation);
                instanceGo.transform.localScale = mapObjectData.LocalScale;
                mapObjectData.Go = instanceGo;
                // 存的碰撞体数据只用于导出，不用于还原
                // 是否有碰撞体，碰撞体数据有多少由预制件自身和预制件自身是否挂在ColliderDataMono脚本决定
                //if(mapObjectConfig.IsDynamic)
                //{
                //    var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                //    var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                //    var colliderRadiusProperty = mapObjectDataProperty.FindPropertyRelative("ColliderRadius");
                //    MapUtilities.UpdateColliderByColliderData(instanceGo, colliderCenterProperty.vector3Value, colliderSizeProperty.vector3Value, colliderRadiusProperty.floatValue);
                //}
            }
        }

        /// <summary>
        /// 清除动态地图数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static bool CleanDynamicMapDatas(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                Debug.LogError($"地图:{map.gameObject.name}清除动态地图数据失败！");
                return false;
            }
            map.UpdateMapObjectDataLogicDatas();
            if (!map.CleanDynamicMapObjects())
            {
                Debug.LogError($"地图:{map.gameObject.name}清除动态地图对象失败！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 清除动态地图对象GameObjects
        /// </summary>
        /// <param name="map"></param>
        private static bool CleanDynamicMapObjects(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                Debug.LogError($"地图:{map.gameObject.name}清除动态地图对象失败！");
                return false;
            }
            for (int i = 0; i < map.MapObjectDataList.Count; i++)
            {
                var mapObjectData = map.MapObjectDataList[i];
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    continue;
                }
                var isDynamic = MapSetting.GetEditorInstance().ObjectSetting.IsDynamicMapObjectType(mapObjectConfig.ObjectType);
                if (isDynamic && mapObjectData.Go != null)
                {
                    GameObject.DestroyImmediate(mapObjectData.Go);
                    mapObjectData.Go = null;
                }
            }
            return true;
        }

        /// <summary>
        /// 导出地图数据
        /// </summary>
        /// <param name="map"></param>
        public static void ExportMapData(this Map map)
        {
            // 流程上说场景给客户端使用一定会经历导出流程
            // 在导出时确保MapObjectDataMono和地图对象配置数据一致
            // 从而确保场景资源被使用时挂在数据和配置匹配
            map.UpdateAllMapObjectDataMonos();
            var isPrefabAssetInstance = PrefabUtility.IsPartOfPrefabInstance(map.gameObject);
            // 确保数据应用到对应Asset上
            if (isPrefabAssetInstance)
            {
                PrefabUtility.ApplyPrefabInstance(map.gameObject, InteractionMode.AutomatedAction);
            }
            MapExportEditorUtilities.ExportGameMapData(map);
        }

        /// <summary>
        /// 更新所有地图对象的MapObjectDataMono数据到最新
        /// </summary>
        /// <param name="map"></param>
        private static void UpdateAllMapObjectDataMonos(this Map map)
        {
            for (int i = 0; i < map.MapObjectDataList.Count; i++)
            {
                var mapObjectData = map.MapObjectDataList[i];
                if (mapObjectData.Go == null)
                {
                    continue;
                }
                MapEditorUtilities.AddOrUpdateMapObjectDataMono(mapObjectData.Go, mapObjectData.UID);
            }
        }

        /// <summary>
        /// 一键重创地图对象
        /// </summary>
        /// <param name="map"></param>
        public static void OneKeyRecreateMapObjectGos(this Map map)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(map.gameObject))
            {
                return;
            }
            for (int i = 0; i < map.MapObjectDataList.Count; i++)
            {
                var mapObjectData = map.MapObjectDataList[i];
                map.RecreateMapObjectGo(mapObjectData);
            }
            AssetDatabase.SaveAssets();
        }
    }
}
