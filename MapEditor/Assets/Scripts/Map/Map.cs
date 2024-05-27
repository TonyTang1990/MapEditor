/*
 * Description:             Map.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// Map.cs
    /// 地图编辑器脚本
    /// </summary>
    public class Map : MonoBehaviour
    {
        /// <summary>
        /// 场景GUI总开关
        /// </summary>
        [Header("场景GUI总开关")]
        public bool SceneGUISwitch = true;

        /// <summary>
        /// 地图网格GUI开关
        /// </summary>
        [Header("地图网格GUI开关")]
        public bool MapLineGUISwitch = true;

        /// <summary>
        /// 地图区域GUI开关
        /// </summary>
        [Header("地图区域GUI开关")]
        public bool MapAreaGUISwitch = false;

        /// <summary>
        /// 地图对象场景GUI开关
        /// </summary>
        [Header("地图对象场景GUI开关")]
        public bool MapObjectSceneGUISwitch = true;

        /// <summary>
        /// 地图埋点场景GUI开关
        /// </summary>
        [Header("地图埋点场景GUI开关")]
        public bool MapDataSceneGUISwitch = true;

        /// <summary>
        /// 地图对象创建自动聚焦开关
        /// </summary>
        [Header("地图对象创建自动聚焦开关")]
        public bool MapObjectAddedAutoFocus = true;

        /// <summary>
        /// 地图横向大小
        /// </summary>
        [Header("地图横向大小")]
        [Range(1, 1000)]
        public int MapWidth;

        /// <summary>
        /// 地图纵向大小
        /// </summary>
        [Header("地图纵向大小")]
        [Range(1, 1000)]
        public int MapHeight;

        /// <summary>
        /// 地图起始位置
        /// </summary>
        [Header("地图起始位置")]
        public Vector3 MapStartPos = Vector3.zero;

        /// <summary>
        /// 区域九宫格大小
        /// </summary>
        [Header("区域九宫格大小")]
        [Range(1f, 100f)]
        public float GridSize;

        /// <summary>
        /// 自定义地形Asset
        /// </summary>
        [Header("自定义地形Asset")]
        public GameObject MapTerrianAsset;

        /// <summary>
        /// 地图对象数据列表
        /// </summary>
        [Header("地图对象数据列表")]
        [SerializeReference]
        public List<MapObjectData> MapObjectDataList = new List<MapObjectData>();

        /// <summary>
        /// 地图埋点数据列表
        /// </summary>
        [Header("地图埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 当前选中需要新增的地图对象类型
        /// </summary>
        [HideInInspector]
        public int AddMapObjectType = (int)MapObjectType.TREASURE_BOX;

        /// <summary>
        /// 当前选中需要新增的地图对象索引
        /// </summary>
        [HideInInspector]
        public int AddMapObjectIndex = 1;

        /// <summary>
        /// 当前选中需要新增的地图埋点类型
        /// </summary>
        [HideInInspector]
        public int AddMapDataType = (int)MapDataType.PLAYER_SPAWN;

        /// <summary>
        /// 当前选中需要新增的地图埋点索引
        /// </summary>
        [HideInInspector]
        public int AddMapDataIndex = 1;

        /// <summary>
        /// 导出类型
        /// </summary>
        [Header("导出类型")]
        public ExportType ExportType;

#if UNITY_EDITOR
        /// <summary>
        /// 执行添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <param name="copyRotation">是否复制旋转</param>
        /// <returns></returns>
        public MapObjectData DoAddMapObjectData(int uid, int insertIndex = -1, bool copyRotation = false)
        {
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return null;
            }
            return AddMapObjectData(uid, insertIndex, copyRotation);
        }

        /// <summary>
        /// 添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <param name="copyRotation">是否复制旋转</param>
        /// <returns></returns>
        private MapObjectData AddMapObjectData(int uid, int insertIndex = -1, bool copyRotation = false)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持添加此地图对象数据！");
                return null;
            }
            var mapObjectDataTotalNum = MapObjectDataList.Count;
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
            var mapObjectPosition = MapStartPos;
            var mapObjectRotation = Quaternion.identity;
            if (mapObjectDataTotalNum != 0)
            {
                var insertMapObjectPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapObjectData = MapObjectDataList[insertMapObjectPos];
                mapObjectPosition = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.position : insertMapObjectData.Position;
                mapObjectRotation = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.rotation : Quaternion.Euler(insertMapObjectData.Rotation);
            }
            var instanceGo = CreateGameObjectByUID(uid);
            if (instanceGo != null && MapObjectAddedAutoFocus)
            {
                Selection.SetActiveObjectWithContext(instanceGo, instanceGo);
            }
            instanceGo.transform.position = mapObjectPosition;
            if(copyRotation)
            {
                instanceGo.transform.rotation = mapObjectRotation;
            }
            var newMapObjectData = new MapObjectData(uid, instanceGo);
            MapObjectDataList.Insert(insertPos, newMapObjectData);
            Debug.Log($"插入地图对象UID:{uid}索引:{insertPos}");
            return newMapObjectData;
        }

        /// <summary>
        /// 获取制定索引的地图对象数据
        /// Note:
        /// 返回null表示无效索引
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public MapObjectData GetMapObjectDataByIndex(int index)
        {
            if (index < 0 || index >= MapObjectDataList.Count)
            {
                return null;
            }
            return MapObjectDataList[index];
        }

        /// <summary>
        /// 获取指定实例对象对应的地图对象数据索引
        /// Note:
        /// 返回-1表示找不到制定实例对象对应的地图对象数据
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        public int GetMapObjectDataIndexByGo(GameObject go)
        {
            return MapObjectDataList.FindIndex(mapObjectData => mapObjectData.Go == go);
        }

        /// <summary>
        /// 执行移除指定索引的地图对象数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DoRemoveMapObjectDataByIndex(int index)
        {
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return false;
            }
            return RemoveMapObjectDataByIndex(index);
        }

        /// <summary>
        /// 移除指定索引的地图对象数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemoveMapObjectDataByIndex(int index)
        {
            var mapObjectDataNum = MapObjectDataList.Count;
            if (index < 0 || index >= mapObjectDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapObjectDataNum - 1},移除地图对象数据失败！");
                return false;
            }
            var mapObjectData = MapObjectDataList[index];
            if (mapObjectData.Go != null)
            {
                mapObjectData.Position = Vector3.zero;
                GameObject.DestroyImmediate(mapObjectData.Go);
            }
            MapObjectDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 执行添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        public MapData DoAddMapData(int uid, int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return null;
            }
            return AddMapData(uid, insertIndex, copyRotation, positionOffset);
        }

        /// <summary>
        /// 添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex">插入位置(-1表示插入尾部)</param>
        /// <param name="copyRotation">是否复制旋转值</param>
        /// <param name="positionOffset">位置偏移</param>
        /// <returns></returns>
        private MapData AddMapData(int uid, int insertIndex = -1, bool copyRotation = false, Vector3? positionOffset = null)
        {
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (mapDataConfig == null)
            {
                Debug.LogError($"未配置地图埋点UID:{uid}配置数据，不支持添加此地图埋点数据！");
                return null;
            }
            var mapDataType = mapDataConfig.DataType;
            var mapDataTotalNum = MapDataList.Count;
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
            var mapDataPosition = MapStartPos;
            var mapDataRotation = mapDataConfig.Rotation;
            if (mapDataTotalNum != 0)
            {
                var insertMapDataPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapData = MapDataList[insertMapDataPos];
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
                if(copyRotation)
                {
                    mapDataRotation = insertMapData != null ? insertMapData.Rotation : mapDataConfig.Rotation;
                }
            }
            if(positionOffset != null)
            {
                mapDataPosition += (Vector3)positionOffset;
            }
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition, mapDataRotation);
            MapDataList.Insert(insertPos, newMapData);
            return newMapData;
        }

        /// <summary>
        /// 执行移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DoRemoveMapDataByIndex(int index)
        {
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return false;
            }
            return RemoveMapDataByIndex(index);
        }

        /// <summary>
        /// 移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemoveMapDataByIndex(int index)
        {
            var mapDataNum = MapDataList.Count;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},移除地图埋点数据失败！");
                return false;
            }
            MapDataList.RemoveAt(index);
            return true;
        }

        /// <summary>
        /// 创建之地给你地图对象UID配置的实体对象(未配置Asset返回null)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public GameObject CreateGameObjectByUID(int uid)
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
                var parentNodeTransform = MapUtilities.GetOrCreateMapObjectTypeParentNode(gameObject, mapObjectType);
                instanceGo.transform.SetParent(parentNodeTransform);
                instanceGo.transform.position = MapStartPos;
                var instanceGoName = instanceGo.name.RemoveSubStr("(Clone)");
                instanceGoName = $"{instanceGoName}_{uid}";
                instanceGo.name = instanceGoName;
                // 动态物体碰撞器统一代码创建
                if (mapObjectConfig.IsDynamic)
                {
                    MapUtilities.AddOrUpdateColliderByColliderDataMono(instanceGo);
                }
                else
                {
                    MapUtilities.UpdateColliderByColliderDataMono(instanceGo);
                }
                MapUtilities.AddOrUpdateMapObjectDataMono(instanceGo, uid);
            }
            return instanceGo;
        }

        /// <summary>
        /// 清除所有地图埋点批量选择
        /// </summary>
        public void ClearAllMapDataBatchOperation()
        {
            for(int i = 0, length = MapDataList.Count; i < length; i++)
            {
                MapDataList[i].BatchOperationSwitch = false;
            }
        }

        /// <summary>
        /// 选中Gizmos显示
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if(SceneGUISwitch == false)
            {
                return;
            }
            if(Event.current.type == EventType.Repaint)
            {
                if(MapObjectSceneGUISwitch)
                {
                    DrawMapObjectDataGizmosGUI();
                }
                if(MapDataSceneGUISwitch)
                {
                    DrawMapDataGizmosGUI();
                }
            }
        }

        /// <summary>
        /// 绘制地图对象数据Gizmos GUI
        /// </summary>
        private void DrawMapObjectDataGizmosGUI()
        {

        }

        /// <summary>
        /// 绘制地图埋点数据Gizmos GUI
        /// </summary>
        private void DrawMapDataGizmosGUI()
        {
            DrawMapCustomDataGizmos();
        }

        /// <summary>
        /// 绘制地图埋点自定义数据显示
        /// </summary>
        private void DrawMapCustomDataGizmos()
        {
            for(int i = 0; i < MapDataList.Count; i++)
            {
                var mapData = MapDataList[i];
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if(mapDataConfig == null)
                {
                    continue;
                }
                var mapDataType = mapDataConfig.DataType;
                if(mapDataType == MapDataType.MONSTER_GROUP)
                {
                    DrawMapMonsterGroupDataGizmos(mapData);
                }
            }
        }

        /// <summary>
        /// 绘制指定地图怪物组埋点数据的自定义数据Gizmos
        /// </summary>
        /// <param name="mapData"></param>
        private void DrawMapMonsterGroupDataGizmos(MapData mapData)
        {
            var monsterGroupMapData = mapData as MonsterGroupMapData;
            if(monsterGroupMapData == null)
            {
                return;
            }
            if(monsterGroupMapData.GUISwitchOff)
            {
                return;
            }
            var position = monsterGroupMapData.Position;
            var preGizmosColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(position, monsterGroupMapData.MonsterCreateRadius);
            Gizmos.color = preGizmosColor;

            preGizmosColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, monsterGroupMapData.MonsterActiveRadius);
            Gizmos.color = preGizmosColor;
        }
#endif
    }
}