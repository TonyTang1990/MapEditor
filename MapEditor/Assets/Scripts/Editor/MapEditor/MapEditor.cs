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
        /// MapAreaGUISwitch属性
        /// </summary>
        private SerializedProperty mMapAreaGUISwitchProperty;

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
        /// GridSize属性
        /// </summary>
        private SerializedProperty mGridSizeProperty;

        /// <summary>
        /// MapTerrianAsset属性
        /// </summary>
        private SerializedProperty mMapTerrianAssetProperty;

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
        /// 地图对象操作区域是否展开显示
        /// </summary>
        private bool mMapObjectOperationUnfold = true;

        /// <summary>
        /// 地图对象数据是否展开显示
        /// </summary>
        private bool mMapObjectDataAreaUnfold = true;

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
        /// 地图埋点数据区域是否展开显示
        /// </summary>
        private bool mMapDataAreaUnfold = true;

        /// <summary>
        /// 地图自定义数据区域是否展开显示<地图埋点类型， 是否展开显示>
        /// </summary>
        private Dictionary<MapDataType, bool> mMapCustomDataAreaUnfoldMap = new Dictionary<MapDataType, bool>()
        {
            { MapDataType.MONSTER, true },
            { MapDataType.MONSTER_GROUP, true},
        };

        /// <summary>
        /// 地图自定义数据区域地图埋点类型列表
        /// </summary>
        private List<MapDataType> mMapCustomDataTypeList;

        /// <summary>
        /// 地图埋点类型和选项数组(显示名字数组)Map<地图埋点类型, 选项数组>
        /// </summary>
        private Dictionary<MapDataType, string[]> mMapDataChoiceOptionsMap = new Dictionary<MapDataType, string[]>();

        /// <summary>
        /// 地图埋点类型和选项值数组(UID数组)Map<地图埋点类型， 选项值数组>
        /// </summary>
        private Dictionary<MapDataType, int[]> mMapDataChoiceValuesMap = new Dictionary<MapDataType, int[]>();

        /// <summary>
        /// 按键按下Map<按键值， 是否按下>
        /// </summary>
        private Dictionary<KeyCode, bool> mKeyCodeDownMap = new Dictionary<KeyCode, bool>();

        private void Awake()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();
            mMapWidthProperty.intValue = mMapWidthProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapWidth : mMapWidthProperty.intValue;
            mMapHeightProperty.intValue = mMapHeightProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapHeight : mMapHeightProperty.intValue;
            mGridSizeProperty.floatValue = Mathf.Approximately(mGridSizeProperty.floatValue, 0f) ? MapSetting.GetEditorInstance().DefaultGridSize : mGridSizeProperty.floatValue;
            CreateAllNodes();
            UpdateMapObjectDataChoiceDatas();
            UpdateMapDataChoiceDatas();
            UpdateAddMapObjectDataPreviewAsset();
            UpdateMapSizeDrawDatas();
            UpdateMapGOPosition();
            UpdateTerrianSizeAndPos();
            UpdateMapObjectDataLogicDatas();
            UpdateGridSizeDrawDatas();
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
            mMaAreaGridGUISwitchProperty ??= serializedObject.FindProperty("MaAreaGridGUISwitch");            
            mMapAreaGUISwitchProperty ??= serializedObject.FindProperty("MapAreaGUISwitch");
            mMapObjectSceneGUISwitchProperty ??= serializedObject.FindProperty("MapObjectSceneGUISwitch");
            mMapDataSceneGUISwitchProperty ??= serializedObject.FindProperty("MapDataSceneGUISwitch");
            mMapObjectAddedAutoFocusProperty ??= serializedObject.FindProperty("MapObjectAddedAutoFocus");            
            mMapWidthProperty ??= serializedObject.FindProperty("MapWidth");
            mMapHeightProperty ??= serializedObject.FindProperty("MapHeight");
            mMapStartPosProperty ??= serializedObject.FindProperty("MapStartPos");
            mGridSizeProperty ??= serializedObject.FindProperty("GridSize");
            mMapTerrianAssetProperty ??= serializedObject.FindProperty("MapTerrianAsset");
            mMapObjectDataListProperty ??= serializedObject.FindProperty("MapObjectDataList");
            mMapDataListProperty ??= serializedObject.FindProperty("MapDataList");
            mAddMapObjectTypeProperty ??= serializedObject.FindProperty("AddMapObjectType");
            mAddMapObjectIndexProperty ??= serializedObject.FindProperty("AddMapObjectIndex");
            mAddMapDataTypeProperty ??= serializedObject.FindProperty("AddMapDataType");
            mAddMapDataIndexProperty ??= serializedObject.FindProperty("AddMapDataIndex");
            mExportTypeProperty ??= serializedObject.FindProperty("ExportType");
        }

        /// <summary>
        /// 初始化GUIStyles
        /// </summary>
        private void InitGUIStyles()
        {
            if(mRedLabelGUIStyle == null)
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
        /// 创建所有节点
        /// </summary>
        private void CreateAllNodes()
        {
            if(!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            CreateAllMapObjectParentNodes();
            CreateMapTerrianNode();
            CreateNavMeshSurface();
        }

        /// <summary>
        /// 创建所有地图对象父节点挂点
        /// </summary>
        private void CreateAllMapObjectParentNodes()
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapObjectParentNode = MapUtilities.GetOrCreateMapObjectParentNode(mTarget?.gameObject);
            if(mapObjectParentNode != null)
            {
                mapObjectParentNode.transform.localPosition = Vector3.zero;
            }

            var mapObjectTypeValues = Enum.GetValues(MapConst.MapObjectType);
            foreach(var mapObjectTypeValue in mapObjectTypeValues)
            {
                var mapObjectType = (MapObjectType)mapObjectTypeValue;
                var mapObjectTypeParentNodeTransform = MapUtilities.GetOrCreateMapObjectTypeParentNode(mTarget?.gameObject, mapObjectType);
                mapObjectTypeParentNodeTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 创建地图地形节点
        /// </summary>
        private void CreateMapTerrianNode()
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var customAsset = mMapTerrianAssetProperty.objectReferenceValue as GameObject;
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject, customAsset);
            if(mapTerrianTransform != null)
            {
                mapTerrianTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 重新创建地形节点
        /// </summary>
        private void RecreateMapTerrianNode()
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject);
            if(mapTerrianTransform != null)
            {
                GameObject.DestroyImmediate(mapTerrianTransform.gameObject);
            }
            CreateMapTerrianNode();
            UpdateTerrianSizeAndPos();
        }

        /// <summary>
        /// 创建寻路组件
        /// </summary>
        private void CreateNavMeshSurface()
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            MapEditorUtilities.GetOrCreateNavMeshSurface(mTarget?.gameObject);
        }

        /// <summary>
        /// 更新地图地块大小和位置
        /// </summary>
        private void UpdateTerrianSizeAndPos()
        {
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget?.gameObject);
            if(mapTerrianTransform == null)
            {
                return;
            }

            var mapWidth = mMapWidthProperty.intValue;
            var mapHeight = mMapHeightProperty.intValue;
            mapTerrianTransform.localScale = new Vector3(0.1f * mapWidth, 1, 0.1f * mapHeight);
            mapTerrianTransform.localPosition = new Vector3(mapWidth / 2, 0, mapHeight / 2);
            var meshRenderer = mapTerrianTransform.GetComponent<MeshRenderer>();
            if(meshRenderer != null)
            {
                meshRenderer.sharedMaterial.SetTextureScale("_MainTex", new Vector2(mapWidth, mapHeight));
            }
        }

        /// <summary>
        /// 更新地图对象数据的逻辑数据(对象存在时才更新记录逻辑数据)
        /// </summary>
        private void UpdateMapObjectDataLogicDatas()
        {
            // 地图对象可能删除还原，所以需要逻辑层面记录数据
            for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                var mapObjectUID = uidProperty.intValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
                if(mapObjectConfig == null)
                {
                    continue;
                }
                var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
                if(goProperty.objectReferenceValue != null)
                {
                    var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                    var rotationProperty = mapObjectDataProperty.FindPropertyRelative("Rotation");
                    var localScaleProperty = mapObjectDataProperty.FindPropertyRelative("LocalScale");
                    var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                    var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                    var colliderRadiusProperty = mapObjectDataProperty.FindPropertyRelative("ColliderRadius");
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
            if(mMapObjectDataChoiceOptionsMap == null || mMapObjectDataChoiceOptionsMap.Count == 0)
            {
                return;
            }
            var addMapObjectType = (MapObjectType)mAddMapObjectTypeProperty.intValue;
            var mapObjectDataValueOptions = mMapObjectDataChoiceValuesMap[addMapObjectType];
            var addMapObjectIndexValue = mAddMapObjectIndexProperty.intValue;
            if(!MapEditorUtilities.IsIntValueInArrays(addMapObjectIndexValue, mapObjectDataValueOptions))
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
                mAddMapDataIndexProperty.intValue = mapDataValueOptions[0];
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 更新地图对象选择数据
        /// </summary>
        private void UpdateMapObjectDataChoiceDatas()
        {
            var objectSetting = MapSetting.GetEditorInstance().ObjectSetting;
            var mapObjectTypeValues = Enum.GetValues(MapConst.MapObjectType);
            mMapObjectDataChoiceOptionsMap.Clear();
            mMapObjectDataChoiceValuesMap.Clear();
            foreach(var mapObjectTypeValue in mapObjectTypeValues)
            {
                var mapObjectType = (MapObjectType)mapObjectTypeValue;
                var mapObjectTypeAllConfigs = objectSetting.GetAllMapObjectConfigByType(mapObjectType);
                var mapObjectTypeConfigNum = mapObjectTypeAllConfigs.Count;
                string[] allChoiceOptions = new string[mapObjectTypeConfigNum];
                int[] allValueOptions = new int[mapObjectTypeConfigNum];
                for(int i = 0; i < mapObjectTypeConfigNum; i++)
                {
                    var mapObjectTypeAllConfig = mapObjectTypeAllConfigs[i];
                    allChoiceOptions[i] = mapObjectTypeAllConfig.GetOptionName();
                    allValueOptions[i] = mapObjectTypeAllConfig.UID;
                }
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
            var dataSetting = MapSetting.GetEditorInstance().DataSetting;
            var mapDataTypeValues = Enum.GetValues(MapConst.MapDataType);
            mMapDataChoiceOptionsMap.Clear();
            mMapDataChoiceValuesMap.Clear();
            foreach (var mapDataTypeValue in mapDataTypeValues)
            {
                var mapDataType = (MapDataType)mapDataTypeValue;
                var mapDataTypeAllConfigs = dataSetting.GetAllMapObjectConfigByType(mapDataType);
                var mapDataTypeConfigNum = mapDataTypeAllConfigs.Count;
                string[] allChoiceOptions = new string[mapDataTypeConfigNum];
                int[] allValueOptions = new int[mapDataTypeConfigNum];
                for (int i = 0; i < mapDataTypeConfigNum; i++)
                {
                    var mapObjectTypeAllConfig = mapDataTypeAllConfigs[i];
                    allChoiceOptions[i] = mapObjectTypeAllConfig.GetOptionName();
                    allValueOptions[i] = mapObjectTypeAllConfig.UID;
                }
                mMapDataChoiceOptionsMap.Add(mapDataType, allChoiceOptions);
                mMapDataChoiceValuesMap.Add(mapDataType, allValueOptions);
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
            if(!mMapDataChoiceOptionsMap.TryGetValue(mapDataType, out mapDataChoiceOptions))
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
            if(addMapObjectValue != 0)
            {
                var addMapObjectUID = addMapObjectValue;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(addMapObjectUID);
                if(mapObjectConfig != null && mapObjectConfig.Asset != null)
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
            for(int i = 0; i < mapVerticalLineNum; i++)
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
        /// 更新地图区域九宫格大小绘制数据
        /// </summary>
        private void UpdateGridSizeDrawDatas()
        {
            mGridDataList.Clear();
            var gridSize = mGridSizeProperty.floatValue;
            var mapWidth = mMapWidthProperty.intValue;
            var mapHeight = mMapHeightProperty.intValue;
            var startPos = mMapStartPosProperty.vector3Value;
            var mapStartGridXZ = MapEditorUtilities.GetGridXZByPosition(startPos, gridSize);
            var maxMapPos = startPos;
            maxMapPos.x = maxMapPos.x + mapWidth;
            maxMapPos.z = maxMapPos.z + mapHeight;
            var mapMaxGridXZ = MapEditorUtilities.GetGridXZByPosition(maxMapPos, gridSize);
            var gridMinX = mapStartGridXZ.Key;
            var gridMinZ = mapStartGridXZ.Value;
            var gridMaxX = mapMaxGridXZ.Key;
            var gridMaxZ = mapMaxGridXZ.Value;
            for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                var position = positionProperty.vector3Value;
                var gridXZ = MapEditorUtilities.GetGridXZByPosition(position, gridSize);
                gridMinX = Mathf.Min(gridMinX, gridXZ.Key);
                gridMinZ = Mathf.Min(gridMinZ, gridXZ.Value);
                gridMaxX = Mathf.Min(gridMaxX, gridMaxZ.Key);
                gridMaxZ = Mathf.Min(gridMaxZ, gridMaxZ.Value);
            }
            var gridVector3Size = new Vector3(gridSize, 0, gridSize);
            var halfGridVector3Size = gridVector3Size / 2;
            for(int gridX = gridMinX; gridX <= gridMaxX; gridX++)
            {
                for(int gridZ = gridMinZ; gridZ <= gridMaxZ; gridZ++)
                {
                    var gridCenterData = new Vector3(gridX * gridSize + halfGridVector3Size.x, 0, gridZ * gridSize + halfGridVector3Size.z);
                    var gridUID = MapEditorUtilities.GetGridUID(gridX, gridZ);
                    var gridData = new KeyValuePair<Vector3, int>(gridCenterData, gridUID);
                    mGridDataList.Add(gridData);
                }
            }
        }

        /// <summary>
        /// 更新地图对象位置
        /// </summary>
        private void UpdateMapGOPosition()
        {
            if(mTarget == null)
            {
                return;
            }
            mTarget.transform.position = mMapStartPosProperty.vector3Value;
        }

        /// <summary>
        /// 交换指定属性和交换索引数据
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="exchangeIndex1"></param>
        /// <param name="exchangeIndex2"></param>
        /// <returns></returns>
        private bool ExchangeMapDataByIndex(SerializedProperty propertyList, int exchangeIndex1, int exchangeIndex2)
        {
            if(propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，交换属性数据位置失败！");
                return false;
            }
            if(exchangeIndex1 == exchangeIndex2)
            {
                return true;
            }
            var dataNum = propertyList.arraySize;
            if(exchangeIndex1 < 0 || exchangeIndex2 >= dataNum || exchangeIndex2 < 0 || exchangeIndex2 >= dataNum)
            {
                Debug.LogError($"指定交换索引1:{exchangeIndex1}或交换索引2:{exchangeIndex2}不是有效索引范围:0-{dataNum - 1},交换属性数据位置失败！");
                return false;
            }
            var mapDataIndex2Property = propertyList.GetArrayElementAtIndex(exchangeIndex2);
            var mapData2 = mapDataIndex2Property.managedReferenceValue;
            var mapDataIndex1Property = propertyList.GetArrayElementAtIndex(exchangeIndex1);
            var mapData1 = mapDataIndex1Property.managedReferenceValue;
            mapDataIndex1Property.managedReferenceValue = mapData2;
            mapDataIndex2Property.managedReferenceValue = mapData1;
            //Debug.Log($"交换属性数据索引1:{exchangeIndex1}和索引2:{exchangeIndex2}成功！");
            return true;
        }

        /// <summary>
        /// 执行将指定属性对象和指定索引的数据向上移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoMovePropertyDataUpByIndex(SerializedProperty propertyList, int index)
        {
            if(!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return false;
            }
            return MovePropertyDataUpByIndex(propertyList, index);
        }

        /// <summary>
        /// 将指定属性对象和指定索引的数据向上移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool MovePropertyDataUpByIndex(SerializedProperty propertyList, int index)
        {
            if (propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，向上移动属性数据失败！");
                return false;
            }
            var mapDataNum = propertyList.arraySize;
            if(index < 0 || index > mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum},向上移动属性数据失败！");
                return false;
            }
            var newIndex = Math.Clamp(index - 1, 0, mapDataNum);
            ExchangeMapDataByIndex(propertyList, index, newIndex);
            return true;
        }

        /// <summary>
        /// 执行将指定属性对象和指定索引的数据向下移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoMovePropertyDataDownByIndex(SerializedProperty propertyList, int index)
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return false;
            }
            return MovePropertyDataDownByIndex(propertyList, index);
        }

        /// <summary>
        /// 将指定属性对象和指定索引的数据向下移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool MovePropertyDataDownByIndex(SerializedProperty propertyList, int index)
        {
            if (propertyList == null || !propertyList.isArray)
            {
                Debug.LogError($"传递的属性对象为空或不是数组属性，向下移动属性数据失败！");
                return false;
            }
            var mapDataNum = propertyList.arraySize;
            if (index < 0 || index >= mapDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapDataNum - 1},向下移动属性数据失败！");
                return false;
            }
            var newIndex = Math.Clamp(index + 1, 0, mapDataNum - 1);
            ExchangeMapDataByIndex(propertyList, index, newIndex);
            return true;
        }

        /// <summary>
        /// 执行添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private MapObjectData DoAddMapObjectData(int uid, int insertIndex = -1)
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
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
            var instanceGo = CreateGameObjectByUID(uid);
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
            return newMapObjectData;
        }

        /// <summary>
        /// 执行移除指定索引的地图对象数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoRemoveMapObjectDataByIndex(int index)
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
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
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
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
                var insertMapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(insertMapDataPos);
                var insertMapData = insertMapDataProperty.managedReferenceValue as MapData;
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
            }
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition, mapDataConfig.Rotation);
            mMapDataListProperty.InsertArrayElementAtIndex(insertPos);
            var newMapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(insertPos);
            newMapDataProperty.managedReferenceValue = newMapData;
            serializedObject.ApplyModifiedProperties();
            return newMapData;
        }

        /// <summary>
        /// 执行移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoRemoveMapDataByIndex(int index)
        {
            if (!MapUtilities.IsOperationAvalible(mTarget?.gameObject))
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
            return true;
        }

        /// <summary>
        /// 获取当前脚本GameObject对应Asset
        /// Note:
        /// 未存储到本地Asset返回null
        /// </summary>
        /// <returns></returns>
        private string GetMapAssetPath()
        {
            string assetPath = null;
            var gameObjectStatus = MapUtilities.GetGameObjectStatus(mTarget?.gameObject);
            if (gameObjectStatus == GameObjectStatus.Normal)
            {
                return null;
            }
            else if (gameObjectStatus == GameObjectStatus.PrefabInstance)
            {
                var asset = PrefabUtility.GetCorrespondingObjectFromSource(mTarget?.gameObject);
                assetPath = AssetDatabase.GetAssetPath(asset);
            }
            else if (gameObjectStatus == GameObjectStatus.Asset)
            {
                assetPath = AssetDatabase.GetAssetPath(mTarget?.gameObject);
            }
            else if(gameObjectStatus == GameObjectStatus.PrefabContent)
            {
                var prefabStage = PrefabStageUtility.GetPrefabStage(mTarget?.gameObject);
                assetPath = prefabStage != null ? prefabStage.assetPath : null;
            }
            return assetPath;
        }

        /// <summary>
        /// 标记指定按键按下
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkKeyDown(KeyCode keyCode)
        {
            if(!mKeyCodeDownMap.ContainsKey(keyCode))
            {
                mKeyCodeDownMap.Add(keyCode, true);
            }
            else
            {
                mKeyCodeDownMap[keyCode] = true;
            }
        }

        /// <summary>
        /// 标记指定按键释放
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkKeyUp(KeyCode keyCode)
        {
            if(!mKeyCodeDownMap.ContainsKey(keyCode))
            {
                mKeyCodeDownMap.Add(keyCode, false);
            }
            else
            {
                mKeyCodeDownMap[keyCode] = false;
            }
        }

        /// <summary>
        /// 标记所有按键释放
        /// </summary>
        private void UnmarkAllKeyUp()
        {
            if(mKeyCodeDownMap.Count == 0)
            {
                return;
            }
            mKeyCodeDownMap.Clear();
        }

        /// <summary>
        /// 指定按键是否按下
        /// </summary>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        private bool IsKeyCodeDown(KeyCode keyCode)
        {
            bool result = false;
            if(mKeyCodeDownMap.TryGetValue(keyCode, out result))
            {
                return result;
            }
            return false;
        }

        /// <summary>
        /// 一键清除地图埋点批量勾选
        /// </summary>
        private void OneKeySwitchOffMapDataBatchOperation()
        {
            mTarget?.ClearAllMapDataBatchOperation();
        }

        /// <summary>
        /// 清除动态数据
        /// </summary>
        private void CleanDynamicDatas()
        {
            if(!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            UpdateMapObjectDataLogicDatas();
            CleanDynamicMapObjectGos();
        }

        /// <summary>
        /// 清除动态地图对象GameObjects
        /// </summary>
        private void CleanDynamicMapObjectGos()
        {
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
                if (mapObjectConfig.IsDynamic && goProperty.objectReferenceValue != null)
                {
                    var go = goProperty.objectReferenceValue as GameObject;
                    GameObject.DestroyImmediate(go);
                    goProperty.objectReferenceValue = null;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
        /// <summary>
        /// 恢复指定MapObject属性地图对象
        /// </summary>
        /// <param name="mapObjectDataProperty"></param>
        private void RecreateMapObjectGo(SerializedProperty mapObjectDataProperty)
        {
            if(mapObjectDataProperty == null)
            {
                Debug.LogError($"不支持传空地图对象属性，重创地图对象失败！");
                return;
            }
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            var mapObjectUID = uidProperty.intValue;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if(mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，重创地图对象失败！");
                return;
            }
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            var gameObject = goProperty.objectReferenceValue as GameObject;
            if(gameObject != null)
            {
                GameObject.DestroyImmediate(gameObject);
            }
            var instanceGo = mTarget != null ? mTarget.CreateGameObjectByUID(mapObjectUID) : null;
            if(instanceGo != null)
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
        /// 恢复动态数据
        /// </summary>
        private void RecoverDynamicDatas()
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            UpdateMapObjectDataLogicDatas();
            RecoverDynamicMapObjectGos();
        }

        /// <summary>
        /// 恢复动态地图对象GameObjects
        /// </summary>
        private void RecoverDynamicMapObjectGos()
        {
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
                if (mapObjectConfig.IsDynamic && goProperty.objectReferenceValue == null)
                {
                    RecreateMapObjectGo(mapObjectDataProperty);
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 拷贝寻路数据Asset
        /// </summary>
        private void CopyNavMeshAsset()
        {
            var navMeshSurface = mTarget.GetComponent<NavMeshSurface>();
            if(navMeshSurface == null)
            {
                EditorUtility.DisplayDialog("拷贝寻路数据Asset", "找不到寻路NavMeshSurface组件，烘焙和拷贝寻路数据Asset失败！", "确认");
                return;
            }
            var mapAssetPath = GetMapAssetPath();
            if(string.IsNullOrEmpty(mapAssetPath))
            {
                EditorUtility.DisplayDialog("拷贝寻路数据Asset", $"当前对象:{mTarget.name}未保存成任何本地Asset，复制寻路数据Asset失败！", "确认");
                return;
            }
            var navMeshAssetPath = AssetDatabase.GetAssetPath(navMeshSurface.navMeshData);
            if(navMeshSurface.navMeshData == null || string.IsNullOrEmpty(navMeshAssetPath))
            {
                EditorUtility.DisplayDialog("拷贝寻路数据Asset", "未烘焙任何有效寻路数据Asset，复制寻路数据Asset失败！", "确认");
                return;
            }
            navMeshAssetPath = PathUtilities.GetRegularPath(navMeshAssetPath);
            var targetAssetFolderPath = Path.GetDirectoryName(mapAssetPath);
            var navMeshAssetName = Path.GetFileName(navMeshAssetPath);
            var newNavMeshAssetPath = Path.Combine(targetAssetFolderPath, navMeshAssetName);
            newNavMeshAssetPath = PathUtilities.GetRegularPath(newNavMeshAssetPath);
            if (!string.Equals(navMeshAssetPath, newNavMeshAssetPath))
            {
                AssetDatabase.MoveAsset(navMeshAssetPath, newNavMeshAssetPath);
                Debug.Log($"移动寻路数据Asset:{navMeshAssetPath}到{newNavMeshAssetPath}成功！");
            }
            else
            {
                Debug.Log($"移动寻路数据Asset:{navMeshAssetPath}已经在目标位置，不需要移动！");
            }
        }

        /// <summary>
        /// 一键重创地图对象
        /// </summary>
        private void OneKeyRecreateMapObjectGos()
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
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
        /// 更新所有地图对象的MapObjectDataMono数据到最新
        /// </summary>
        private void UpdateAllMapObjectDataMonos()
        {
            for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
                var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
                if(goProperty.objectReferenceValue == null)
                {
                    continue;
                }
                var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
                var go = goProperty.objectReferenceValue as GameObject;
                MapUtilities.AddOrUpdateMapObjectDataMono(go, uidProperty.intValue);
            }
        }

        /// <summary>
        /// 导出地图数据
        /// </summary>
        private void ExportMapData()
        {
            if(!MapEditorUtilities.CheckIsGameMapAvalibleExport(mTarget))
            {
                EditorUtility.DisplayDialog("导出地图数据", "场景数据有问题，不满足导出条件，导出场景数据失败！", "确认");
                return;
            }
            // 流程上说场景给客户端使用一定会经历导出流程
            // 在导出时确保MapObjectDataMono和地图对象配置数据一致
            // 从而确保场景资源被使用时挂在数据和配置匹配
            UpdateAllMapObjectDataMonos();
            // 确保所有数据运用到最新
            serializedObject.ApplyModifiedProperties();
            var isPrefabAssetInstance = PrefabUtility.IsPartOfPrefabInstance(mTarget?.gameObject);
            // 确保数据应用到对应Asset上
            if(isPrefabAssetInstance)
            {
                PrefabUtility.ApplyPrefabInstance(mTarget?.gameObject, InteractionMode.AutomatedAction);
            }
            MapEditorUtilities.ExportGameMapData(mTarget);
        }

        /// <summary>
        /// 一键烘焙拷贝和导出地图数据
        /// </summary>
        private async void OneKeyBakeAndExport()
        {
            if(!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            RecoverDynamicDatas();
            var navMeshSurface = MapEditorUtilities.GetOrCreateNavMeshSurface(mTarget?.gameObject);
            var bakePathTask = BakePathTask(navMeshSurface);
            var bakePathResult = await bakePathTask;
            CopyNavMeshAsset();
            CleanDynamicDatas();
            ExportMapData();
            AssetDatabase.SaveAssets();
            Debug.Log($"一键烘焙拷贝导出地图数据完成！");
        }

        /// <summary>
        /// 烘焙寻路任务
        /// </summary>
        /// <param name="navMeshSurface"></param>
        /// <returns></returns>
        private async Task<bool> BakePathTask(NavMeshSurface navMeshSurface)
        {
            var navMeshSurfaces = new UnityEngine.Object[] { navMeshSurface };
            var navMeshDataAssetPath = navMeshSurface.navMeshData != null ? AssetDatabase.GetAssetPath(navMeshSurface.navMeshData) : null;
            if(!string.IsNullOrEmpty(navMeshDataAssetPath))
            {
                NavMeshAssetManager.instance.ClearSurfaces(navMeshSurfaces);
                AssetDatabase.DeleteAsset(navMeshDataAssetPath);
                // 确保删除成功
                while(AssetDatabase.LoadAssetAtPath<NavMeshData>(navMeshDataAssetPath) != null)
                {
                    await Task.Delay(1);
                }
            }
            NavMeshAssetManager.instance.StartBakingSurfaces(navMeshSurfaces);
            while(NavMeshAssetManager.instance.IsSurfaceBaking(navMeshSurface))
            {
                await Task.Delay(1);
            }
            return true;
        }

        /// <summary>
        /// 响应添加地图对象类型选择变化
        /// </summary>
        private void OnAddMapObjectTypeChange()
        {
            var addMapObjectType = (MapObjectType)mAddMapObjectTypeProperty.intValue;
            var mapObjectDataChoiceValues = mMapObjectDataChoiceValuesMap[addMapObjectType];
            if(mapObjectDataChoiceValues != null && mapObjectDataChoiceValues.Length > 0)
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
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(mapObjectDataIndex);
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            uidProperty.intValue = newUID;
            OnMapObjectDataUIDChange(mapObjectDataIndex);
        }

        /// <summary>
        /// 响应指定地图对象索引的UID变化
        /// </summary>
        /// <param name="mapObjectDataIndex"></param>
        private void OnMapObjectDataUIDChange(int mapObjectDataIndex)
        {
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(mapObjectDataIndex);
            RecreateMapObjectGo(mapObjectDataProperty);
        }

        /// <summary>
        /// 响应添加地图埋点索引选择变化
        /// </summary>
        private void OnAddMapDataIndexChange()
        {

        }

        /// <summary>
        /// 执行指定地图埋点索引的UID变化
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="newUID"></param>
        private void DoChangeMapDataUID(int mapDataIndex, int newUID)
        {
            if(!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
            {
                return;
            }
            var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(mapDataIndex);
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            uidProperty.intValue = newUID;
            OnMapDataUIDChange(mapDataIndex);
        }

        /// <summary>
        /// 响应指定地图埋点索引的UID变化
        /// </summary>
        /// <param name="mapDataIndex"></param>
        private void OnMapDataUIDChange(int mapDataIndex)
        {

        }

        /// <summary>
        /// 响应指定索引的地图数据埋点位置移动
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="positionOffset"></param>
        private void OnMapDataPositionMove(int mapDataIndex, Vector3 positionOffset)
        {
            var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(mapDataIndex);
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            positionProperty.vector3Value = positionProperty.vector3Value + positionOffset;
            var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            if(batchOperationSwitchProperty.boolValue)
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
            for(int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                if(i == mapDataIndex)
                {
                    continue;
                }
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var batchOperationSwitch = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                if(batchOperationSwitch.boolValue)
                {
                    var positionProperty = mapDataProperty.FindPropertyRelative("Position");
                    positionProperty.vector3Value = positionProperty.vector3Value + positionOffset;
                }
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

            // 确保对SerializedObjec和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapLineGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapAreaGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapObjectSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapDataSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapObjectAddedAutoFocusProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapWidthProperty);
            EditorGUILayout.PropertyField(mMapHeightProperty);
            if(EditorGUI.EndChangeCheck())
            {
                UpdateMapSizeDrawDatas();
                UpdateMapGOPosition();
                UpdateTerrianSizeAndPos();
                UpdateGridSizeDrawDatas();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapStartPosProperty);
            if(EditorGUI.EndChangeCheck())
            {
                UpdateMapSizeDrawDatas();
                UpdateGridSizeDrawDatas();
                UpdateMapGOPosition();
            }

            EditorGUILayout.BeginChangeCheck();
            EditorGUILayout.PropertyField(mGridSizeProperty);
            if(EditorGUI.EndChangeCheck())
            {
                UpdateGridSizeDrawDatas();
            }

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapTerrianAssetProperty);
            if(EditorGUI.EndChangeCheck())
            {
                RecreateMapTerrianNode();
            }

            DrawGameMapButtonArea();
            DrawMapOperationInspectorArea();

            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制游戏地图按钮区域
        /// </summary>
        private void DrawGameMapButtonArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            if(GUILayout.Button("清除动态数据", GUILayout.ExpandWidth(true)))
            {
                CleanDynamicDatas();
            }
            if(GUILayout.Button("恢复动态数据", GUILayout.ExpandWidth(true)))
            {
                RecoverDynamicDatas();
            }
            if(GUILayout.Button("拷贝NavMesh Asset", GUILayout.ExpandWidth(true)))
            {
                CopyNavMeshAsset();
            }
            if(GUILayout.Button("一键重创地图对象", GUILayout.ExpandWidth(true)))
            {
                OneKeyRecreateMapObjectGos();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("导出类型:", GUILayout.Width(60f));
            mExportTypeProperty.intValue = (int)(ExportType)EditorGUILayout.EnumPopup((ExportType)mExportTypeProperty.intValue, GUILayout.Width(150f));
            if(GUILayout.Button("导出地图数据", GUILayout.ExpandWidth(true)))
            {
                ExportMapData();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("一键烘焙拷贝导出", GUILayout.ExpandWidth(true)))
            {
                OneKeyBakeAndExport();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图操作Inspector区域
        /// </summary>
        private void DrawMapOperationInspectorArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            mPanelToolBarSelectIndex = GUILayout.Toolbar(mPanelToolBarSelectIndex, mPanelToolBarStrings, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            mSelectedTabType = (MapTabType)mPanelToolBarSelectIndex;
            EditorGUILayout.EndHorizontal();
            if(mSelectedTabType == MapTabType.MapBuild)
            {
                DrawMapObjectInspectorArea();
            }
            else if(mSelectedTabType == MapTabType.DataEditor)
            {
                DrawMapDataInspectorArea();
            }
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
            if(mMapObjectDataChoiceOptionsMap != null && mMapObjectDataChoiceOptionsMap.Count > 0)
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
                if(EditorGUI.EndChangeCheck())
                {
                    OnAddMapObjectIndexChange();
                }
                if(GUILayout.Button("+", GUILayout.ExpandWidth(true)))
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
            mMapObjectDataAreaUnfold = EditorGUILayout.Foldout(mMapObjectDataAreaUnfold, "地图对象数据列表");
            if(mMapObjectDataAreaUnfold)
            {
                DrawMapObjectTitleArea();
                for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
                {
                    DrawOneMapObjectPropertyByIndex(i);
                }
            }
        }

        /// <summary>
        /// 绘制地图对象数据标题区域
        /// </summary>
        private void DrawMapObjectTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("是否动态", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("实体对象", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("位置", MapStyles.TabMiddleStyle, GUILayout.Width(160f));
            EditorGUILayout.LabelField("描述", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("上移", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("下移", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("添加", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("删除", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
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
            var uidProperty = mapObjectDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            EditorGUILayout.LabelField($"{mapObjectDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            EditorGUI.BeginChangeCheck();
            var mapObjectType = mapObjectConfig != null ? mapObjectConfig.ObjectType : MapObjectType.TREASURE_BOX;
            var mapObjectDataChoiceOptions = GetMapObjectDataChoiceOptionsByType(mapObjectType);
            var mapObjectDataChoiceValues = GetMapObjectDataChoiceValuesByType(mapObjectType);
            uid = EditorGUILayout.IntPopup(uid, mapObjectDataChoiceOptions, mapObjectDataChoiceValues, GUILayout.Width(150f));
            if(EditorGUI.EndChangeCheck())
            {
                DoChangeMapObjectDataUID(mapObjectDataIndex, uid);
            }
            if(mapObjectConfig != null)
            {
                EditorGUILayout.LabelField(mapObjectConfig.ObjectType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.Space(20f, false);
                EditorGUILayout.Toggle(mapObjectConfig.IsDynamic, GUILayout.Width(40f));
                EditorGUILayout.IntField(mapObjectConfig.ConfId, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            }
            else
            {
                EditorGUILayout.LabelField("找不到对象类型数据", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.LabelField("找不到是否动态数据", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
                EditorGUILayout.LabelField("找不到关联Id数据", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            }
            var gameObject = goProperty.objectReferenceValue as GameObject;
            EditorGUILayout.ObjectField(goProperty.objectReferenceValue, MapConst.GameObjectType, true, GUILayout.Width(150f));
            var newVector3Value = gameObject != null ? gameObject.transform.position : Vector3.zero;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("X", GUILayout.Width(10f));
            newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(39f));
            if (gameObject != null && EditorGUI.EndChangeCheck())
            {
                gameObject.transform.position = newVector3Value;
            }
            var des = mapObjectConfig != null ? mapObjectConfig.Des : "";
            EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            if (GUILayout.Button("↑", GUILayout.Width(40f)))
            {
                DoMovePropertyDataUpByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("↓", GUILayout.Width(40f)))
            {
                DoMovePropertyDataDownByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("+", GUILayout.Width(40f)))
            {
                var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
                DoAddMapObjectData(addMapObjectValue, mapObjectDataIndex);
            }
            if (GUILayout.Button("-", GUILayout.Width(40f)))
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
            if (mMapDataChoiceOptionsMap.Count > 0)
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
                if(GUILayout.Button("一键清除批量勾选", GUILayout.ExpandWidth(true)))
                {
                    OneKeySwitchOffMapDataBatchOperation();
                }
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
            DrawMapCommonDataArea();
            DrawMapCustomDataArea();
        }

        /// <summary>
        /// 绘制地图埋点通用数据区域
        /// </summary>
        private void DrawMapCommonDataArea()
        {
            mMapDataAreaUnfold = EditorGUILayout.Foldout(mMapDataAreaUnfold, "地图埋点数据列表");
            if(mMapDataAreaUnfold)
            {
                DrawMapDataTitleArea();
                for(int i = 0; i < mMapDataListProperty.arraySize; i++)
                {
                    DrawOneMapDataPropertyByIndex(i);
                }
            }
        }

        /// <summary>
        /// 绘制地图埋点数据标题区域
        /// </summary>
        private void DrawMapDataTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("批量", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("埋点类型", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("位置", MapStyles.TabMiddleStyle, GUILayout.Width(160f));
            EditorGUILayout.LabelField("旋转", MapStyles.TabMiddleStyle, GUILayout.Width(160f));
            EditorGUILayout.LabelField("描述", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("上移", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("下移", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("添加", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("删除", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定索引单个MapData属性
        /// </summary>
        /// <param name="mapDataIndex"></param>
        private void DrawOneMapDataPropertyByIndex(int mapDataIndex)
        {
            EditorGUILayout.BeginHorizontal();
            var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(mapDataIndex);
            var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
            EditorGUILayout.Space(10f, false);
            batchOperationSwitchProperty.boolValue = EditorGUILayout.Toggle(batchOperationSwitchProperty.boolValue, GUILayout.Width(30f));
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            EditorGUILayout.LabelField($"{mapDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            EditorGUI.BeginChangeCheck();
            var mapDataType = mapDataConfig != null ? mapDataConfig.DataType : MapDataType.PLAYER_SPAWN;
            var mapDataChoiceOptions = GetMapDataChoiceOptionsByType(mapDataType);
            var mapDataChoiceValues = GetMapDataChoiceValuesByType(mapDataType);
            uid = EditorGUILayout.IntPopup(uid, mapDataChoiceOptions, mapDataChoiceValues, GUILayout.Width(150f));
            if (EditorGUI.EndChangeCheck())
            {
                DoChangeMapDataUID(mapDataIndex, uid);
            }
            if (mapDataConfig != null)
            {
                EditorGUILayout.LabelField(mapDataConfig.DataType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.IntField(mapDataConfig.ConfId, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            }
            else
            {
                EditorGUILayout.LabelField("找不到对象类型数据", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
                EditorGUILayout.LabelField("找不到关联Id数据", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            }
            var newVector3Value = positionProperty.vector3Value;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("X", GUILayout.Width(10f));
            newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(39f));
            if(EditorGUI.EndChangeCheck())
            {
                var positionOffset = newVector3Value - positionProperty.vector3Value;
                OnMapDataPositionMove(mapDataIndex, positionOffset);
            }
            var rotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
            var newRotationVector3Value = rotationProperty.vector3Value;
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.LabelField("X", GUILayout.Width(10f));
            newRotationVector3Value.x = EditorGUILayout.FloatField(newRotationVector3Value.x, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newRotationVector3Value.y = EditorGUILayout.FloatField(newRotationVector3Value.y, GUILayout.Width(39f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newRotationVector3Value.z = EditorGUILayout.FloatField(newRotationVector3Value.z, GUILayout.Width(39f));
            if (EditorGUI.EndChangeCheck())
            {
                rotationProperty.vector3Value = newRotationVector3Value;
            }
            var des = mapDataConfig != null ? mapDataConfig.Des : "";
            EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            if (GUILayout.Button("↑", GUILayout.Width(40f)))
            {
                DoMovePropertyDataUpByIndex(mMapDataListProperty, mapDataIndex);
            }
            if (GUILayout.Button("↓", GUILayout.Width(40f)))
            {
                DoMovePropertyDataDownByIndex(mMapDataListProperty, mapDataIndex);
            }
            if (GUILayout.Button("+", GUILayout.Width(40f)))
            {
                var addMapDataValue = mAddMapDataIndexProperty.intValue;
                DoAddMapData(addMapDataValue, mapDataIndex);
            }
            if (GUILayout.Button("-", GUILayout.Width(40f)))
            {
                DoRemoveMapDataByIndex(mapDataIndex);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图埋点自定义数据区域
        /// </summary>
        private void DrawMapCustomDataArea()
        {
            // 自定义埋点数据暂时采用遍历所有埋点数据匹配类型的方式过滤显示需要的埋点数据，后去卡再想办法优化
            if(mMapCustomDataTypeList == null)
            {
                mMapCustomDataTypeList = mMapCustomDataAreaUnfoldMap.Keys.ToList();
            }
            foreach(var mapDataType in mMapCustomDataTypeList)
            {
                mMapCustomDataAreaUnfoldMap[mapDataType] = EditorGUILayout.Foldout(mMapCustomDataAreaUnfoldMap[mapDataType], $"地图埋点类型{mapDataType.ToString()}自定义数据列表");
                if(mMapCustomDataAreaUnfoldMap[mapDataType])
                {
                    if(mapDataType == MapDataType.MONSTER)
                    {
                        DrawMapMonsterCustomDataArea();
                    }
                    else if(mapDataType == MapDataType.MONSTER_GROUP)
                    {
                        DrawMapMonsterGroupCustomDataArea();
                    }
                }
            }
        }

        /// <summary>
        /// 绘制怪物埋点类型的自定义数据区域
        /// </summary>
        private void DrawMapMonsterCustomDataArea()
        {
            DrawMapMonsterDataTitleArea();
            for(int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                if(mapDataConfig.DataType != MapDataType.MONSTER)
                {
                    continue;
                }
                DrawOneMapMonsterDataPropertyIndex(i);
            }
        }

        /// <summary>
        /// 绘制怪物埋点自定义数据标题区域
        /// </summary>
        private void DrawMapMonsterDataTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("组Id", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定属性索引的单个怪物数据埋点属性显示
        /// </summary>
        /// <param name="index"></param>
        private void DrawOneMapMonsterDataPropertyIndex(int index)
        {
            var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(index);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            EditorGUILayout.LabelField($"{uid}", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            var groupIdProperty = mapDataProperty.FindPropertyRelative("GroupId");
            if(groupIdProperty != null)
            {
                groupIdProperty.intValue = EditorGUILayout.IntField(groupIdProperty.intValue, GUILayout.Width(60f));
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制怪物组埋点类型的自定义数据区域
        /// </summary>
        private void DrawMapMonsterGroupCustomDataArea()
        {
            DrawMapMonsterGroupDataTitleArea();
            for (int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                var uid = uidProperty.intValue;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
                if (mapDataConfig.DataType != MapDataType.MONSTER_GROUP)
                {
                    continue;
                }
                DrawOneMapMonsterGroupDataPropertyIndex(i);
            }
        }

        /// <summary>
        /// 绘制怪物组埋点自定义数据标题区域
        /// </summary>
        private void DrawMapMonsterGroupDataTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("组Id", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("怪物创建半径", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("怪物警戒半径", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("GUI关闭开关", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制指定属性索引的单个怪物组数据埋点属性显示
        /// </summary>
        /// <param name="index"></param>
        private void DrawOneMapMonsterGroupDataPropertyIndex(int index)
        {
            var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(index);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField($"{index}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            EditorGUILayout.LabelField($"{uid}", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            var groupIdProperty = mapDataProperty.FindPropertyRelative("GroupId");
            if (groupIdProperty != null)
            {
                groupIdProperty.intValue = EditorGUILayout.IntField(groupIdProperty.intValue, GUILayout.Width(60f));
            }
            var monsterCreateRadiusProperty = mapDataProperty.FindPropertyRelative("MonsterCreateRadius");
            if (monsterCreateRadiusProperty != null)
            {
                monsterCreateRadiusProperty.floatValue = EditorGUILayout.FloatField(monsterCreateRadiusProperty.floatValue, GUILayout.Width(100f));
            }
            var monsterActiveRadiusProperty = mapDataProperty.FindPropertyRelative("MonsterActiveRadius");
            if (monsterActiveRadiusProperty != null)
            {
                monsterActiveRadiusProperty.floatValue = EditorGUILayout.FloatField(monsterActiveRadiusProperty.floatValue, GUILayout.Width(100f));
            }
            var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative("GUISwitchOff");
            if (guiSwitchOffProperty != null)
            {
                EditorGUILayout.Space(30f, false);
                guiSwitchOffProperty.boolValue = EditorGUILayout.Toggle(guiSwitchOffProperty.boolValue, GUILayout.Width(70f));
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
                    if (mMapAreaGUISwitchProperty.boolValue)
                    {
                        DrawMapGridRects();
                    }
                    if (mMapObjectSceneGUISwitchProperty != null && mMapObjectSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapObjectDataLabels();
                    }
                    if(mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapDataLabels();
                        DrawMapDataSpheres();
                    }
                }

                if(currentEvent.type == EventType.KeyDown)
                {
                    if(currentEvent.keyCode == KeyCode.W)
                    {
                        MarkKeyDown(KeyCode.W);
                    }
                    if(currentEvent.keyCode == KeyCode.E)
                    {
                        MarkKeyDown(KeyCode.E);
                    }
                }
                if(currentEvent.type == EventType.KeyUp)
                {
                    if (currentEvent.keyCode == KeyCode.W)
                    {
                        MarkKeyUp(KeyCode.W);
                    }
                    if (currentEvent.keyCode == KeyCode.E)
                    {
                        MarkKeyUp(KeyCode.E);
                    }
                }
                if(IsKeyCodeDown(KeyCode.W))
                {
                    OnWKeyboardClick();
                }
                if(IsKeyCodeDown(KeyCode.E))
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
        /// 绘制九宫格Rect
        /// </summary>
        private void DrawMapGridRects()
        {
            var preHandlesColor = Handles.color;
            Handles.color = Color.yellow;
            var gridSize = mGridSizeProperty.floatValue;
            var gridVector3Size = new Vector3(gridSize, 0, gridSize);
            for(int i = 0; i < mGridDataList.Count; i++)
            {
                Handles.DrawWireCube(mGridDataList[i].Key, gridVector3Size);
                Handles.Label(mGridDataList[i].Key, $"区域{mGridDataList[i].Value}");
            }
            Handles.color = preHandlesColor;
        }

        /// <summary>
        /// 绘制地图对象数据标签
        /// </summary>
        private void DrawMapObjectDataLabels()
        {
            for(int i = 0; i < mMapObjectDataListProperty.arraySize; i++)
            {
                var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(i);
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
            for (int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                string mapDataLabelName = MapEditorUtilities.GetMapDataPropertyLabelName(mMapDataListProperty, i);
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataBatchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                var labelPos = mapDataPositionProperty.vector3Value + MapEditorConst.MapDAtaLabelPosOffset;
                var displayGUIStyle = mapDataBatchOperationSwitchProperty.boolValue ? mYellowLabelGUIStyle : mRedLabelGUIStyle;
                Handles.Label(labelPos, mapDataLabelName, displayGUIStyle);
            }
        }

        /// <summary>
        /// 绘制地图埋点球体
        /// </summary>
        private void DrawMapDataSpheres()
        {
            for (int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var uidProperty = mapDataProperty.FindPropertyRelative("UID");
                var mapDataUID = uidProperty.intValue;
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
                var preHandlesColor = Handles.color;
                Handles.color = mapDataConfig != null ? mapDataConfig.SceneSphereColor : Color.gray;
                Handles.SphereHandleCap(i, mapDataPositionProperty.vector3Value, rotationQuaternion, MapEditorConst.MapDataSphereSize, EventType.Repaint);
                Handles.color = preHandlesColor;
            }
        }

        /// <summary>
        /// 绘制所有地图埋点数据坐标操作PositionHandle
        /// </summary>
        private void DrawMapDataPositionHandles()
        {
            for (int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                EditorGUI.BeginChangeCheck();
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
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
            for (int i = 0; i < mMapDataListProperty.arraySize; i++)
            {
                EditorGUI.BeginChangeCheck();
                var mapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(i);
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
                var newTargetRotation = Handles.RotationHandle(rotationQuaternion, mapDataPositionProperty.vector3Value);
                if (EditorGUI.EndChangeCheck())
                {
                    mapDataRotationProperty.vector3Value = newTargetRotation.eulerAngles;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
