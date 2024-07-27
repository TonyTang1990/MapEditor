/*
 * Description:             Map.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        /// 区域GUI开关
        /// </summary>
        [Header("区域GUI开关")]
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
        public int AddMapObjectType = (int)MapObjectType.TreasureBox;

        /// <summary>
        /// 当前选中需要新增的地图对象索引
        /// </summary>
        [HideInInspector]
        public int AddMapObjectIndex = 1;

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
        /// 导出类型
        /// </summary>
        [Header("导出类型")]
        public ExportType ExportType;

        /// <summary>
        /// 导出文件名(不填表示导出跟预制件同名)
        /// </summary>
        [Header("导出文件名(不填表示导出跟预制件同名)")]
        public string CustomExportFileName = string.Empty;

        /// <summary>
        /// 地图对象是否展开数据
        /// </summary>
        [HideInInspector]
        public bool MapObjectDataUnfoldData = false;

        /// <summary>
        /// 地图对象埋点是否组展开数据列表
        /// </summary>
        [HideInInspector]
        public List<bool> MapObjectDataGroupUnfoldDataList = new List<bool>();

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

        /// <summary>
        /// 关卡数据Asset
        /// </summary>
        public LevelMapData LevelMapData;

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
            DrawMapCustomDatasGizmos();
        }

        /// <summary>
        /// 绘制地图埋点自定义数据显示
        /// </summary>
        private void DrawMapCustomDatasGizmos()
        {
            for(int i = 0; i < MapDataList.Count; i++)
            {
                var mapData = MapDataList[i];
                DrawMapCustomDataGizmos(mapData);
            }
        }
        
        /// <summary>
        /// 绘制指定地图埋点数据的自定义数据Gizmos
        /// </summary>
        /// <param name="mapData"></param>
        private void DrawMapCustomDataGizmos(MapData mapData)
        {
            if(mapData == null)
            {
                return;
            }
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
            if (mapDataConfig == null)
            {
                return;
            }
        }
    }
}