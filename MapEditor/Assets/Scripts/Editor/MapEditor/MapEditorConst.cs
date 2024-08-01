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
        /// 地图预制件相对目录
        /// </summary>
        public const string MapPrefabFolderRelativePath = "Assets/Resources/Maps";

        /// <summary>
        /// 关卡地图数据保存路径
        /// </summary>
        public const string LevelMapDataSaveFolder = "Asset/Editor/Map/Level/LevelMapData/";

        /// <summary>
        /// 地图对象类型配置UI显示宽度
        /// </summary>
        public const float MapObjectTypeConfigUIWidth = 150f;

        /// <summary>
        /// 地图对象类型配置是否动态对象UI显示宽度
        /// </summary>
        public const float MapObjectTypeConfigIsDynamicUIWidth = 60f;

        /// <summary>
        /// 地图对象描述配置UI显示宽度
        /// </summary>
        public const float MapObjectTypeConfigDesUIWidth = 120f;

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
        public static readonly Vector3 MapDataLabelPosOffset = new Vector3(0f, 0.5f, 0f);

        /// <summary>
        /// 地图埋点数据复制位置偏移(避免复制埋点坐标轴重叠不好操作)
        /// </summary>
        public static readonly Vector3 MapDataDuplicatePositionOffset = new Vector3(1f, 0f, 0f);

        /// <summary>
        /// 地图折叠数量限制
        /// </summary>
        public const int MapFoldNumLimit = 50;

        /// <summary>
        /// 面板地图对象批量的UI宽度
        /// </summary>
        public const float InspectorObjectBatchUIWidth = 40f;

        /// <summary>
        /// 面板地图对象索引的UI宽度
        /// </summary>
        public const float InspectorObjectIndexUIWidth = 40f;

        /// <summary>
        /// 面板地图对象UID的UI宽度
        /// </summary>
        public const float InspectorObjectUIDUIWidth = 150f;

        /// <summary>
        /// 面板地图对象类型的UI宽度
        /// </summary>
        public const float InspectorObjectTypeUIWidth = 150f;

        /// <summary>
        /// 面板地图对象是否动态的UI宽度
        /// </summary>
        public const float InspectorObjectDynamicUIWidth = 60f;

        /// <summary>
        /// 面板地图对象配置Id的UI宽度
        /// </summary>
        public const float InspectorObjectConfIdUIWidth = 100f;

        /// <summary>
        /// 面板地图对象GUI关闭的UI宽度
        /// </summary>
        public const float InspectorObjectGUISwitchOffUIWidth = 60f;

        /// <summary>
        /// 面板地图对象实体对象的UI宽度
        /// </summary>
        public const float InspectorObjectInstanceUIWidth = 100f;

        /// <summary>
        /// 面板地图对象位置的UI宽度
        /// </summary>
        public const float InspectorObjectPositionUIWidth = 160f;

        /// <summary>
        /// 面板地图对象描述的UI宽度
        /// </summary>
        public const float InspectorObjectDesUIWidth = 140f;

        /// <summary>
        /// 面板地图对象上移的UI宽度
        /// </summary>
        public const float InspectorObjectMoveUpUIWidth = 40f;

        /// <summary>
        /// 面板地图对象下移的UI宽度
        /// </summary>
        public const float InspectorObjectMoveDownUIWidth = 40f;

        /// <summary>
        /// 面板地图对象移除的UI宽度
        /// </summary>
        public const float InspectorObjectRemoveUIWidth = 40f;

        /// <summary>
        /// 面板地图对象添加的UI宽度
        /// </summary>
        public const float InspectorObjectAddUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点批量的UI宽度
        /// </summary>
        public const float InspectorDataBatchUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点索引的UI宽度
        /// </summary>
        public const float InspectorDataIndexUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点UID的UI宽度
        /// </summary>
        public const float InspectorDataUIDUIWidth = 150f;

        /// <summary>
        /// 面板地图埋点类型的UI宽度
        /// </summary>
        public const float InspectorDataTypeUIWidth = 150f;

        /// <summary>
        /// 面板地图埋点配置Id的UI宽度
        /// </summary>
        public const float InspectorDataConfIdUIWidth = 100f;

        /// <summary>
        /// 面板地图埋点组Id的UI宽度
        /// </summary>
        public const float InspectorDataGroupIdUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点怪物创建半径的UI宽度
        /// </summary>
        public const float InspectorDataMonsterCreateRadiusUIWidth = 60f;

        /// <summary>
        /// 面板地图埋点怪物警戒半径的UI宽度
        /// </summary>
        public const float InspectorDataMonsterActiveRediusUIWidth = 60f;

        /// <summary>
        /// 面板地图埋点GUI关闭开关的UI宽度
        /// </summary>
        public const float InspectorDataGUISwitchOffUIWidth = 60f;

        /// <summary>
        /// 面板地图埋点位置的UI宽度
        /// </summary>
        public const float InspectorDataPositionUIWidth = 160f;

        /// <summary>
        /// 面板地图埋点旋转的UI宽度
        /// </summary>
        public const float InspectorDataRotationUIWidth = 140f;

        /// <summary>
        /// 面板地图埋点描述的UI宽度
        /// </summary>
        public const float InspectorDataDesUIWidth = 140f;

        /// <summary>
        /// 面板地图埋点上移的UI宽度
        /// </summary>
        public const float InspectorDataMoveUpUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点下移的UI宽度
        /// </summary>
        public const float InspectorDataMoveDownUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点移除的UI宽度
        /// </summary>
        public const float InspectorDataRemoveUIWidth = 40f;

        /// <summary>
        /// 面板地图埋点添加的UI宽度
        /// </summary>
        public const float InspectorDataAddUIWidth = 40f;
    }
}