﻿/*
 * Description:             MapEditorConst.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

namespace MapEditor
{
    /// <summary>
    /// MapEditorConst.cs
    /// 地图编辑器Editor常量
    /// </summary>
    public static class MapEditorConst
    {
        /// <summary>
        /// 地图编辑场景路径
        /// </summary>
        public const string MapEditorScenePath = "Assets/Resources/Scenes/MapEditorScene.unity";

        /// <summary>
        /// 地图导出目录相对路径(相对Asset目录)
        /// </summary>
        public const string MapExportRelativePath = "../Product/Lua/MapData";

        /// <summary>
        /// 地图对象UID UI显示宽度
        /// </summary>
        public const float MapObjectUIDUIWidth = 40f;

        /// <summary>
        /// 地图对象类型UI显示宽度
        /// </summary>
        public const float MapObjectTypeUIWidth = 150f;

        /// <summary>
        /// 地图对象是否是动态对象UI显示宽度
        /// </summary>
        public const float MapObjectIsDynamicUIWidth = 60f;

        /// <summary>
        /// 地图对象Asset UI显示宽度
        /// </summary>
        public const float MapObjectAssetUIWIdth = 150f;

        /// <summary>
        /// 地图对象描述UI显示宽度
        /// </summary>
        public const float MapObjectDesUIWidth = 120f;

        /// <summary>
        /// 地图对象预览UI显示宽度
        /// </summary>
        public const float MapObjectPreviewUIWidth = 100f;

        /// <summary>
        /// 地图埋点UID UI显示宽度
        /// </summary>
        public const float MapDataUIDUIWidth = 40f;

        /// <summary>
        /// 地图埋点类型UI显示宽度
        /// </summary>
        public const float MapDataTypeUIWidth = 150f;

        /// <summary>
        /// 地图埋点颜色UI显示宽度
        /// </summary>
        public const float MapDataColorUIWidth = 150f;

        /// <summary>
        /// 地图埋点描述UI显示宽度
        /// </summary>
        public const float MapDataDesUIWidth = 120f;

        /// <summary>
        /// 地图埋点数据Sphere显示大小
        /// </summary>
        public const float MapDataSphereSize = 0.5f;

        /// <summary>
        /// 地图对象数据标签位置显示偏移
        /// </summary>
        public static readonly Vector3 MapObjectDataLabelPosOffset = new Vector3(0f, 0.5f, 0f);

        /// <summary>
        /// 地图埋点数据标签位置显示偏移
        /// </summary>
        public static readonly Vector3 MapDAtaLabelPosOffset = new Vector3(0f, 0.5f, 0f);
    }
}