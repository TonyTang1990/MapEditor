/*
 * Description:             MapEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.AI.Navigation;
using Unity.AI.Navigation.Editor;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.AI;

namespace MapEditor
{
    /// <summary>
    /// MapEditor.cs
    /// 地图编辑脚本Editor
    /// </summary>
    [CustomEditor(typeof(Map))]
    public class MapEditor : Editor
    {
        /// <summary>
        /// 地图编辑器页签类型
        /// </summary>
        private enum MapTabType
        {
            MapBuild = 0,           // 地图编辑面板
            DataEditor = 1,         // 数据埋点面板
        }

        /// <summary>
        /// 目标组件
        /// </summary>
        private Map mTarget;

        /// <summary>
        /// SceneGUISwitch属性
        /// </summary>
        private SerializedProperty mSceneGUISwitchProperty;

        /// <summary>
        /// MapLineGUISwitch属性
        /// </summary>
        private SerializedProperty mMapLineGUISwitchProperty;

        /// <summary>
        /// MapObjectSceneGUISwitch属性
        /// </summary>
        private SerializedProperty mMapObjectSceneGUISwitchProperty;

        /// <summary>
        /// SceneGUISwitch属性
        /// </summary>
        private SerializedProperty mMapDataSceneGUISwitchProperty;

        /// <summary>
        /// MapObjectAddedAutoFocus属性
        /// </summary>
        private SerializedProperty mMapObjectAddedAutoFocusProperty;

        /// <summary>
        /// MapWidth属性
        /// </summary>
        private SerializedProperty mMapWidthProperty;

        /// <summary>
        /// MapHeight属性
        /// </summary>
        private SerializedProperty mMapHeightProperty;

        /// <summary>
        /// MapStartPos属性
        /// </summary>
        private SerializedProperty mMapStartPosProperty;

        /// <summary>
        /// MapObjectDataList属性
        /// </summary>
        private SerializedProperty mMapObjectDataListProperty;

        /// <summary>
        /// MapDataList属性
        /// </summary>
        private SerializedProperty mMapDataListProperty;

        /// <summary>
        /// mAddMapObjectType属性
        /// </summary>
        private SerializedProperty mAddMapObjectTypeProperty;

        /// <summary>
        /// AddMapObjectValue属性
        /// </summary>
        private SerializedProperty mAddMapObjectIndexProperty;

        /// <summary>
        /// mAddMapDataType属性
        /// </summary>
        private SerializedProperty mAddMapDataTypeProperty;

        /// <summary>
        /// AddMapDataValue属性
        /// </summary>
        private SerializedProperty mAddMapDataIndexProperty;

        /// <summary>
        /// ExportType属性
        /// </summary>
        private SerializedProperty mExportTypeProperty;

        /// <summary>
        /// CustomExportFileName属性
        /// </summary>
        private SerializedProperty mCustomExportFileNameProperty;

        /// <summary>
        /// 所有的折叠数据列表属性
        /// </summary>
        public SerializedProperty mAllGroupUnfoldDataListProperty;

        /// <summary>
        /// 折叠类型和MapFoldTypeData类型折叠数据属性Map
        /// </summary>
        private Dictionary<MapFoldType, SerializedProperty> mMapFoldTypeDataPropertyMap = new Dictionary<MapFoldType, SerializedProperty>();

        ///// <summary>
        ///// MapObjectDataUnfoldData属性
        ///// </summary>
        private SerializedProperty mMapObjectDataUnfoldDataProperty;

        /// <summary>
        /// MapDataUnfoldData属性
        /// </summary>
        private SerializedProperty mMapDataUnfoldDataProperty;

        /// <summary>
        /// BatchTickRangeStartIndex属性
        /// </summary>
        private SerializedProperty mBatchTickRangeStartIndexProperty;

        /// <summary>
        /// BatchTickRangeEndIndex属性
        /// </summary>
        private SerializedProperty mBatchTickRangeEndIndexProperty;

        /// <summary>
        /// LevelMapData属性
        /// </summary>
        private SerializedProperty mLevelMapDataProperty;

        /// <summary>
        /// 红色Label显示GUIStyle
        /// </summary>
        private GUIStyle mRedLabelGUIStyle;

        /// <summary>
        /// 黄色Label显示GUIStyle
        /// </summary>
        private GUIStyle mYellowLabelGUIStyle;

        /// <summary>
        /// 横向绘制线条数据列表<起点, 终点>列表
        /// </summary>
        private List<KeyValuePair<Vector3, Vector3>> mHDrawLinesDataList = new List<KeyValuePair<Vector3, Vector3>>();

        /// <summary>
        /// 纵向绘制线条数据列表<起点, 终点>列表
        /// </summary>
        private List<KeyValuePair<Vector3, Vector3>> mVDrawLinesDataList = new List<KeyValuePair<Vector3, Vector3>>();

        /// <summary>
        /// 九宫格绘制数据<中心点，九宫格UID>列表
        /// </summary>
        private List<KeyValuePair<Vector3, int>> mGridDataList = new List<KeyValuePair<Vector3, int>>();

        /// <summary>
        /// 操作面板标题列表
        /// </summary>
        private string[] mPanelToolBarStrings = { "地图编辑", "数据埋点" };

        /// <summary>
        /// 操作面板选择索引
        /// </summary>
        private int mPanelToolBarSelectIndex = 0;

        /// <summary>
        /// 操作面板选择地图编辑器Tab类型
        /// </summary>
        private MapTabType mSelectedTabType = MapTabType.MapBuild;

        /// <summary>
        /// 当前选中需要增加的地图对象预览Asset
        /// </summary>
        private Texture2D mAddMapObjectPreviewAsset;

        /// <summary>
        /// 地图对象类型和选项数组(显示名字数组)Map<地图对象类型, 选项数组>
        /// </summary>
        private Dictionary<MapObjectType, string[]> mMapObjectDataChoiceOptionsMap = new Dictionary<MapObjectType, string[]>();

        /// <summary>
        /// 地图对象类型和选项值数组(UID数组)Map<地图对象类型， 选项值数组>
        /// </summary>
        private Dictionary<MapObjectType, int[]> mMapObjectDataChoiceValuesMap = new Dictionary<MapObjectType, int[]>();

        /// <summary>
        /// 地图埋点类型和选项数组(显示名字数组)Map<地图埋点类型, 选项数组>
        /// </summary>
        private Dictionary<MapDataType, string[]> mMapDataChoiceOptionsMap = new Dictionary<MapDataType, string[]>();

        /// <summary>
        /// 地图埋点类型和选项值数组(UID数组)Map<地图埋点类型， 选项值数组>
        /// </summary>
        private Dictionary<MapDataType, int[]> mMapDataChoiceValuesMap = new Dictionary<MapDataType, int[]>();

        /// <summary>
        /// 场景按键按下Map<按键值， 是否按下>
        /// </summary>
        private Dictionary<KeyCode, bool> mSceneKeyCodeDownMap = new Dictionary<KeyCode, bool>();

        /// <summary>
        /// Inspector按键按下Map<按键值， 是否按下>
        /// </summary>
        private Dictionary<KeyCode, bool> mInspectorKeyCodeDownMap = new Dictionary<KeyCode, bool>();

        /// <summary>
        /// 埋点数据属性缓存Map<索引, 属性>
        /// Note:
        /// 经测试无论是GetArrayElementAtIndex()还是FindPropertyRelative()这些API调用都很耗时
        /// 这里缓存埋点数据SerializedProperty用于解决每帧重复获取相同索引的埋点数据GetArrayElementAtIndex()调用问题
        /// </summary>
        private Dictionary<int, SerializedProperty> mMapDataProeprtyMapCache = new Dictionary<int, SerializedProperty>();

        /// <summary>
        /// 地图埋点类型和相关数据索引列表缓存Map<地图埋点类型， 对应地图埋点索引列表>
        /// Note:
        /// 经测试无论是GetArrayElementAtIndex()还是FindPropertyRelative()这些API调用都很耗时
        /// 所以在绘制不同地图埋点数据时，这里采用每次使用缓存的地图埋点类型和索引列表数据避免每个类型都全量遍历mMapDataListProperty问题
        /// 从而优化API调用耗时问题
        /// </summary>
        private Dictionary<MapDataType, List<int>> mMapDataTypeIndexsMapCache = new Dictionary<MapDataType, List<int>>();

        private void Awake()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();
            mMapWidthProperty.intValue = mMapWidthProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapWidth : mMapWidthProperty.intValue;
            mMapHeightProperty.intValue = mMapHeightProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapHeight : mMapHeightProperty.intValue;
            UpdateMapDataTypeIndexDatas();
            CreateAllNodes();
            UpdateMapObjectDataChoiceDatas();
            UpdateMapDataChoiceDatas();
            InitMapUnfoldDatas();
            UpdateAddMapObjectDataPreviewAsset();
            UpdateMapSizeDrawDatas();
            UpdateMapGOPosition();
            UpdateTerrianSizeAndPos();
            UpdateMapObjectDataLogicDatas();
            CorrectAddMapObjectIndexValue();
            CorrectAddMapDataIndexValue();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 初始化目标组件
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (target as Map);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitProperties()
        {
            mSceneGUISwitchProperty ??= serializedObject.FindProperty("SceneGUISwitch");
            mMapLineGUISwitchProperty ??= serializedObject.FindProperty("MapLineGUISwitch");
            mMapObjectSceneGUISwitchProperty ??= serializedObject.FindProperty("MapObjectSceneGUISwitch");
            mMapDataSceneGUISwitchProperty ??= serializedObject.FindProperty("MapDataSceneGUISwitch");
            mMapObjectAddedAutoFocusProperty ??= serializedObject.FindProperty("MapObjectAddedAutoFocus");
            mMapWidthProperty ??= serializedObject.FindProperty("MapWidth");
            mMapHeightProperty ??= serializedObject.FindProperty("MapHeight");
            mMapStartPosProperty ??= serializedObject.FindProperty("MapStartPos");
            mMapObjectDataListProperty ??= serializedObject.FindProperty("MapObjectDataList");
            mMapDataListProperty ??= serializedObject.FindProperty("MapDataList");
            mAddMapObjectTypeProperty ??= serializedObject.FindProperty("AddMapObjectType");
            mAddMapObjectIndexProperty ??= serializedObject.FindProperty("AddMapObjectIndex");
            mAddMapDataTypeProperty ??= serializedObject.FindProperty("AddMapDataType");
            mAddMapDataIndexProperty ??= serializedObject.FindProperty("AddMapDataIndex");
            mExportTypeProperty ??= serializedObject.FindProperty("ExportType");
            mCustomExportFileNameProperty ??= serializedObject.FindProperty("CustomExportFileName");
            mAllGroupUnfoldDataListProperty ??= serializedObject.FindProperty("AllGroupUnfoldDataList");
            mBatchTickRangeStartIndexProperty ??= serializedObject.FindProperty("BatchTickRangeStartIndex");
            mBatchTickRangeEndIndexProperty ??= serializedObject.FindProperty("BatchTickRangeEndIndex");
            mLevelMapDataProperty ??= serializedObject.FindProperty("LevelMapData");
        }

        /// <summary>
        /// 初始化GUIStyles
        /// </summary>
        private void InitGUIStyles()
        {
            if (mRedLabelGUIStyle == null)
            {
                mRedLabelGUIStyle = new GUIStyle();
                mRedLabelGUIStyle.fontSize = 15;
                mRedLabelGUIStyle.alignment = TextAnchor.MiddleCenter;
                mRedLabelGUIStyle.normal.textColor = Color.red;
            }
            if (mYellowLabelGUIStyle == null)
            {
                mYellowLabelGUIStyle = new GUIStyle();
                mYellowLabelGUIStyle.fontSize = 15;
                mYellowLabelGUIStyle.alignment = TextAnchor.MiddleCenter;
                mYellowLabelGUIStyle.normal.textColor = Color.yellow;
            }
        }

        /// <summary>
        /// 更新地图埋点类型和索引列表数据
        /// </summary>
        private void UpdateMapDataTypeIndexDatas()
        {
            mMapDataTypeIndexsMapCache.Clear();
            List<int> mapDataTypeIndexs;
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                if (mapDataProperty == null)
                {
                    continue;
                }
                var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                mapDataTypeIndexs = GetMapDataTypeIndexs(mapDataConfig.DataType);
                if (mapDataTypeIndexs == null)
                {
                    mapDataTypeIndexs = new List<int>();
                    mMapDataTypeIndexsMapCache.Add(mapDataConfig.DataType, mapDataTypeIndexs);
                }
                mapDataTypeIndexs.Add(i);
            }
        }

        /// <summary>
        /// 获取指定地图埋点类型的所有数据索引列表
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private List<int> GetMapDataTypeIndexs(MapDataType mapDataType)
        {
            List<int> mapDataTypeIndexs;
            if (!mMapDataTypeIndexsMapCache.TryGetValue(mapDataType, out mapDataTypeIndexs))
            {

            }
            return mapDataTypeIndexs;
        }

        /// <summary>
        /// 清除地图埋点类型的所有数据索引列表
        /// </summary>
        private void ClearMapDataTypeIndexsMapCache()
        {
            mMapDataTypeIndexsMapCache.Clear();
        }

        /// <summary>
        /// 创建所有节点
        /// </summary>
        private void CreateAllNodes()
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            CreateAllMapObjectParentNodes();
            CreateMapTerrianNode();
            CreateNavMeshSurface();
            CreateVirtualCameraNode();
        }

        /// <summary>
        /// 创建所有地图对象父节点挂点
        /// </summary>
        private void CreateAllMapObjectParentNodes()
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapObjectParentNode = MapEditorUtilities.GetOrCreateMapObjectParentNode(mTarget?.gameObject);
            if (mapObjectParentNode != null)
            {
                mapObjectParentNode.transform.localPosition = Vector3.zero;
            }

            var mapObjectTypeValues = Enum.GetValues(MapConst.MapObjectType);
            foreach (var mapObjectTypeValue in mapObjectTypeValues)
            {
                var mapObjectType = (MapObjectType)mapObjectTypeValue;
                var mapObjectTypeParentNodeTransform = MapEditorUtilities.GetOrCreateMapObjectTypeParentNode(mTarget?.gameObject, mapObjectType);
                mapObjectTypeParentNodeTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 创建地图地形节点
        /// </summary>
        private void CreateMapTerrianNode()
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject);
            if (mapTerrianTransform != null)
            {
                mapTerrianTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 创建寻路组件
        /// </summary>
        private void CreateNavMeshSurface()
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            MapEditorUtilities.GetOrCreateNavMeshSurface(mTarget?.gameObject);
        }

        /// <summary>
        /// 创建虚拟摄像机节点
        /// </summary>
        private void CreateVirtualCameraNode()
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            MapEditorUtilities.GetOrCreateGameVirtualCameraNode(mTarget?.gameObject);
        }

        /// <summary>
        /// 更新地图地块大小和位置
        /// </summary>
        private void UpdateTerrianSizeAndPos()
        {
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject);
            if (mapTerrianTransform == null)
            {
                return;
            }

            var mapWidth = mMapWidthProperty.intValue;
            var mapHeight = mMapHeightProperty.intValue;
            mapTerrianTransform.localScale = new Vector3(0.1f * mapWidth, 1, 0.1f * mapHeight);
            mapTerrianTransform.localPosition = new Vector3(mapWidth / 2, 0, mapHeight / 2);
            var meshRenderer = mapTerrianTransform.GetComponent<MeshRenderer>();
            if (meshRenderer != null)
            {
                meshRenderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(mapWidth, mapHeight));
            }
        }

        /// <summary>
        /// 更新地图对象数据的逻辑数据(对象存在时才更新记录逻辑数据)
        /// </summary>
        private void UpdateMapObjectDataLogicDatas()
        {
            // 避免选中Asset也会触发逻辑数据更新导致预制件触发变化
            var gameObjectStatus = MapEditorUtilities.GetGameObjectStatus(mTarget.gameObject);
            if (gameObjectStatus == GameObjectStatus.INVALIDE || gameObjectStatus == GameObjectStatus.Asset)
            {
                return;
            }
            // 地图对象可能删除还原，所以需要逻辑层面记录数据
            for (int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                var mapObjectUID = uidProperty.intValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
                if (mapObjectConfig == null)
                {
                    continue;
                }
                var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
                if (goProperty.objectReferenceValue != null)
                {
                    var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                    var rotationProperty = mapObjectDataProperty.FindPropertyRelative("Rotation");
                    var localScaleProperty = mapObjectDataProperty.FindPropertyRelative("LocalScale");
                    var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                    var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                    var mapObjectGO = goProperty.objectReferenceValue as GameObject;
                    positionProperty.vector3Value = mapObjectGO.transform.position;
                    rotationProperty.vector3Value = mapObjectGO.transform.rotation.eulerAngles;
                    localScaleProperty.vector3Value = mapObjectGO.transform.localScale;
                    var boxCollider = mapObjectGO.GetComponent<BoxCollider>();
                    if (boxCollider != null)
                    {
                        colliderCenterProperty.vector3Value = boxCollider.center;
                        colliderSizeProperty.vector3Value = boxCollider.size;
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 矫正添加地图对象索引值(避免因为配置面板修改导致索引超出有效范围问题)
        /// </summary>
        private void CorrectAddMapObjectIndexValue()
        {
            if (mMapObjectDataChoiceOptionsMap == null || mMapObjectDataChoiceOptionsMap.Count == 0)
            {
                return;
            }
            var addMapObjectType = (MapObjectType)mAddMapObjectTypeProperty.intValue;
            var mapObjectDataValueOptions = mMapObjectDataChoiceValuesMap[addMapObjectType];
            var addMapObjectIndexValue = mAddMapObjectIndexProperty.intValue;
            if (!MapEditorUtilities.IsIntValueInArrays(addMapObjectIndexValue, mapObjectDataValueOptions))
            {
                mAddMapObjectIndexProperty.intValue = mapObjectDataValueOptions[0];
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 矫正添加地图埋点索引值(避免因为配置面板修改导致索引超出有效范围问题)
        /// </summary>
        private void CorrectAddMapDataIndexValue()
        {
            if (mMapDataChoiceOptionsMap == null || mMapDataChoiceOptionsMap.Count == 0)
            {
                return;
            }
            var addMapDataType = (MapDataType)mAddMapDataTypeProperty.intValue;
            var mapDataValueOptions = mMapDataChoiceValuesMap[addMapDataType];
            var addMapDataIndexValue = mAddMapDataIndexProperty.intValue;
            if (!MapEditorUtilities.IsIntValueInArrays(addMapDataIndexValue, mapDataValueOptions))
            {
                if (mapDataValueOptions != null && mapDataValueOptions.Length > 0)
                {
                    mAddMapDataIndexProperty.intValue = mapDataValueOptions[0];
                }
                else
                {
                    mAddMapDataIndexProperty.intValue = 0;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 更新地图对象选择数据
        /// </summary>
        private void UpdateMapObjectDataChoiceDatas()
        {
            var mapObjectTypeValues = Enum.GetValues(MapConst.MapObjectType);
            mMapObjectDataChoiceOptionsMap.Clear();
            mMapObjectDataChoiceValuesMap.Clear();
            foreach (var mapObjectTypeValue in mapObjectTypeValues)
            {
                var mapObjectType = (MapObjectType)mapObjectTypeValue;
                (string[] allChoiceOptions, int[] allValueOptions) = MapEditorUtilities.GetMapObjectChoiceInfosByType(mapObjectType);
                mMapObjectDataChoiceOptionsMap.Add(mapObjectType, allChoiceOptions);
                mMapObjectDataChoiceValuesMap.Add(mapObjectType, allValueOptions);
            }
        }

        /// <summary>
        /// 获取指定地图对象类型的选项数组
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        private string[] GetMapObjectDataChoiceOptionsByType(MapObjectType mapObjectType)
        {
            string[] mapObjectDataChoiceOptions;
            if (!mMapObjectDataChoiceOptionsMap.TryGetValue(mapObjectType, out mapObjectDataChoiceOptions))
            {
                mapObjectDataChoiceOptions = new string[0];
            }
            return mapObjectDataChoiceOptions;
        }

        /// <summary>
        /// 获取指定地图对象类型的选项值数组
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        private int[] GetMapObjectDataChoiceValuesByType(MapObjectType mapObjectType)
        {
            int[] mapObjectDataChoiceValues;
            if (!mMapObjectDataChoiceValuesMap.TryGetValue(mapObjectType, out mapObjectDataChoiceValues))
            {
                mapObjectDataChoiceValues = new int[0];
            }
            return mapObjectDataChoiceValues;
        }

        /// <summary>
        /// 更新地图埋点选择数据
        /// </summary>
        private void UpdateMapDataChoiceDatas()
        {
            var mapDataTypeValues = Enum.GetValues(MapConst.MapDataType);
            mMapDataChoiceOptionsMap.Clear();
            mMapDataChoiceValuesMap.Clear();
            foreach (var mapDataTypeValue in mapDataTypeValues)
            {
                var mapDataType = (MapDataType)mapDataTypeValue;
                (string[] allChoiceOptions, int[] allValueOptions) = MapEditorUtilities.GetMapDataChoiceInfosByType(mapDataType);
                mMapDataChoiceOptionsMap.Add(mapDataType, allChoiceOptions);
                mMapDataChoiceValuesMap.Add(mapDataType, allValueOptions);
            }
        }

        /// <summary>
        /// 更新自定义导出文件名
        /// </summary>
        /// <param name="customExportFileName"></param>
        private void UpdateCustomExportFileName(string customExportFileName = "")
        {
            mCustomExportFileNameProperty.stringValue = customExportFileName;
        }

        /// <summary>
        /// 初始化地图展开数据
        /// </summary>
        private void InitMapUnfoldDatas()
        {
            InitMapObjectDataUnfoldDatas();
            InitMapDataUnfoldDatas();
        }

        /// <summary>
        /// 初始化地图对象展开数据
        /// </summary>
        private void InitMapObjectDataUnfoldDatas()
        {
            for (int i = 0, length = mMapObjectDataListProperty.arraySize; i < length; i++)
            {
                if (i % MapEditorConst.MapFoldNumLimit != 0)
                {
                    continue;
                }
                var index = GetMapFoldIndex(i);
                if (ExistMapUnfold(MapFoldType.MapObjectDataFold, index))
                {
                    continue;
                }
                AddMapGroupUnfoldData(MapFoldType.MapObjectDataFold, index);
            }
        }

        /// <summary>
        /// 初始化地图埋点展开数据
        /// </summary>
        private void InitMapDataUnfoldDatas()
        {
            var mapDataTypeValues = Enum.GetValues(MapConst.MapDataType);
            foreach (var mapDataType in mapDataTypeValues)
            {
                var realMapDataType = (MapDataType)mapDataType;
                InitUnfoldDataByType(realMapDataType);
            }
        }

        /// <summary>
        /// 初始化制定地图埋点类型折叠数据
        /// </summary>
        /// <param name="mapDataType"></param>
        private void InitUnfoldDataByType(MapDataType mapDataType)
        {
            var mapDataTypeIndexs = GetMapDataTypeIndexs(mapDataType);
            if (mapDataTypeIndexs == null)
            {
                return;
            }
            var mapFoldType = MapEditorUtilities.GetMapDataFoldType(mapDataType);
            for (int i = 0, length = mapDataTypeIndexs.Count; i < length; i++)
            {
                if (i % MapEditorConst.MapFoldNumLimit != 0)
                {
                    continue;
                }
                var index = GetMapFoldIndex(i);
                if (ExistMapUnfold(mapFoldType, index))
                {
                    continue;
                }
                AddMapGroupUnfoldData(mapFoldType, index);
            }
        }

        /// <summary>
        /// 获取指定地图埋点类型的选项数组
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private string[] GetMapDataChoiceOptionsByType(MapDataType mapDataType)
        {
            string[] mapDataChoiceOptions;
            if (!mMapDataChoiceOptionsMap.TryGetValue(mapDataType, out mapDataChoiceOptions))
            {
                mapDataChoiceOptions = new string[0];
            }
            return mapDataChoiceOptions;
        }

        /// <summary>
        /// 获取指定地图埋点类型的选项值数组
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private int[] GetMapDataChoiceValuesByType(MapDataType mapDataType)
        {
            int[] mapDataChoiceValues;
            if (!mMapDataChoiceValuesMap.TryGetValue(mapDataType, out mapDataChoiceValues))
            {
                mapDataChoiceValues = new int[0];
            }
            return mapDataChoiceValues;
        }

        /// <summary>
        /// 更新添加选择的地图对象预览Asset
        /// </summary>
        private void UpdateAddMapObjectDataPreviewAsset()
        {
            var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
            if (addMapObjectValue != 0)
            {
                var addMapObjectUID = addMapObjectValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(addMapObjectUID);
                if (mapObjectConfig != null && mapObjectConfig.Asset != null)
                {
                    mAddMapObjectPreviewAsset = MapEditorUtilities.GetAssetPreview(mapObjectConfig.Asset);
                }
                else
                {
                    mAddMapObjectPreviewAsset = null;
                }
            }
        }

        /// <summary>
        /// 更新地图绘制数据
        /// </summary>
        private void UpdateMapSizeDrawDatas()
        {
            //Debug.Log($"更新地图大小绘制数据！");
            mHDrawLinesDataList.Clear();
            mVDrawLinesDataList.Clear();
            var startPos = mMapStartPosProperty.vector3Value;
            var mapHorizontalLineNum = mMapWidthProperty.intValue + 1;
            var mapVerticalLineNum = mMapHeightProperty.intValue + 1;
            var totalMapWidth = mMapWidthProperty.intValue;
            var totalMapHeight = mMapHeightProperty.intValue;
            for (int i = 0; i < mapVerticalLineNum; i++)
            {
                var fromPos = startPos;
                fromPos.z = fromPos.z + i;
                var toPos = fromPos;
                toPos.x = toPos.x + totalMapWidth;
                mHDrawLinesDataList.Add(new KeyValuePair<Vector3, Vector3>(fromPos, toPos));
            }

            for (int j = 0; j < mapHorizontalLineNum; j++)
            {
                var fromPos = startPos;
                fromPos.x = fromPos.x + j;
                var toPos = fromPos;
                toPos.z = toPos.z + totalMapHeight;
                mVDrawLinesDataList.Add(new KeyValuePair<Vector3, Vector3>(fromPos, toPos));
            }
        }

        /// <summary>
        /// 更新地图对象位置
        /// </summary>
        private void UpdateMapGOPosition()
        {
            if (mTarget == null)
            {
                return;
            }
            mTarget.transform.position = mMapStartPosProperty.vector3Value;
        }

        /// <summary>
        /// 执行将指定属性对象和指定索引的数据向上移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoMovePropertyDataUpByIndex(SerializedProperty propertyList, int index)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return false;
            }
            var result = MapEditorUtilities.MovePropertyDataUpByIndex(propertyList, index);
            if (result)
            {
                UpdateMapDataTypeIndexDatas();
            }
            return result;
        }

        /// <summary>
        /// 执行将指定属性对象和指定索引的数据向下移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoMovePropertyDataDownByIndex(SerializedProperty propertyList, int index)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return false;
            }
            var result = MapEditorUtilities.MovePropertyDataDownByIndex(propertyList, index);
            if (result)
            {
                UpdateMapDataTypeIndexDatas();
            }
            return result;
        }

        /// <summary>
        /// 执行添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private MapObjectData DoAddMapObjectData(int uid, int insertIndex = -1)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return null;
            }
            return AddMapObjectData(uid, insertIndex);
        }

        /// <summary>
        /// 添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private MapObjectData AddMapObjectData(int uid, int insertIndex = -1)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持添加此地图对象数据！");
                return null;
            }
            var mapObjectDataTotalNum = mMapObjectDataListProperty.arraySize;
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
            var mapObjectPosition = mMapStartPosProperty.vector3Value;
            if (mapObjectDataTotalNum != 0)
            {
                var insertMapObjectPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapObjectProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertMapObjectPos);
                var insertMapObjectData = insertMapObjectProperty.managedReferenceValue as MapObjectData;
                mapObjectPosition = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.position : insertMapObjectData.Position;
            }
            var instanceGo = mTarget?.CreateGameObjectByUID(uid);
            if(instanceGo == null)
            {
                Debug.LogError($"地图对象UID:{uid}未绑定实体对象，创建地图对象失败！");
                return null;
            }
            if (instanceGo != null && mMapObjectAddedAutoFocusProperty.boolValue)
            {
                Selection.SetActiveObjectWithContext(instanceGo, instanceGo);
            }
            instanceGo.transform.position = mapObjectPosition;
            var newMapObjectData = new MapObjectData(uid, instanceGo);
            mMapObjectDataListProperty.InsertArrayElementAtIndex(insertPos);
            var newMapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertPos);
            newMapObjectDataProperty.managedReferenceValue = newMapObjectData;
            serializedObject.ApplyModifiedProperties();
            UpdateMapObjectDataLogicDatas();
            return newMapObjectData;
        }

        /// <summary>
        /// 执行移除指定索引的地图对象数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoRemoveMapObjectDataByIndex(int index)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
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
            var mapObjectDataNum = mMapObjectDataListProperty.arraySize;
            if (index < 0 || index >= mapObjectDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapObjectDataNum - 1},移除地图对象数据失败！");
                return false;
            }
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(index);
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            if (goProperty.objectReferenceValue != null)
            {
                var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                positionProperty.vector3Value = Vector3.zero;
                GameObject.DestroyImmediate(goProperty.objectReferenceValue);
            }
            mMapObjectDataListProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            UpdateMapObjectDataLogicDatas();
            return true;
        }

        /// <summary>
        /// 执行添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private MapData DoAddMapData(int uid, int insertIndex = -1)
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return null;
            }
            return AddMapData(uid, insertIndex);
        }

        /// <summary>
        /// 添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private MapData AddMapData(int uid, int insertIndex = -1)
        {
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (mapDataConfig == null)
            {
                Debug.LogError($"未配置地图埋点UID:{uid}配置数据，不支持添加此地图埋点数据！");
                return null;
            }
            var mapDataType = mapDataConfig.DataType;
            var mapDataTotalNum = mMapDataListProperty.arraySize;
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
            var mapDataPosition = mMapStartPosProperty.vector3Value;
            if (mapDataTotalNum != 0)
            {
                var insertMapDataPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapDataProperty = GetMapDataSerializedPropertyByIndex(insertMapDataPos);
                var insertMapData = insertMapDataProperty.managedReferenceValue as MapData;
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
            }
            var defaultMonsterCreateRadius = MapConst.DefaultMonsterCreateRadius;
            var defaultMonsterActiveRadius = MapConst.DefaultMonsterActiveRadius;
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition, mapDataConfig.Rotation, defaultMonsterCreateRadius, defaultMonsterActiveRadius);
            mMapDataListProperty.InsertArrayElementAtIndex(insertPos);
            var newMapDataProperty = GetMapDataSerializedPropertyByIndex(insertPos);
            newMapDataProperty.managedReferenceValue = newMapData;
            serializedObject.ApplyModifiedProperties();
            ClearMapDataSerializedPropertyCache();
            UpdateMapDataTypeIndexDatas();
            return newMapData;
        }

        /// <summary>
        /// 执行移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoRemoveMapDataByIndex(int index)
        {
            if (!MapEditorUtilities.IsOperationAvalible(mTarget?.gameObject))
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
            var mapDataNum = mMapDataListProperty.arraySize;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},移除地图埋点数据失败！");
                return false;
            }
            mMapDataListProperty.DeleteArrayElementAtIndex(index);
            serializedObject.ApplyModifiedProperties();
            ClearMapDataSerializedPropertyCache();
            UpdateMapDataTypeIndexDatas();
            return true;
        }

        /// <summary>
        /// 执行保存埋点数据到关卡数据
        /// </summary>
        /// <returns></returns>
        private bool DoSaveToLevelMapDataAsset()
        {
            if(mLevelMapDataProperty.objectReferenceValue != null)
            {
                var levelMapDataAsset = mLevelMapDataProperty.objectReferenceValue as LevelMapData;
                levelMapDataAsset.InitDataFromMap(mTarget);
                EditorUtility.SetDirty(levelMapDataAsset);
                AssetDatabase.SaveAssets();
                var levelMapAssetPath = AssetDatabase.GetAssetPath(levelMapDataAsset);
                Debug.Log($"指定关卡数据Asset:{levelMapAssetPath}保存成功！");
                return true;
            }
            var defaultSaveFoldPath = PathUtilities.GetAssetFullPath(MapEditorConst.LevelMapDataSaveFolder);
            FolderUtilities.CheckAndCreateSpecificFolder(defaultSaveFoldPath);
            string saveFilePath = EditorUtility.SaveFilePanel("关卡数据保存", MapEditorConst.LevelMapDataSaveFolder, "MapTemplateData", "asset");
            if(string.IsNullOrEmpty(saveFilePath))
            {
                Debug.Log($"未选择有效保存路径，保存关卡数据失败！");
                return false;
            }
            var newLevelMapDataAsset = ScriptableObject.CreateInstance<LevelMapData>();
            newLevelMapDataAsset.InitDataFromMap(mTarget);
            var saveFileFoldPath = Path.GetDirectoryName(saveFilePath);
            FolderUtilities.CheckAndCreateSpecificFolder(saveFileFoldPath);
            var saveFileAssetPath = PathUtilities.GetProjectRelativeFolderPath(saveFilePath);
            AssetDatabase.CreateAsset(newLevelMapDataAsset, saveFileAssetPath);
            mLevelMapDataProperty.objectReferenceValue = newLevelMapDataAsset;
            AssetDatabase.SaveAssets();
            Debug.Log($"新关卡数据Asset:{saveFileFoldPath}保存成功！");
            return true;
        }

        /// <summary>
        /// 标记场景指定按键按下
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkSceneKeyDown(KeyCode keyCode)
        {
            if (!mSceneKeyCodeDownMap.ContainsKey(keyCode))
            {
                mSceneKeyCodeDownMap.Add(keyCode, true);
            }
            else
            {
                mSceneKeyCodeDownMap[keyCode] = true;
            }
        }

        /// <summary>
        /// 标记场景指定按键释放
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkSceneKeyUp(KeyCode keyCode)
        {
            if (!mSceneKeyCodeDownMap.ContainsKey(keyCode))
            {
                mSceneKeyCodeDownMap.Add(keyCode, false);
            }
            else
            {
                mSceneKeyCodeDownMap[keyCode] = false;
            }
        }

        /// <summary>
        /// 标记所有场景按键释放
        /// </summary>
        private void UnmarkAllSceneKeyUp()
        {
            if (mSceneKeyCodeDownMap.Count == 0)
            {
                return;
            }
            mSceneKeyCodeDownMap.Clear();
        }

        /// <summary>
        /// 指定场景按键是否按下
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        private bool IsSceneKeyCodeDown(KeyCode keyCode)
        {
            bool result = false;
            if (mSceneKeyCodeDownMap.TryGetValue(keyCode, out result))
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// 标记Inspector指定按键按下
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkInspectorKeyDown(KeyCode keyCode)
        {
            if (!mInspectorKeyCodeDownMap.ContainsKey(keyCode))
            {
                mInspectorKeyCodeDownMap.Add(keyCode, true);
            }
            else
            {
                mInspectorKeyCodeDownMap[keyCode] = true;
            }
        }

        /// <summary>
        /// 标记Inspector指定按键释放
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkInspectorKeyUp(KeyCode keyCode)
        {
            if (!mInspectorKeyCodeDownMap.ContainsKey(keyCode))
            {
                mInspectorKeyCodeDownMap.Add(keyCode, false);
            }
            else
            {
                mInspectorKeyCodeDownMap[keyCode] = false;
            }
        }

        /// <summary>
        /// 标记所有Inspector按键释放
        /// </summary>
        private void UnmarkAllInspectorKeyUp()
        {
            if (mInspectorKeyCodeDownMap.Count == 0)
            {
                return;
            }
            mInspectorKeyCodeDownMap.Clear();
        }

        /// <summary>
        /// 指定Inspector按键是否按下
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        private bool IsInspectorKeyCodeDown(KeyCode keyCode)
        {
            bool result = false;
            if (mInspectorKeyCodeDownMap.TryGetValue(keyCode, out result))
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// 检查地图埋点数据长度变化
        /// Note:
        /// 用于非Editor支持复制数据，Editor这方因为缓存检测不到数据变化导致的显示不及时问题
        /// </summary>
        private void CheckMapDataLengthChange()
        {
            if (mMapDataProeprtyMapCache.Count == mMapDataListProperty.arraySize)
            {
                return;
            }
            ClearMapDataSerializedPropertyCache();
            UpdateMapDataTypeIndexDatas();
            Debug.Log($"检测到地图埋点数据长度变化，相关缓存数据更新重置！");
        }

        /// <summary>
        /// 检查面板按键事件
        /// </summary>
        private void CheckInspectorKeyCodeEvent()
        {
            var currentEvent = Event.current;
            if(currentEvent.type == EventType.KeyDown)
            {
                MarkInspectorKeyDown(currentEvent.keyCode);
            }
            if(currentEvent.type == EventType.KeyUp)
            {
                MarkInspectorKeyUp(currentEvent.keyCode);
            }
        }

        /// <summary>
        /// 指定索引的地图埋点数据SerializedProeprty
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private SerializedProperty GetMapDataSerializedPropertyByIndex(int index)
        {
            SerializedProperty mapDataProperty;
            if (!mMapDataProeprtyMapCache.TryGetValue(index, out mapDataProperty))
            {
                var mapDataLength = mMapDataListProperty.arraySize;
                if (index >= mapDataLength)
                {
                    return mapDataProperty;
                }
                mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(index);
                mMapDataProeprtyMapCache.Add(index, mapDataProperty);
            }
            return mapDataProperty;
        }

        /// <summary>
        /// 清除地图埋点数据SerializedProperty缓存
        /// </summary>
        private void ClearMapDataSerializedPropertyCache()
        {
            mMapDataProeprtyMapCache.Clear();
            Debug.Log($"清理地图埋点数据SerializedProperty缓存！");
        }

        /// <summary>
        /// 一键操作地图批量勾选
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchBatchOperation(bool isOn)
        {
            var mapTabType = (MapTabType)mPanelToolBarSelectIndex;
            if (mapTabType == MapTabType.MapBuild)
            {
                UpdateAllMapObjectDataBatchOperation(isOn);
            }
            else if (mapTabType == MapTabType.DataEditor)
            {
                UpdateAllMapDataBatchOperation(isOn);
            }
        }

        /// <summary>
        /// 一键勾选指定范围的地图批量数据
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchRangeOperation(bool isOn)
        {
            var mapTabType = (MapTabType)mPanelToolBarSelectIndex;
            if (mapTabType == MapTabType.MapBuild)
            {
                OneKeySwitchMapObjectDataRangeOperation(isOn);
            }
            else if (mapTabType == MapTabType.DataEditor)
            {
                OneKeySwitchMapDataRangeOperation(isOn);
            }
        }

        /// <summary>
        /// 一键膝盖地图对象批量数据
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchMapObjectDataRangeOperation(bool isOn)
        {
            var totalMapDataNum = mMapObjectDataListProperty.arraySize;
            var startIndex = mBatchTickRangeStartIndexProperty.intValue;
            var endIndex = mBatchTickRangeEndIndexProperty.intValue;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i < 0)
                {
                    continue;
                }
                if (i >= totalMapDataNum)
                {
                    break;
                }
                UpdateMapObjectDataBatchOperationByIndex(i, isOn);
            }
        }

        /// <summary>
        /// 一键修改地图埋点批量数据
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchMapDataRangeOperation(bool isOn)
        {
            var totalMapDataNum = mMapDataListProperty.arraySize;
            var startIndex = mBatchTickRangeStartIndexProperty.intValue;
            var endIndex = mBatchTickRangeEndIndexProperty.intValue;
            for (int i = startIndex; i <= endIndex; i++)
            {
                if (i < 0)
                {
                    continue;
                }
                if (i >= totalMapDataNum)
                {
                    break;
                }
                UpdateMapDataBatchOperationByIndex(i, isOn);
            }
        }

        /// <summary>
        /// 更新所有地图对象批量选择
        /// </summary>
        /// <param name="isOn"></param>
        private void UpdateAllMapObjectDataBatchOperation(bool isOn)
        {
            for (int i = 0, length = mMapObjectDataListProperty.arraySize; i < length; i++)
            {
                UpdateMapObjectDataBatchOperationByIndex(i, isOn);
            }
        }

        /// <summary>
        /// 更新指定地图对象类型的批量选择
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="mapObjectType"></param>
        private void UpdateMapObjectDataBatchOperationByType(bool isOn, MapObjectType mapObjectType)
        {
            for (int i = 0, length = mMapObjectDataListProperty.arraySize; i < length; i++)
            {
                var mapObjectProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var uid = mapObjectProperty.FindPropertyRelative("UID").intValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
                if(mapObjectConfig.ObjectType == mapObjectType)
                {
                    UpdateMapObjectDataBatchOperationByIndex(i, isOn);
                }
            }
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="isOn"></param>
        private void UpdateAllMapDataBatchOperation(bool isOn)
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                UpdateMapDataBatchOperationByIndex(i, isOn);
            }
        }

        /// <summary>
        /// 更新指定地图埋点类型的批量选择
        /// </summary>
        /// <param name="isOn"></param>
        /// <param name="mapDataType"></param>
        private void UpdateMapDataBatchOperationByType(bool isOn, MapDataType mapDataType)
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                var uid = mapDataProperty.FindPropertyRelative("UID").intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                if (mapDataConfig.DataType == mapDataType)
                {
                    UpdateMapDataBatchOperationByIndex(i, isOn);
                }
            }
        }

        /// <summary>
        /// 更新指定地图对象索引的批量选项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isOn"></param>
        private void UpdateMapObjectDataBatchOperationByIndex(int index, bool isOn)
        {
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(index);
            if (mapObjectDataProperty == null)
            {
                return;
            }
            var batchOperationSwitchProperty = mapObjectDataProperty.FindPropertyRelative("BatchOperationSwitch");
            batchOperationSwitchProperty.boolValue = isOn;
        }

        /// <summary>
        /// 更新指定地图埋点索引的批量选项
        /// </summary>
        /// <param name="index"></param>
        /// <param name="isOn"></param>
        private void UpdateMapDataBatchOperationByIndex(int index, bool isOn)
        {
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(index);
            if (mapDataProperty == null)
            {
                return;
            }
            var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            batchOperationSwitchProperty.boolValue = isOn;
        }

        /// <summary>
        /// 恢复指定MapObject属性地图对象
        /// </summary>
        /// <param name="mapObjectDataProperty"></param>
        private void RecreateMapObjectGo(SerializedProperty mapObjectDataProperty)
        {
            if (mapObjectDataProperty == null)
            {
                Debug.LogError($"不支持传空地图对象属性，重创地图对象失败！");
                return;
            }
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            var mapObjectUID = uidProperty.intValue;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，重创地图对象失败！");
                return;
            }
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            var gameObject = goProperty.objectReferenceValue as GameObject;
            if (gameObject != null)
            {
                GameObject.DestroyImmediate(gameObject);
            }
            var instanceGo = mTarget != null ? mTarget.CreateGameObjectByUID(mapObjectUID) : null;
            if (instanceGo != null)
            {
                var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                var rotationProperty = mapObjectDataProperty.FindPropertyRelative("Rotation");
                var localScaleProperty = mapObjectDataProperty.FindPropertyRelative("LocalScale");
                instanceGo.transform.position = positionProperty.vector3Value;
                instanceGo.transform.rotation = Quaternion.Euler(rotationProperty.vector3Value);
                instanceGo.transform.localScale = localScaleProperty.vector3Value;
                goProperty.objectReferenceValue = instanceGo;
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
        /// 一键重创地图对象
        /// </summary>
        private void OneKeyRecreateMapObjectGos()
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            for (int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                RecreateMapObjectGo(mapObjectDataProperty);
            }
            serializedObject.ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 移除所有无效UID的配置数据
        /// </summary>
        private void RemoveAllInvalideUIDDatas()
        {
            RemoveAllInvalideUIDMapObjectDatas();
            RemoveAllInvalideUIDMapDatas();
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 移除所有无效UID的地图对象配置数据
        /// </summary>
        private void RemoveAllInvalideUIDMapObjectDatas()
        {
            for (int i = mMapObjectDataListProperty.arraySize - 1; i >= 0; i--)
            {
                var mapObjcetDataListProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapObjcetDataListProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
                if (mapDataConfig == null)
                {
                    mMapObjectDataListProperty.DeleteArrayElementAtIndex(i);
                    Debug.Log($"移除索引:{i}的无效UID:{uid}地图对象数据配置！");
                }
            }
        }

        /// <summary>
        /// 移除所有无效UID的地图埋点配置数据
        /// </summary>
        private void RemoveAllInvalideUIDMapDatas()
        {
            for (int i = mMapDataListProperty.arraySize - 1; i >= 0; i--)
            {
                var mapDataListProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapDataListProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                if (mapDataConfig == null)
                {
                    mMapDataListProperty.DeleteArrayElementAtIndex(i);
                    Debug.Log($"移除索引:{i}的无效UID:{uid}地图埋点数据配置！");
                }
            }
        }

        /// <summary>
        /// 更新所有地图对象的MapObjectDataMono数据到最新
        /// </summary>
        private void UpdateAllMapObjectDataMonos()
        {
            for (int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
                if (goProperty.objectReferenceValue == null)
                {
                    continue;
                }
                var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                var go = goProperty.objectReferenceValue as GameObject;
                MapEditorUtilities.AddOrUpdateMapObjectDataMono(go, uidProperty.intValue);
            }
        }

        /// <summary>
        /// 导出地图数据
        /// </summary>
        private void ExportMapData()
        {
            // 导出地图数据前确保地图对象数据逻辑更新保存一下
            UpdateMapObjectDataLogicDatas();
            // 流程上说场景给客户端使用一定会经历导出流程
            // 在导出时确保MapObjectDataMono和地图对象配置数据一致
            // 从而确保场景资源被使用时挂在数据和配置匹配
            UpdateAllMapObjectDataMonos();
            // 确保所有数据运用到最新
            serializedObject.ApplyModifiedProperties();
            var isPrefabAssetInstance = PrefabUtility.IsPartOfPrefabInstance(mTarget?.gameObject);
            // 确保数据应用到对应Asset上
            if (isPrefabAssetInstance)
            {
                PrefabUtility.ApplyPrefabInstance(mTarget?.gameObject, InteractionMode.AutomatedAction);
            }
            MapExportEditorUtilities.ExportGameMapData(mTarget);
        }

        /// <summary>
        /// 一键烘焙拷贝和导出地图数据
        /// </summary>
        private async Task<bool> OneKeyBakeAndExport()
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return false;
            }
            // 寻路地形烘焙时打开，烘完隐藏，避免运行时被错误看到问题
            var mapTerrian = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject);
            mapTerrian.gameObject.SetActive(true);
            var navMeshSurface = MapEditorUtilities.GetOrCreateNavMeshSurface(mTarget?.gameObject);
            var bakePathTask = MapEditorUtilities.BakePathTask(navMeshSurface);
            var bakePathResult = await bakePathTask;
            mapTerrian.gameObject.SetActive(false);
            if (!bakePathResult)
            {
                Debug.LogError($"地图:{mTarget?.gameObject.name}寻路烘焙失败，一键烘焙导出地图数据失败！");
                return false;
            }
            var copyNavMeshAssetResult = await MapEditorUtilities.CopyNavMeshAsset(mTarget?.gameObject);
            if (!copyNavMeshAssetResult)
            {
                Debug.LogError($"地图:{mTarget?.gameObject.name}拷贝寻路Asset失败，一键烘焙导出地图数据失败！");
                return false;
            }
            ExportMapData();
            AssetDatabase.SaveAssets();
            Debug.Log($"一键烘焙拷贝导出地图数据完成！");
            return true;
        }

        /// <summary>
        /// 响应添加地图对象类型选择变化
        /// </summary>
        private void OnAddMapObjectTypeChange()
        {
            var addMapObjectType = (MapObjectType)mAddMapObjectTypeProperty.intValue;
            var mapObjectDataChoiceValues = mMapObjectDataChoiceValuesMap[addMapObjectType];
            if (mapObjectDataChoiceValues != null && mapObjectDataChoiceValues.Length > 0)
            {
                mAddMapObjectIndexProperty.intValue = mapObjectDataChoiceValues[0];
            }
            else
            {
                mAddMapObjectIndexProperty.intValue = 0;
            }
            serializedObject.ApplyModifiedProperties();
            OnAddMapObjectIndexChange();
        }

        /// <summary>
        /// 响应添加地图对象索引选择变化
        /// </summary>
        private void OnAddMapObjectIndexChange()
        {
            UpdateAddMapObjectDataPreviewAsset();
        }

        /// <summary>
        /// 响应添加地图埋点类型选择变化
        /// </summary>
        private void OnAddMapDataTypeChange()
        {
            var addMapDataType = (MapDataType)mAddMapDataTypeProperty.intValue;
            var mapDataChoiceValues = mMapDataChoiceValuesMap[addMapDataType];
            if (mapDataChoiceValues != null && mapDataChoiceValues.Length > 0)
            {
                mAddMapDataIndexProperty.intValue = mapDataChoiceValues[0];
            }
            else
            {
                mAddMapDataIndexProperty.intValue = 0;
            }
            serializedObject.ApplyModifiedProperties();
            OnAddMapDataIndexChange();
        }

        /// <summary>
        /// 执行指定地图对象索引的UID变化
        /// </summary>
        /// <param name="mapObjectDataIndex"></param>
        /// <param name="newUID"></param>
        private void DoChangeMapObjectDataUID(int mapObjectDataIndex, int newUID)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(mapObjectDataIndex);
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            var oldUID = uidProperty.intValue;
            uidProperty.intValue = newUID;
            var batchOperationSwitchProperty = mapObjectDataProperty.FindPropertyRelative("BatchOperationSwitch");
            if (batchOperationSwitchProperty.boolValue)
            {
                DoMapObjectDataUIDChangeExcept(mapObjectDataIndex, oldUID, newUID);
            }
        }

        /// <summary>
        /// 执行地图对象数据UID批量变化(排除指定地图对象数据索引)
        /// Note:
        /// 只允许批量修改相同埋点对象类型的UID数据
        /// </summary>
        /// <param name="mapObjectDataIndex"></param>
        /// <param name="oldUID"></param>
        /// <param name="newUID"></param>
        private bool DoMapObjectDataUIDChangeExcept(int mapObjectDataIndex, int oldUID, int newUID)
        {
            var originalMapDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(mapObjectDataIndex);
            if (originalMapDataProperty == null)
            {
                Debug.LogError($"找不到地图对象索引:{mapObjectDataIndex}的埋点数据，批量修改UID到{newUID}失败！");
                return false;
            }
            var originalUIDProperty = originalMapDataProperty.FindPropertyRelative("UID");
            var originalUID = originalUIDProperty.intValue;
            var originalMapDataConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(originalUID);
            if (originalMapDataConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{originalUID}的配置数据，批量修改UID到{newUID}失败！");
                return false;
            }
            for (int i = 0, length = mMapObjectDataListProperty.arraySize; i < length; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                if (mapObjectDataProperty == null)
                {
                    continue;
                }
                var batchOperationSwitchProperty = mapObjectDataProperty.FindPropertyRelative("BatchOperationSwitch");
                if (batchOperationSwitchProperty.boolValue)
                {
                    var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                    var originalUID2 = uidProperty.intValue;
                    if (originalUID2 != oldUID)
                    {
                        continue;
                    }
                    uidProperty.intValue = newUID;
                    RecreateMapObjectGo(mapObjectDataProperty);
                }
            }
            return true;
        }

        /// <summary>
        /// 响应添加地图埋点索引选择变化
        /// </summary>
        private void OnAddMapDataIndexChange()
        {

        }

        /// <summary>
        /// 响应关卡数据Asset变化
        /// </summary>
        private void OnLevelMapDataAssetChange()
        {
            mMapDataListProperty.ClearArray();
            ClearMapDataSerializedPropertyCache();
            ClearMapDataTypeIndexsMapCache();
            var levelMapData = mLevelMapDataProperty.objectReferenceValue as LevelMapData;
            var levelMapDataName = levelMapData != null ? levelMapData.name : mTarget.gameObject.name;
            UpdateCustomExportFileName(levelMapDataName);
            if(levelMapData == null)
            {
                return;
            }
            for(int i = 0, length = levelMapData.MapDataList.Count; i < length; i++)
            {
                var mapData = levelMapData.MapDataList[i];
                mMapDataListProperty.InsertArrayElementAtIndex(i);
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                mapDataProperty.managedReferenceValue = mapData;
            }
            UpdateMapDataTypeIndexDatas();
        }

        /// <summary>
        /// 执行指定地图埋点索引的UID变化
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="newUID"></param>
        private void DoChangeMapDataUID(int mapDataIndex, int newUID)
        {
            if (!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var oldUID = uidProperty.intValue;
            uidProperty.intValue = newUID;
            var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            if (batchOperationSwitchProperty.boolValue)
            {
                DoMapDataBatchUIDChangeExcept(mapDataIndex, oldUID, newUID);
            }
        }

        /// <summary>
        /// 执行指定埋点索引，指定属性名和指定值数据更新(含批量更新)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="mapDataIndex"></param>
        /// <param name="propertyName"></param>
        /// <param name="newValue"></param>
        private void DoChangeMapDataProperty<T>(int mapDataIndex, string propertyName, T newValue)
        {
            if(!MapEditorUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var targetMapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
            var targetProperty = targetMapDataProperty.FindPropertyRelative(propertyName);
            if(targetProperty == null)
            {
                Debug.LogError($"地图埋点索引:{mapDataIndex}的目标属性:{propertyName}找不到，更新指定埋点索引和属性名值失败！");
                return;
            }
            var targetUidProperty = targetMapDataProperty.FindPropertyRelative("UID");
            var targetUid = targetUidProperty.intValue;
            var targetMapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(targetUid);
            var targetMapDataType = targetMapDataConfig.DataType;
            var targetBatchOperationSwitchProeprty = targetMapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            UpdatePropertyByValue<T>(targetProperty, newValue);
            if(targetBatchOperationSwitchProeprty.boolValue)
            {
                List<int> mapDataTypeIndexs = GetMapDataTypeIndexs(targetMapDataType);
                if(mapDataTypeIndexs == null)
                {
                    return;
                }
                foreach(var mapDataTypeIndex in mapDataTypeIndexs)
                {
                    if(mapDataIndex == mapDataTypeIndex)
                    {
                        continue;
                    }
                    var mapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataTypeIndex);
                    var batchOperationProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                    if(batchOperationProperty.boolValue)
                    {
                        var property = mapDataProperty.FindPropertyRelative(propertyName);
                        UpdatePropertyByValue<T>(property, newValue);
                    }
                }
            }
        }

        /// <summary>
        /// 更新指定属性到指定值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="property"></param>
        /// <param name="newValue"></param>
        private bool UpdatePropertyByValue<T>(SerializedProperty property, T newValue)
        {
            if(property == null)
            {
                Debug.LogError($"不允许更新空属性的值！");
                return false;
            }
            if (newValue is int newIntValue)
            {
                property.intValue = newIntValue;
                return true;
            }
            else if (newValue is float newFloatValue)
            {
                property.floatValue = newFloatValue;
                return true;
            }
            else if (newValue is double newDoubleValue)
            {
                property.doubleValue = newDoubleValue;
                return true;
            }
            else if (newValue is string newStringValue)
            {
                property.stringValue = newStringValue;
                return true;
            }
            else if (newValue is bool newBoolValue)
            {
                property.boolValue = newBoolValue;
                return true;
            }
            else
            {
                Debug.LogError($"不支持的属性类型:{newValue.GetType().Name}，更新属性值失败！");
                return false;
            }
        }

        /// <summary>
        /// 执行地图埋点数据UID批量变化(排除指定地图数据索引)
        /// Note:
        /// 只允许批量修改相同埋点类型的UID数据
        /// </summary>
        /// <param name="mapDataIndex"></param>
        private bool DoMapDataBatchUIDChangeExcept(int mapDataIndex, int oldUID, int newUID)
        {
            var originalMapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
            if (originalMapDataProperty == null)
            {
                Debug.LogError($"找不到地图埋点索引:{mapDataIndex}的埋点数据，批量修改UID到{newUID}失败！");
                return false;
            }
            var originalUIDProperty = originalMapDataProperty.FindPropertyRelative("UID");
            var originalUID = originalUIDProperty.intValue;
            var originalMapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(originalUID);
            if (originalMapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{originalUID}的配置数据，批量修改UID到{newUID}失败！");
                return false;
            }
            List<int> mapDataTypeIndexs = GetMapDataTypeIndexs(originalMapDataConfig.DataType);
            if (mapDataTypeIndexs == null)
            {
                return true;
            }
            for (int i = 0, length = mapDataTypeIndexs.Count; i < length; i++)
            {
                var realMapDataIndex = mapDataTypeIndexs[i];
                if (realMapDataIndex == mapDataIndex)
                {
                    continue;
                }
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(realMapDataIndex);
                if (mapDataProperty == null)
                {
                    continue;
                }
                var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                if (batchOperationSwitchProperty.boolValue)
                {
                    var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                    var originalUID2 = uidProperty.intValue;
                    if (originalUID2 != oldUID)
                    {
                        continue;
                    }
                    uidProperty.intValue = newUID;
                }
            }
            return true;
        }

        /// <summary>
        /// 响应指定索引的地图数据埋点位置移动
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="positionOffset"></param>
        private void OnMapDataPositionMove(int mapDataIndex, Vector3 positionOffset)
        {
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            positionProperty.vector3Value = positionProperty.vector3Value + positionOffset;
            var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            if (batchOperationSwitchProperty.boolValue)
            {
                DoMapDataBatchMoveExcept(mapDataIndex, positionOffset);
            }
        }

        /// <summary>
        /// 执行指定索引外的批量数据埋点位置移动
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="positionOffset"></param>
        private void DoMapDataBatchMoveExcept(int mapDataIndex, Vector3 positionOffset)
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                if (i == mapDataIndex)
                {
                    continue;
                }
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                var batchOperationSwitch = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                if (batchOperationSwitch.boolValue)
                {
                    var positionProperty = mapDataProperty.FindPropertyRelative("Position");
                    positionProperty.vector3Value = positionProperty.vector3Value + positionOffset;
                }
            }
        }

        /// <summary>
        /// 获取制定数据索引的折叠数据索引
        /// Note:
        /// 默认折叠数据分组和折叠数据索引一致
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private int GetMapFoldIndex(int index)
        {
            return index / MapEditorConst.MapFoldNumLimit;
        }

        /// <summary>
        /// 获取指定地图折叠类型的展开数据列表属性
        /// </summary>
        /// <param name="targetMapFoldType"></param>
        /// <returns></returns>
        private SerializedProperty GetMapUnfoldDataListProperty(MapFoldType targetMapFoldType)
        {
            SerializedProperty groupUnfoldListProperty;
            if(!mMapFoldTypeDataPropertyMap.TryGetValue(targetMapFoldType, out groupUnfoldListProperty))
            {
                SerializedProperty mapFoldTypeDataProperty = null;
                var groupUnfoldDataListNum = mAllGroupUnfoldDataListProperty.arraySize;
                for(int index = 0; index < groupUnfoldDataListNum; index++)
                {
                    var singleGroupUnfoldDataListProperty = mAllGroupUnfoldDataListProperty.GetArrayElementAtIndex(index);
                    var mapFoldTypeProperty = singleGroupUnfoldDataListProperty.FindPropertyRelative("MapFoldType");
                    var mapFoldType = (MapFoldType)mapFoldTypeProperty.intValue;
                    if(mapFoldType == targetMapFoldType)
                    {
                        mapFoldTypeDataProperty = singleGroupUnfoldDataListProperty;
                        groupUnfoldListProperty = mapFoldTypeDataProperty.FindPropertyRelative("GroupUnfoldList");
                        break;
                    }
                }
                if(mapFoldTypeDataProperty == null)
                {
                    mAllGroupUnfoldDataListProperty.InsertArrayElementAtIndex(groupUnfoldDataListNum);
                    mapFoldTypeDataProperty = mAllGroupUnfoldDataListProperty.GetArrayElementAtIndex(groupUnfoldDataListNum);
                    mapFoldTypeDataProperty.managedReferenceValue = new MapFoldTypeData(targetMapFoldType);
                    var mapFoldTypeProperty = mapFoldTypeDataProperty.FindPropertyRelative("MapFoldType");
                    mapFoldTypeProperty.intValue = (int)targetMapFoldType;
                    groupUnfoldListProperty = mapFoldTypeDataProperty.FindPropertyRelative("GroupUnfoldList");
                }
                mMapFoldTypeDataPropertyMap.Add(targetMapFoldType, groupUnfoldListProperty);
            }
            return groupUnfoldListProperty;
        }

        /// <summary>
        /// 获取指定地图折叠类型和组id是否展开
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool GetMapGroupUnfold(MapFoldType mapFoldType, int index)
        {
            if (!ExistMapUnfold(mapFoldType, index))
            {
                AddMapGroupUnfoldData(mapFoldType, index);
            }
            var groupUnfoldDataListProperty = GetMapUnfoldDataListProperty(mapFoldType);
            var groupUnfoldDataProperty = groupUnfoldDataListProperty.GetArrayElementAtIndex(index);
            return groupUnfoldDataProperty.boolValue;
        }

        /// <summary>
        /// 获取指定折叠类型和组id的折叠数据
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool ExistMapUnfold(MapFoldType mapFoldType, int index)
        {
            var groupUnfoldDataListProperty = GetMapUnfoldDataListProperty(mapFoldType);
            var groupUnfoldDataListSize = groupUnfoldDataListProperty.arraySize;
            if (groupUnfoldDataListSize == 0)
            {
                return false;
            }
            return index < groupUnfoldDataListSize;
        }

        /// <summary>
        /// 添加指定地图折叠类型和组id的展开数据
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="index"></param>
        /// <param name="isUnfold"></param>
        /// <returns></returns>
        private bool AddMapGroupUnfoldData(MapFoldType mapFoldType, int index, bool isUnfold = false)
        {
            if (ExistMapUnfold(mapFoldType, index))
            {
                Debug.LogError($"重复添加折叠类型:{mapFoldType}，索引:{index}地图对象数据的展开数据，添加失败！");
                return false;
            }
            var unfoldDataListProperty = GetMapUnfoldDataListProperty(mapFoldType);
            var groupUnfoldDataNum = unfoldDataListProperty.arraySize;
            unfoldDataListProperty.InsertArrayElementAtIndex(groupUnfoldDataNum);
            var unfoldDataProperty = GetMapGroupUnfoldProperty(mapFoldType, index);
            unfoldDataProperty.boolValue = isUnfold;
            return true;
        }

        /// <summary>
        /// 更新指定地图折叠类型和组id的展开数据
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="index"></param>
        /// <param name="isUnfold"></param>
        /// <returns></returns>
        private bool UpdateMapGroupUnfoldData(MapFoldType mapFoldType, int index, bool isUnfold = false)
        {
            if (!ExistMapUnfold(mapFoldType, index))
            {
                Debug.LogError($"折叠类型:{mapFoldType}，索引:{index}地图对象数据的展开数据不存在，更新失败！");
                return false;
            }
            var unfoldDataProperty = GetMapGroupUnfoldProperty(mapFoldType, index);
            unfoldDataProperty.boolValue = isUnfold;
            return true;
        }

        /// <summary>
        /// 获取指定游戏地图折叠类型指定索引的折叠属性
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private SerializedProperty GetMapGroupUnfoldProperty(MapFoldType mapFoldType, int index)
        {
            var unfoldDataListProperty = GetMapUnfoldDataListProperty(mapFoldType);
            return unfoldDataListProperty.GetArrayElementAtIndex(index);
        }

        /// <summary>
        /// 执行指定折叠类型的一键展开
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <param name="isUnfold"></param>
        private void DoOneKeyUnfold(MapFoldType mapFoldType, bool isUnfold)
        {
            var mapDataUnfoldDataListProperty = GetMapUnfoldDataListProperty(mapFoldType);
            for (int groupId = 0, length = mapDataUnfoldDataListProperty.arraySize; groupId < length; groupId++)
            {
                UpdateMapGroupUnfoldData(mapFoldType, groupId, isUnfold);
            }
        }

        /// <summary>
        /// Inspector自定义显示
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();

            CheckMapDataLengthChange();

            CheckInspectorKeyCodeEvent();

            // 确保对SerializedObjec和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapLineGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapObjectSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapDataSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapObjectAddedAutoFocusProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapWidthProperty);
            EditorGUILayout.PropertyField(mMapHeightProperty);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateMapSizeDrawDatas();
                UpdateMapGOPosition();
                UpdateTerrianSizeAndPos();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapStartPosProperty);
            if (EditorGUI.EndChangeCheck())
            {
                UpdateMapSizeDrawDatas();
                UpdateMapGOPosition();
            }

            DrawGameMapButtonArea();
            DrawMapOperationInspectorArea();

            EditorGUILayout.EndVertical();

            CheckShortcuts();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制游戏地图按钮区域
        /// </summary>
        private void DrawGameMapButtonArea()
        {
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("一键重创地图对象", GUILayout.ExpandWidth(true)))
                {
                    OneKeyRecreateMapObjectGos();
                }
                if (GUILayout.Button("清除无效UID配置", GUILayout.ExpandWidth(true)))
                {
                    RemoveAllInvalideUIDDatas();
                }
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("导出类型:", GUILayout.Width(60f));
                mExportTypeProperty.intValue = (int)(ExportType)EditorGUILayout.EnumPopup((ExportType)mExportTypeProperty.intValue, GUILayout.Width(80f));
                EditorGUILayout.LabelField("自定义导出文件名:", GUILayout.Width(100f));
                mCustomExportFileNameProperty.stringValue = EditorGUILayout.TextField(mCustomExportFileNameProperty.stringValue, GUILayout.Width(115f));
                if (GUILayout.Button("导出地图数据", GUILayout.ExpandWidth(true)))
                {
                    ExportMapData();
                }
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("关卡数据Asset:", GUILayout.Width(90f));
                EditorGUI.BeginChangeCheck();
                mLevelMapDataProperty.objectReferenceValue = EditorGUILayout.ObjectField(mLevelMapDataProperty.objectReferenceValue, MapConst.LevelMapDataType, GUILayout.Width(270f));
                if (EditorGUI.EndChangeCheck())
                {
                    OnLevelMapDataAssetChange();
                }
                if (GUILayout.Button("保存关卡数据", GUILayout.ExpandWidth(true)))
                {
                    DoSaveToLevelMapDataAsset();
                }
                EditorGUILayout.EndHorizontal();
            }
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("一键烘焙拷贝导出", GUILayout.ExpandWidth(true)))
                {
                    OneKeyBakeAndExport();
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图操作Inspector区域
        /// </summary>
        private void DrawMapOperationInspectorArea()
        {
            EditorGUILayout.BeginVertical("box");
            DrawCommonOperationInspectorArea();
            EditorGUILayout.BeginHorizontal();
            mPanelToolBarSelectIndex = GUILayout.Toolbar(mPanelToolBarSelectIndex, mPanelToolBarStrings, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            mSelectedTabType = (MapTabType)mPanelToolBarSelectIndex;
            EditorGUILayout.EndHorizontal();
            if (mSelectedTabType == MapTabType.MapBuild)
            {
                DrawMapObjectInspectorArea();
            }
            else if (mSelectedTabType == MapTabType.DataEditor)
            {
                DrawMapDataInspectorArea();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制通用操作Inspector区域
        /// </summary>
        private void DrawCommonOperationInspectorArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("一键勾选批量(快捷键:Shift+Alt+S)", GUILayout.ExpandWidth(true)))
            {
                OneKeySwitchBatchOperation(true);
            }
            if (GUILayout.Button("一键清除批量勾选(快捷键:Shift+Alt+C)", GUILayout.ExpandWidth(true)))
            {
                OneKeySwitchBatchOperation(false);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("开始索引:", GUILayout.Width(60f));
            mBatchTickRangeStartIndexProperty.intValue = EditorGUILayout.IntField(mBatchTickRangeStartIndexProperty.intValue, GUILayout.Width(100f));
            EditorGUILayout.LabelField("结束索引:", GUILayout.Width(60f));
            mBatchTickRangeEndIndexProperty.intValue = EditorGUILayout.IntField(mBatchTickRangeEndIndexProperty.intValue, GUILayout.Width(100f));
            if (GUILayout.Button("范围勾选", GUILayout.ExpandWidth(true)))
            {
                OneKeySwitchRangeOperation(true);
            }
            if (GUILayout.Button("范围清除勾选", GUILayout.ExpandWidth(true)))
            {
                OneKeySwitchRangeOperation(false);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图对象数据Inspector区域
        /// </summary>
        private void DrawMapObjectInspectorArea()
        {
            EditorGUILayout.BeginVertical("box");
            DrawMapObjectDataOperationArea();
            DrawMapObjectDataListArea();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图对象数据操作区域
        /// </summary>
        private void DrawMapObjectDataOperationArea()
        {
            if (mMapObjectDataChoiceOptionsMap != null && mMapObjectDataChoiceOptionsMap.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("地图对象类型:", GUILayout.Width(90f));
                EditorGUI.BeginChangeCheck();
                mAddMapObjectTypeProperty.intValue = (int)(MapObjectType)EditorGUILayout.EnumPopup((MapObjectType)mAddMapObjectTypeProperty.intValue, GUILayout.Width(150f));
                if (EditorGUI.EndChangeCheck())
                {
                    OnAddMapObjectTypeChange();
                }
                EditorGUILayout.LabelField("地图对象选择:", GUILayout.Width(90f));
                EditorGUI.BeginChangeCheck();
                var addMapObjectType = (MapObjectType)mAddMapObjectTypeProperty.intValue;
                var mapObjectDataChoiceOptions = GetMapObjectDataChoiceOptionsByType(addMapObjectType);
                var mapObjectDataChoiceValues = GetMapObjectDataChoiceValuesByType(addMapObjectType);
                mAddMapObjectIndexProperty.intValue = EditorGUILayout.IntPopup(mAddMapObjectIndexProperty.intValue, mapObjectDataChoiceOptions, mapObjectDataChoiceValues, GUILayout.Width(150f));
                if (EditorGUI.EndChangeCheck())
                {
                    OnAddMapObjectIndexChange();
                }
                if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
                {
                    var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
                    DoAddMapObjectData(addMapObjectValue);
                }
                GUILayout.Box(mAddMapObjectPreviewAsset, MapStyles.CenterLabelStyle, GUILayout.Width(50f), GUILayout.Height(50f));
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("没有有效地图对象配置，不支持地图对象选择！", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制地图对象数据列表区域
        /// </summary>
        private void DrawMapObjectDataListArea()
        {
            mMapObjectDataUnfoldDataProperty.boolValue = EditorGUILayout.Foldout(mMapObjectDataUnfoldDataProperty.boolValue, "地图对象数据列表");
            if (mMapObjectDataListProperty.arraySize == 0)
            {
                return;
            }
            if (mMapObjectDataUnfoldDataProperty.boolValue)
            {
                DrawMapObjectOneKeyFoldArea();
                DrawMapObjectTitleArea();
                for (int i = 0, length = mMapObjectDataListProperty.arraySize; i < length; i++)
                {
                    var mapObjectDataGroupIndex = MapEditorUtilities.GetMapFoldIndex(i);
                    var isUnfold = GetMapGroupUnfold(MapFoldType.MapObjectDataFold, mapObjectDataGroupIndex);
                    var groupStartIndex = mapObjectDataGroupIndex * MapEditorConst.MapFoldNumLimit;
                    var groupEndIndex = (mapObjectDataGroupIndex + 1) * MapEditorConst.MapFoldNumLimit;
                    bool newIsUnfold = isUnfold;
                    if (i == groupStartIndex)
                    {
                        newIsUnfold = EditorGUILayout.Foldout(isUnfold, $"{mapObjectDataGroupIndex}({groupStartIndex}-{groupEndIndex})");
                        if (isUnfold != newIsUnfold)
                        {
                            UpdateMapGroupUnfoldData(MapFoldType.MapObjectDataFold, mapObjectDataGroupIndex, newIsUnfold);
                        }
                    }
                    if (newIsUnfold && i >= groupStartIndex && i < groupEndIndex)
                    {
                        DrawOneMapObjectPropertyByIndex(i);
                    }
                }
            }
        }

        /// <summary>
        /// 绘制地图对象一键折叠Inspector区域
        /// </summary>
        private void DrawMapObjectOneKeyFoldArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            var unfoldTitle = MapEditorUtilities.GetMapOneKeyUnfoldTitle(MapFoldType.MapObjectDataFold);
            if (GUILayout.Button($"{unfoldTitle}", GUILayout.ExpandWidth(true)))
            {
                DoOneKeyUnfold(MapFoldType.MapObjectDataFold, true);
            }
            var foldTitle = MapEditorUtilities.GetMapOneKeyFoldTitle(MapFoldType.MapObjectDataFold);
            if (GUILayout.Button($"{foldTitle}", GUILayout.ExpandWidth(true)))
            {
                DoOneKeyUnfold(MapFoldType.MapObjectDataFold, false);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图对象数据标题区域
        /// </summary>
        private void DrawMapObjectTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("批量", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectBatchUIWidth));
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectIndexUIWidth));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectUIDUIWidth));
            EditorGUILayout.LabelField("对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectTypeUIWidth));
            EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectConfIdUIWidth));
            EditorGUILayout.LabelField("GUI关闭", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectGUISwitchOffUIWidth));
            EditorGUILayout.LabelField("实体对象", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectInstanceUIWidth));
            EditorGUILayout.LabelField("位置", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectPositionUIWidth));
            //EditorGUILayout.LabelField("描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectDesUIWidth));
            EditorGUILayout.LabelField("上移", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectMoveUpUIWidth));
            EditorGUILayout.LabelField("下移", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectMoveDownUIWidth));
            EditorGUILayout.LabelField("添加", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectRemoveUIWidth));
            EditorGUILayout.LabelField("删除", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectAddUIWidth));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引的单个MapObjectData属性
        /// </summary>
        /// <param name="mapObjectDataIndex"></param>
        private void DrawOneMapObjectPropertyByIndex(int mapObjectDataIndex)
        {
            EditorGUILayout.BeginHorizontal();
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(mapObjectDataIndex);
            var batchOperationSwitchProperty = mapObjectDataProperty.FindPropertyRelative("BatchOperationSwitch");
            var batchSpace = 10f;
            EditorGUILayout.Space(batchSpace, false);
            batchOperationSwitchProperty.boolValue = EditorGUILayout.Toggle(batchOperationSwitchProperty.boolValue, GUILayout.Width(MapEditorConst.InspectorObjectBatchUIWidth - batchSpace));
            var currentEvent = Event.current;
            if(IsInspectorKeyCodeDown(KeyCode.LeftShift) || IsInspectorKeyCodeDown(KeyCode.RightShift))
            {
                if(GUILayoutUtility.GetLastRect().Contains(currentEvent.mousePosition))
                {
                    //Debug.Log($"按住Shift并处于埋点对象数据索引:{mapObjectDataIndex}的批量勾选区域！")；
                    if(!batchOperationSwitchProperty.boolValue)
                    {
                        batchOperationSwitchProperty.boolValue = true;
                    }
                }
            }
            else if (IsInspectorKeyCodeDown(KeyCode.LeftAlt) || IsInspectorKeyCodeDown(KeyCode.RightAlt))
            {
                if (GUILayoutUtility.GetLastRect().Contains(currentEvent.mousePosition))
                {
                    //Debug.Log($"按住Alt并处于埋点对象数据索引:{mapObjectDataIndex}的批量勾选区域！")；
                    if (batchOperationSwitchProperty.boolValue)
                    {
                        batchOperationSwitchProperty.boolValue = false;
                    }
                }
            }
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            EditorGUILayout.LabelField($"{mapObjectDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectIndexUIWidth));
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            EditorGUI.BeginChangeCheck();
            var mapObjectType = mapObjectConfig != null ? mapObjectConfig.ObjectType : MapObjectType.Scene;
            var mapObjectDataChoiceOptions = GetMapObjectDataChoiceOptionsByType(mapObjectType);
            var mapObjectDataChoiceValues = GetMapObjectDataChoiceValuesByType(mapObjectType);
            uid = EditorGUILayout.IntPopup(uid, mapObjectDataChoiceOptions, mapObjectDataChoiceValues, GUILayout.Width(MapEditorConst.InspectorObjectUIDUIWidth));
            if (EditorGUI.EndChangeCheck())
            {
                DoChangeMapObjectDataUID(mapObjectDataIndex, uid);
            }
            if (mapObjectConfig != null)
            {
                EditorGUILayout.LabelField(mapObjectConfig.ObjectType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectTypeUIWidth));
                EditorGUILayout.IntField(mapObjectConfig.ConfId, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectConfIdUIWidth));
            }
            else
            {
                EditorGUILayout.LabelField("找不到对象类型数据", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectTypeUIWidth));
                EditorGUILayout.LabelField("找不到关联Id数据", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectConfIdUIWidth));
            }
            var guiSwitchOffProperty = mapObjectDataProperty.FindPropertyRelative("GUISwitchOff");
            var space = 20f;
            EditorGUILayout.Space(space, false);
            guiSwitchOffProperty.boolValue = EditorGUILayout.Toggle(guiSwitchOffProperty.boolValue, GUILayout.Width(MapEditorConst.InspectorObjectGUISwitchOffUIWidth - space));
            var gameObject = goProperty.objectReferenceValue as GameObject;
            EditorGUILayout.ObjectField(goProperty.objectReferenceValue, MapConst.GameObjectType, true, GUILayout.Width(MapEditorConst.InspectorObjectInstanceUIWidth));
            var newVector3Value = gameObject != null ? gameObject.transform.position : Vector3.zero;
            EditorGUI.BeginChangeCheck();
            var posWidth = (MapEditorConst.InspectorObjectPositionUIWidth - 43f) / 3;
            EditorGUILayout.LabelField("X", GUILayout.Width(10f));
            newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(posWidth));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(posWidth));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(posWidth));
            if (gameObject != null && EditorGUI.EndChangeCheck())
            {
                gameObject.transform.position = newVector3Value;
            }
            //var des = mapObjectConfig != null ? mapObjectConfig.Des : "";
            //EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorObjectDesUIWidth));
            if (GUILayout.Button("↑", GUILayout.Width(MapEditorConst.InspectorObjectMoveUpUIWidth)))
            {
                DoMovePropertyDataUpByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("↓", GUILayout.Width(MapEditorConst.InspectorObjectMoveDownUIWidth)))
            {
                DoMovePropertyDataDownByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("+", GUILayout.Width(MapEditorConst.InspectorObjectAddUIWidth)))
            {
                var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
                DoAddMapObjectData(addMapObjectValue, mapObjectDataIndex);
            }
            if (GUILayout.Button("-", GUILayout.Width(MapEditorConst.InspectorObjectRemoveUIWidth)))
            {
                DoRemoveMapObjectDataByIndex(mapObjectDataIndex);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图埋点数据Inspector区域
        /// </summary>
        private void DrawMapDataInspectorArea()
        {
            EditorGUILayout.BeginVertical("box");
            DrawMapDataOperationArea();
            DrawMapDataArea();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图埋点数据操作区域
        /// </summary>
        private void DrawMapDataOperationArea()
        {
            if (mMapDataChoiceOptionsMap != null && mMapDataChoiceOptionsMap.Count > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("地图埋点类型:", GUILayout.Width(90f));
                EditorGUI.BeginChangeCheck();
                mAddMapDataTypeProperty.intValue = (int)(MapDataType)EditorGUILayout.EnumPopup((MapDataType)mAddMapDataTypeProperty.intValue, GUILayout.Width(150f));
                if (EditorGUI.EndChangeCheck())
                {
                    OnAddMapDataTypeChange();
                }
                EditorGUILayout.LabelField("地图埋点选择:", GUILayout.Width(90f));
                EditorGUI.BeginChangeCheck();
                var addMapDataType = (MapDataType)mAddMapDataTypeProperty.intValue;
                var mapDataChoiceOptions = mMapDataChoiceOptionsMap[addMapDataType];
                var mapDataChoiceValues = mMapDataChoiceValuesMap[addMapDataType];
                mAddMapDataIndexProperty.intValue = EditorGUILayout.IntPopup(mAddMapDataIndexProperty.intValue, mapDataChoiceOptions, mapDataChoiceValues, GUILayout.Width(150f));
                if (EditorGUI.EndChangeCheck())
                {
                    OnAddMapDataIndexChange();
                }
                if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
                {
                    var addMapDataValue = mAddMapDataIndexProperty.intValue;
                    DoAddMapData(addMapDataValue);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("没有有效地图埋点配置，不支持地图埋点选择！", GUILayout.ExpandWidth(true));
                EditorGUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// 绘制地图埋点数据区域
        /// </summary>
        private void DrawMapDataArea()
        {
            mMapDataUnfoldDataProperty.boolValue = EditorGUILayout.Foldout(mMapDataUnfoldDataProperty.boolValue, "地图埋点数据列表");
            if (mMapDataUnfoldDataProperty.boolValue)
            {
                foreach(var mapDataType in MapEditorConst.AllMapDataTypes)
                {
                    DrawMapDataAreaByType(mapDataType);
                }
            }
        }

        /// <summary>
        /// 绘制指定地图埋点类型数据区域
        /// </summary>
        /// <param name="mapDataType"></param>
        private void DrawMapDataAreaByType(MapDataType mapDataType)
        {
            var mapDataIndexs = GetMapDataTypeIndexs(mapDataType);
            if (mapDataIndexs == null || mapDataIndexs.Count == 0)
            {
                return;
            }
            if (!MapEditorUtilities.IsShowMapDataFoldType(mapDataType))
            {
                return;
            }
            var mapFoldType = MapEditorUtilities.GetMapDataFoldType(mapDataType);
            DrawMapDataOneKeyArea(mapDataType, mapFoldType);
            MapEditorUtilities.DrawMapDataTitleAreaByType(mapDataType);
            DrawMapDataDetailByType(mapDataType);
        }

        /// <summary>
        /// 绘制指定地图埋点类型的一键区域
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <param name="mapFoldType"></param>
        private void DrawMapDataOneKeyArea(MapDataType mapDataType, MapFoldType mapFoldType)
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("一键勾选批量", GUILayout.ExpandWidth(true)))
            {
                UpdateMapDataBatchOperationByType(true, mapDataType);
            }
            if (GUILayout.Button("一键清除批量勾选", GUILayout.ExpandWidth(true)))
            {
                UpdateMapDataBatchOperationByType(false, mapDataType);
            }
            var unfoldTitle = MapEditorUtilities.GetMapOneKeyUnfoldTitle(mapFoldType);
            if (GUILayout.Button($"{unfoldTitle}", GUILayout.ExpandWidth(true)))
            {
                DoOneKeyUnfold(mapFoldType, true);
            }
            var foldTitle = MapEditorUtilities.GetMapOneKeyFoldTitle(mapFoldType);
            if (GUILayout.Button($"{foldTitle}", GUILayout.ExpandWidth(true)))
            {
                DoOneKeyUnfold(mapFoldType, true);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定地图埋点类型的数据详情区域
        /// </summary>
        /// <param name="mapDataType"></param>
        private void DrawMapDataDetailByType(MapDataType mapDataType)
        {
            var mapDataIndexs = GetMapDataTypeIndexs(mapDataType);
            if (mapDataIndexs == null)
            {
                return;
            }
            var mapFoldType = MapEditorUtilities.GetMapDataFoldType(mapDataType);
            var mapDataTypeIndex = 0;
            for (int i = 0, length = mapDataIndexs.Count; i < length; i++)
            {
                var mapDataIndex = mapDataIndexs[i];
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
                if (mapDataProperty == null)
                {
                    continue;
                }
                var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                if (mapDataConfig == null || mapDataConfig.DataType != mapDataType)
                {
                    continue;
                }
                var mapDataGroupIndex = MapEditorUtilities.GetMapFoldIndex(mapDataTypeIndex);
                var isUnfold = GetMapGroupUnfold(mapFoldType, mapDataGroupIndex);
                var groupStartIndex = mapDataGroupIndex * MapEditorConst.MapFoldNumLimit;
                var groupEndIndex = (mapDataGroupIndex + 1) * MapEditorConst.MapFoldNumLimit;
                bool newIsUnfold = isUnfold;
                if (mapDataTypeIndex == groupStartIndex)
                {
                    newIsUnfold = EditorGUILayout.Foldout(isUnfold, $"{mapDataGroupIndex}({groupStartIndex}-{groupEndIndex})");
                    if (isUnfold != newIsUnfold)
                    {
                        UpdateMapGroupUnfoldData(mapFoldType, mapDataGroupIndex, newIsUnfold);
                    }
                }
                if (newIsUnfold && mapDataTypeIndex >= groupStartIndex && mapDataTypeIndex < groupEndIndex)
                {
                    DrawOneMapDataPropertyByIndex(mapDataConfig.DataType, mapDataIndex);
                }
                mapDataTypeIndex++;
            }
        }

        /// <summary>
        /// 绘制单个指定地图埋点类型和索引的数据
        /// </summary>
        /// <param name="mapDataIndex"></param>
        private void DrawOneMapDataPropertyByIndex(MapDataType mapDataType, int mapDataIndex)
        {
            EditorGUILayout.BeginHorizontal();
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(mapDataIndex);
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Batch))
            {
                var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                var space = 10f;
                EditorGUILayout.Space(space, false);
                batchOperationSwitchProperty.boolValue = EditorGUILayout.Toggle(batchOperationSwitchProperty.boolValue, GUILayout.Width(MapEditorConst.InspectorDataBatchUIWidth - space));
                var currentEvent = Event.current;
                if (IsInspectorKeyCodeDown(KeyCode.LeftShift) || IsInspectorKeyCodeDown(KeyCode.RightShift))
                {
                    if (GUILayoutUtility.GetLastRect().Contains(currentEvent.mousePosition))
                    {
                        //Debug.Log($"按住Shift并处于埋点数据索引:{mapObjectDataIndex}的批量勾选区域！")；
                        if (!batchOperationSwitchProperty.boolValue)
                        {
                            batchOperationSwitchProperty.boolValue = true;
                        }
                    }
                }
                else if (IsInspectorKeyCodeDown(KeyCode.LeftAlt) || IsInspectorKeyCodeDown(KeyCode.RightAlt))
                {
                    if (GUILayoutUtility.GetLastRect().Contains(currentEvent.mousePosition))
                    {
                        //Debug.Log($"按住Alt并处于埋点数据索引:{mapObjectDataIndex}的批量勾选区域！")；
                        if (batchOperationSwitchProperty.boolValue)
                        {
                            batchOperationSwitchProperty.boolValue = false;
                        }
                    }
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Index))
            {
                EditorGUILayout.LabelField($"{mapDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataIndexUIWidth));
            }
            MapDataConfig mapDataConfig = null;
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.UID))
            {
                EditorGUI.BeginChangeCheck();
                var realMapDataType = mapDataConfig != null ? mapDataConfig.DataType : MapDataType.PlayerSpawn;
                var mapDataChoiceOptions = GetMapDataChoiceOptionsByType(realMapDataType);
                var mapDataChoiceValues = GetMapDataChoiceValuesByType(realMapDataType);
                uid = EditorGUILayout.IntPopup(uid, mapDataChoiceOptions, mapDataChoiceValues, GUILayout.Width(MapEditorConst.InspectorDataUIDUIWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    DoChangeMapDataUID(mapDataIndex, uid);
                }
            }
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MapDataType))
            //{
            //    if (mapDataConfig != null)
            //    {
            //        EditorGUILayout.LabelField(mapDataConfig.DataType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataTypeUIWidth));
            //    }
            //    else
            //    {
            //        EditorGUILayout.LabelField("找不到对象类型数据", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataTypeUIWidth));
            //    }
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.ConfId))
            {
                if (mapDataConfig != null)
                {
                    EditorGUILayout.IntField(mapDataConfig.ConfId, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataConfIdUIWidth));
                }
                else
                {
                    EditorGUILayout.LabelField("找不到关联Id数据", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataConfIdUIWidth));
                }
            }
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Des))
            //{
            //    var des = mapDataConfig != null ? mapDataConfig.Des : "";
            //    EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataDesUIWidth));
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.GUISwitchOff))
            {
                var propertyName = "GUISwitchOff";
                var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative(propertyName);
                if (guiSwitchOffProperty != null)
                {
                    var space = 20f;
                    EditorGUILayout.Space(space, false);
                    EditorGUI.BeginChangeCheck();
                    var newGuiSwitchOffPropertyValue = EditorGUILayout.Toggle(guiSwitchOffProperty.boolValue, GUILayout.Width(MapEditorConst.InspectorDataGUISwitchOffUIWidth - space));
                    if(EditorGUI.EndChangeCheck())
                    {
                        DoChangeMapDataProperty(mapDataIndex, propertyName, newGuiSwitchOffPropertyValue);
                    }
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Position))
            {
                var newVector3Value = positionProperty.vector3Value;
                EditorGUI.BeginChangeCheck();
                var singlePosWidth = (MapEditorConst.InspectorDataPositionUIWidth - 43f) / 3;
                EditorGUILayout.LabelField("X", GUILayout.Width(10f));
                newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(singlePosWidth));
                EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
                newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(singlePosWidth));
                EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
                newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(singlePosWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    var positionOffset = newVector3Value - positionProperty.vector3Value;
                    OnMapDataPositionMove(mapDataIndex, positionOffset);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Rotation))
            {
                var rotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var newRotationVector3Value = rotationProperty.vector3Value;
                EditorGUI.BeginChangeCheck();
                var singleRotWidth = (MapEditorConst.InspectorDataRotationUIWidth - 43f) / 3;
                EditorGUILayout.LabelField("X", GUILayout.Width(10f));
                newRotationVector3Value.x = EditorGUILayout.FloatField(newRotationVector3Value.x, GUILayout.Width(singleRotWidth));
                EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
                newRotationVector3Value.y = EditorGUILayout.FloatField(newRotationVector3Value.y, GUILayout.Width(singleRotWidth));
                EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
                newRotationVector3Value.z = EditorGUILayout.FloatField(newRotationVector3Value.z, GUILayout.Width(singleRotWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    rotationProperty.vector3Value = newRotationVector3Value;
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.MonsterCreateRadius))
            {
                var propertyName = "MonsterCreateRadius";
                var monsterCreateRadiusProperty = mapDataProperty.FindPropertyRelative(propertyName);
                if (monsterCreateRadiusProperty != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var newMonsterCreateRadius = EditorGUILayout.FloatField(monsterCreateRadiusProperty.floatValue, GUILayout.Width(MapEditorConst.InspectorDataMonsterCreateRadiusUIWidth));
                    if (EditorGUI.EndChangeCheck())
                    {
                        DoChangeMapDataProperty(mapDataIndex, propertyName, newMonsterCreateRadius);
                    }
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.MonsterActiveRadius))
            {
                var propertyName = "MonsterActiveRadius";
                var monsterActiveRadiusProperty = mapDataProperty.FindPropertyRelative(propertyName);
                if (monsterActiveRadiusProperty != null)
                {
                    EditorGUI.BeginChangeCheck();
                    var newMonsterActiveRadius = EditorGUILayout.FloatField(monsterActiveRadiusProperty.floatValue, GUILayout.Width(MapEditorConst.InspectorDataMonsterActiveRediusUIWidth));
                    if (EditorGUI.EndChangeCheck())
                    {
                        DoChangeMapDataProperty(mapDataIndex, propertyName, newMonsterActiveRadius);
                    }
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.MoveUp))
            {
                if (GUILayout.Button("↑", GUILayout.Width(MapEditorConst.InspectorDataMoveUpUIWidth)))
                {
                    DoMovePropertyDataUpByIndex(mMapDataListProperty, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.MoveDown))
            {
                if (GUILayout.Button("↓", GUILayout.Width(MapEditorConst.InspectorDataMoveDownUIWidth)))
                {
                    DoMovePropertyDataDownByIndex(mMapDataListProperty, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Add))
            {
                if (GUILayout.Button("+", GUILayout.Width(MapEditorConst.InspectorDataRemoveUIWidth)))
                {
                    var addMapDataValue = mAddMapDataIndexProperty.intValue;
                    DoAddMapData(addMapDataValue, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapUIType.Remove))
            {
                if (GUILayout.Button("-", GUILayout.Width(MapEditorConst.InspectorDataAddUIWidth)))
                {
                    DoRemoveMapDataByIndex(mapDataIndex);
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            if(mSceneGUISwitchProperty != null && mSceneGUISwitchProperty.boolValue)
            {
                var currentEvent = Event.current;
                if (currentEvent.type == EventType.Repaint)
                {
                    if (mMapLineGUISwitchProperty.boolValue)
                    {
                        DrawMapLines();
                    }
                    if (mMapObjectSceneGUISwitchProperty != null && mMapObjectSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapObjectDataLabels();
                    }
                    if(mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapDataLabels();
                        DrawMapDataSpheres();
                        DrawMapCustomDataHandles();
                    }
                }

                if(currentEvent.type == EventType.KeyDown)
                {
                    MarkSceneKeyDown(currentEvent.keyCode);
                }
                if(currentEvent.type == EventType.KeyUp)
                {
                    MarkSceneKeyUp(currentEvent.keyCode);
                }
                if(IsSceneKeyCodeDown(KeyCode.W))
                {
                    OnWKeyboardClick();
                }
                if(IsSceneKeyCodeDown(KeyCode.E))
                {
                    OnEKeyboardClick();
                }
            }
        }

        /// <summary>
        /// 响应W按键按下
        /// </summary>
        private void OnWKeyboardClick()
        {
            if (mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
            {
                DrawMapDataPositionHandles();
            }
        }

        /// <summary>
        /// 响应E按键按下
        /// </summary>
        private void OnEKeyboardClick()
        {
            if (mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
            {
                DrawMapDataRotationHandles();
            }
        }

        /// <summary>
        /// 绘制线条
        /// </summary>
        private void DrawMapLines()
        {
            foreach (var horizontalLineData in mHDrawLinesDataList)
            {
                Handles.DrawLine(horizontalLineData.Key, horizontalLineData.Value, 2f);
            }
            foreach (var verticalLineData in mVDrawLinesDataList)
            {
                Handles.DrawLine(verticalLineData.Key, verticalLineData.Value, 2f);
            }
        }

        /// <summary>
        /// 绘制地图对象数据标签
        /// </summary>
        private void DrawMapObjectDataLabels()
        {
            for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var guiSwitchOffProperty = mapObjectDataProperty.FindPropertyRelative("GUISwitchOff");
                if (guiSwitchOffProperty.boolValue)
                {
                    continue;
                }
                var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
                var go = goProperty.objectReferenceValue as GameObject;
                if(go == null)
                {
                    continue;
                }
                var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                var mapObjectUID = uidProperty.intValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
                var mapObjectDes = mapObjectConfig != null ? mapObjectConfig.Des : "";
                var mapObjectLabelName = $"[{i}]{mapObjectDes}";
                var labelPos = go.transform.position + MapEditorConst.MapObjectDataLabelPosOffset;
                Handles.Label(labelPos, mapObjectLabelName, mRedLabelGUIStyle);
            }
        }

        /// <summary>
        /// 绘制地图埋点标签
        /// </summary>
        private void DrawMapDataLabels()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                DrawMapDataLabel(i);
            }
        }

        /// <summary>
        /// 绘制指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        private void DrawMapDataLabel(int index)
        {
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(index);
            if(mapDataProperty == null)
            {
                return;
            }
            var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative("GUISwitchOff");
            if (guiSwitchOffProperty.boolValue)
            {
                return;
            }
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uidProperty.intValue);
            if (mapDataConfig == null)
            {
                return;
            }
            var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
            string mapDataLabelName = MapEditorUtilities.GetMapDataPropertyLabelName(mapDataConfig, mMapDataListProperty, index);
            var mapDataBatchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            var labelPos = mapDataPositionProperty.vector3Value + MapEditorConst.MapDataLabelPosOffset;
            var displayGUIStyle = mapDataBatchOperationSwitchProperty.boolValue ? mYellowLabelGUIStyle : mRedLabelGUIStyle;
            Handles.Label(labelPos, mapDataLabelName, displayGUIStyle);
        }

        /// <summary>
        /// 绘制地图埋点球体
        /// </summary>
        private void DrawMapDataSpheres()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                DrawMapDataSphere(i);
            }
        }

        /// <summary>
        /// 绘制指定索引的地图埋点球体
        /// </summary>
        /// <param name="index"></param>
        private void DrawMapDataSphere(int index)
        {
            var mapDataProperty = GetMapDataSerializedPropertyByIndex(index);
            if(mapDataProperty == null)
            {
                return;
            }
            var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative("GUISwitchOff");
            if (guiSwitchOffProperty.boolValue)
            {
                return;
            }
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var mapDataUID = uidProperty.intValue;
            var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
            var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
            var mapDataPosition = mapDataPositionProperty.vector3Value;
            var mapDataRotation = mapDataRotationProperty.vector3Value;
            var rotationQuaternion = Quaternion.Euler(mapDataRotation);
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            var preHandlesColor = Handles.color;
            Handles.color = mapDataConfig != null ? mapDataConfig.SceneSphereColor : Color.gray;
            Handles.SphereHandleCap(index, mapDataPosition, rotationQuaternion, MapEditorConst.MapDataSphereSize, EventType.Repaint);
            Handles.color = preHandlesColor;
        }

        /// <summary>
        /// 绘制自定义数据的Handles
        /// </summary>
        private void DrawMapCustomDataHandles()
        {
            for(int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                if(mapDataProperty == null)
                {
                    continue;
                }
                var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative("GUISwitchOff");
                if(guiSwitchOffProperty.boolValue)
                {
                    continue;
                }
                var mapData = mapDataProperty.managedReferenceValue as MapData;
                DrawMapCustomDataHandles(mapData);
            }
        }

        /// <summary>
        /// 绘制指定地图埋点属性对象的自定义Handles
        /// </summary>
        /// <param name="mapData"></param>
        private void DrawMapCustomDataHandles(MapData mapData)
        {
            if(mapData == null)
            {
                return;
            }
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
            if(mapDataConfig == null)
            {
                return;
            }
        }

        /// <summary>
        /// 绘制所有地图埋点数据坐标操作PositionHandle
        /// </summary>
        private void DrawMapDataPositionHandles()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                if (mapDataProperty == null)
                {
                    continue;
                }
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
                EditorGUI.BeginChangeCheck();
                var newTargetPosition = Handles.PositionHandle(mapDataPositionProperty.vector3Value, rotationQuaternion);
                if(EditorGUI.EndChangeCheck())
                {
                    var positionOffset = newTargetPosition - mapDataPositionProperty.vector3Value;
                    OnMapDataPositionMove(i, positionOffset);
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 绘制所有地图埋点数据坐标操作RoationHandle
        /// </summary>
        private void DrawMapDataRotationHandles()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                if(mapDataProperty == null)
                {
                    continue;
                }
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
                EditorGUI.BeginChangeCheck();
                var newTargetRotation = Handles.RotationHandle(rotationQuaternion, mapDataPositionProperty.vector3Value);
                if (EditorGUI.EndChangeCheck())
                {
                    mapDataRotationProperty.vector3Value = newTargetRotation.eulerAngles;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }

        /// <summary>
        /// 检查快捷键
        /// </summary>
        private void CheckShortcuts()
        {
            CheckOnekeyBatchShortcuts();
        }

        /// <summary>
        /// 检查一键批量快捷键
        /// </summary>
        private void CheckOnekeyBatchShortcuts()
        {
            if ((IsInspectorKeyCodeDown(KeyCode.LeftShift) || IsInspectorKeyCodeDown(KeyCode.RightShift)) &&
                (IsInspectorKeyCodeDown(KeyCode.LeftAlt) || IsInspectorKeyCodeDown(KeyCode.RightAlt)))
            {
                if(IsInspectorKeyCodeDown(KeyCode.S))
                {
                    OneKeySwitchBatchOperation(true);
                }
                else if(IsInspectorKeyCodeDown(KeyCode.C))
                {
                    OneKeySwitchBatchOperation(false);
                }
            }
        }
    }
}
