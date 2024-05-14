/*
 * Description:             MapEditorConst.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using UnityEngine;

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
        public const string MapExportRelativePath = "Assets/Resources/MapExport";

        /// <summary>
        /// 地图对象UID UI显示宽度
        /// </summary>
        public const float MapObjectUIDUIWidth = 50f;

        /// <summary>
        /// 地图对象类型UI显示宽度
        /// </summary>
        public const float MapObjectTypeUIWidth = 150f;

        /// <summary>
        /// 地图对象是否是动态对象UI显示宽度
        /// </summary>
        public const float MapObjectIsDynamicUIWidth = 60f;

        /// <summary>
        /// 地图对象关联配置Id UI显示宽度
        /// </summary>
        public const float MapObjectConfIdUIWidth = 100f;

        /// <summary>
        /// 地图对象Asset UI显示宽度
        /// </summary>
        public const float MapObjectAssetUIWidth = 150f;

        /// <summary>
        /// 地图对象描述UI显示宽度
        /// </summary>
        public const float MapObjectDesUIWidth = 120f;

        /// <summary>
        /// 地图对象预览UI显示宽度
        /// </summary>
        public const float MapObjectPreviewUIWidth = 100f;

        /// <summary>
        /// 地图对象预览UI显示高度
        /// </summary>
        public const float MapObjectPreviewUIHeight = 100f;

        /// <summary>
        /// 地图埋点UID UI显示宽度
        /// </summary>
        public const float MapDataUIDUIWidth = 50f;

        /// <summary>
        /// 地图埋点类型UI显示宽度
        /// </summary>
        public const float MapDataTypeUIWidth = 150f;

        /// <summary>
        /// 地图埋点关联配置Id UI显示宽度
        /// </summary>
        public const float MapDataConfIdUIWidth = 100f;

        /// <summary>
        /// 地图埋点颜色UI显示宽度
        /// </summary>
        public const float MapDataColorUIWidth = 150f;

        /// <summary>
        /// 地图埋点初始旋转显示宽度
        /// </summary>
        public const float MapDataRotationUIWidth = 160f;

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