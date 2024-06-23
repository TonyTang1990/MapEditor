/*
 * Description:             MapTemplateDataMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/22
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapTemplateDataMono.cs
    /// 地图锚点模版数据编辑
    /// </summary>
    public class MapTemplateDataMono : MonoBehaviour
    {
        /// <summary>
        /// 场景GUI总开关
        /// </summary>
        [Header("场景GUI总开关")]
        public bool SceneGUISwitch = true;

        /// <summary>
        /// 模版数据Asset
        /// </summary>
        [Header("模版数据Asset")]
        public MapTemplateData TemplateData;

        /// <summary>
        /// 地图埋点数据列表
        /// </summary>
        [Header("地图埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 当前选中需要新增的地图埋点类型
        /// </summary>
        [HideInInspector]
        public int AddMapDataType = (int)MapDataType.PlayerSpawn;

        /// <summary>
        /// 当前选中需要新增的地图埋点索引
        /// </summary>
        [HideInInspector]
        public int AddMapDataIndex = 1;

        /// <summary>
        /// 地图埋点是否展开数据
        /// </summary>
        [HideInInspector]
        public bool MapDataUnfoldData = false;

        /// <summary>
        /// 地图玩家出生点埋点组是否展开数据列表
        /// </summary>
        [HideInInspector]
        public List<bool> PlayerSpawnMapGroupUnfoldDataList = new List<bool>();

        /// <summary>
        /// 地图怪物埋点组是否展开数据列表
        /// </summary>
        [HideInInspector]
        public List<bool> MonsterMapGroupUnfoldDataList = new List<bool>();

        /// <summary>
        /// 地图怪物组埋点组是否展开数据列表
        /// </summary>
        [HideInInspector]
        public List<bool> MonsterGroupMapGroupUnfoldDataList = new List<bool>();

        /// <summary>
        /// 批量勾选起始索引
        /// </summary>
        [HideInInspector]
        public int BatchTickRangeStartIndex;

        /// <summary>
        /// 批量勾选截止索引
        /// </summary>
        [HideInInspector]
        public int BatchTickRangeEndIndex;

#if UNITY_EDITOR
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
            if (TemplateData == null)
            {
                Debug.LogError($"地图埋点模版数据Asset为空，添加地图埋点数据失败！");
                return null;
            }
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return null;
            }
            return TemplateData.AddMapData(uid, gameObject.transform.position, insertIndex, copyRotation, positionOffset);
        }

        /// <summary>
        /// 执行移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public bool DoRemoveMapDataByIndex(int index)
        {
            if(TemplateData == null)
            {
                Debug.LogError($"地图埋点模版数据Asset为空，移除地图埋点数据失败！");
                return false;
            }
            if (!MapUtilities.CheckOperationAvalible(gameObject))
            {
                return false;
            }
            return TemplateData.RemoveMapDataByIndex(index);
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="isOn"></param>
        public bool UpdateAllMapDataBatchOperation(bool isOn)
        {
            if (TemplateData == null)
            {
                Debug.LogError($"地图埋点模版数据Asset为空，更新地图埋点批量操作数据失败！");
                return false;
            }
            return TemplateData.UpdateAllMapDataBatchOperation(isOn);
        }
        /// <summary>
        /// 选中Gizmos显示
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (SceneGUISwitch == false)
            {
                return;
            }
            if (Event.current.type == EventType.Repaint)
            {
                DrawMapDataGizmosGUI();
            }
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
            if (TemplateData == null)
            {
                return;
            }
            for (int i = 0; i < TemplateData.MapDataList.Count; i++)
            {
                var mapData = TemplateData.MapDataList[i];
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if (mapDataConfig == null)
                {
                    continue;
                }
                var mapDataType = mapDataConfig.DataType;
                if (mapDataType == MapDataType.MonsterGroup)
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
            if (monsterGroupMapData == null)
            {
                return;
            }
            if (monsterGroupMapData.GUISwitchOff)
            {
                return;
            }
            var localPosition = monsterGroupMapData.TemplateLocalPosition;
            var worldPosition = transform.position + localPosition;
            var preGizmosColor = Gizmos.color;
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(worldPosition, monsterGroupMapData.MonsterCreateRadius);
            Gizmos.color = preGizmosColor;

            preGizmosColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(worldPosition, monsterGroupMapData.MonsterActiveRadius);
            Gizmos.color = preGizmosColor;
        }
#endif
    }
}