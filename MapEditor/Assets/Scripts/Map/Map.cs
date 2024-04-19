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
        /// 地图对象场景GUI开关
        /// </summary>
        [Header("地图对象场景GUI开关")]
        public bool MapObjectSceneGUISwich = true;

        /// <summary>
        /// 地图埋点场景GUI开关
        /// </summary>
        [Header("地图埋点场景GUI开关")]
        public bool MapDataSceneGUISwich = true;

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
        public List<MapObjectData> MapObjetDataList = new List<MapObjectData>();

        /// <summary>
        /// 地图埋点数据列表
        /// </summary>
        [Header("地图埋点数据列表")]
        [SerializeReference]
        public List<MapData> MapDataList = new List<MapData>();

        /// <summary>
        /// 当前选中需要新增的地图对象索引
        /// </summary>
        [HideInInspector]
        public int AddMapObjectIndex = 1;

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

        }
    }
}