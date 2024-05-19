/*
 * Description:             Map.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System.Collections;
using System.Collections.Generic;
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
        /// 地图线条GUI开关
        /// </summary>
        [Header("地图线条GUI开关")]
        public bool MapLineGUISwitch = true;

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
    }
}