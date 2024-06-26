﻿/*
 * Description:             MapConst.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;

namespace MapEditor
{
    /// <summary>
    /// MapConst.cs
    /// 地图编辑器常量
    /// </summary>
    public static class MapConst
    {
        /// <summary>
        /// 游戏地图设置保存路径
        /// </summary>
        public const string MapSettingSavePath = "Assets/Editor/Map/MapSetting.asset";

        /// <summary>
        /// 地图导出目录相对路径(相对Asset目录)
        /// </summary>
        public const string MapExportRelativePath = "Assets/Resources/MapExport";

        /// <summary>
        /// 地图横向默认大小
        /// </summary>
        public const int DefaultMapWidth = 6;

        /// <summary>
        /// 地图纵向默认大小
        /// </summary>
        public const int DefaultMapHeight = 30;

        /// <summary>
        /// 地图九宫格默认大小
        /// </summary>
        public const float DefaultGridSize = 6f;

        /// <summary>
        /// GameObject类型信息
        /// </summary>
        public static readonly Type GameObjectType = typeof(UnityEngine.GameObject);

        /// <summary>
        /// MapObjectType类型信息
        /// </summary>
        public static readonly Type MapObjectType = typeof(MapObjectType);

        /// <summary>
        /// MapDataType类型信息
        /// </summary>
        public static readonly Type MapDataType = typeof(MapDataType);

        /// <summary>
        /// MapTempalateData类型信息
        /// </summary>
        public static readonly Type MapTemplateDataType = typeof(MapTemplateData);

        /// <summary>
        /// 地图对象父节点挂点名
        /// </summary>
        public const string MapObjectParentNodeName = "MapObjectParent";

        /// <summary>
        /// 地图地块节点名
        /// </summary>
        public const string MapTerrianNodeName = "MapTerrian";

        /// <summary>
        /// 默认地图地形预制件路径
        /// </summary>
        public const string DetaulMapTerrianPrefabPath = "Assets/Resources/Terrian/Terrian.prefab";

        /// <summary>
        /// 默认模板策略名
        /// </summary>
        public const string DefaultTemplateStrategyName = "默认策略名";
    }
}