/*
 * Description:             MapEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.AI.Navigation;
using UnityEditor;
using UnityEngine;

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
        /// MapObjectSceneGUISwitch属性
        /// </summary>
        private SerializedProperty mMapObjectSceneGUISwitchProperty;

        /// <summary>
        /// SceneGUISwitch属性
        /// </summary>
        private SerializedProperty mMapDataSceneGUISwitchProperty;

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
        /// AddMapObjectValue属性
        /// </summary>
        private SerializedProperty mAddMapObjectIndexProperty;

        /// <summary>
        /// AddMapDataValue属性
        /// </summary>
        private SerializedProperty mAddMapDataIndexProperty;

        /// <summary>
        /// ExportType属性
        /// </summary>
        private SerializedProperty mExportTypeProperty;

        /// <summary>
        /// Label显示GUIStyle
        /// </summary>
        private GUIStyle mLabelGUIStyle;

        /// <summary>
        /// 横向绘制线条数据列表<起点, 终点>列表
        /// </summary>
        private List<KeyValuePair<Vector3, Vector3>> mHDrawLinesDataList = new List<KeyValuePair<Vector3, Vector3>>();

        /// <summary>
        /// 纵向绘制线条数据列表<起点, 终点>列表
        /// </summary>
        private List<KeyValuePair<Vector3, Vector3>> mVDrawLinesDataList = new List<KeyValuePair<Vector3, Vector3>>();

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
        /// 地图对象选项数组(显示名字数组)
        /// </summary>
        private string[] mMapObjectDataChoiceOptions;

        /// <summary>
        /// 地图对象选项值数组(UID数组)
        /// </summary>
        private int[] mMapObjectDataChoiceValues;

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
        /// 地图埋点选项数组(显示名字数组)
        /// </summary>
        private string[] mMapDataChoiceOptions;

        /// <summary>
        /// 地图埋点选项值数组(UID数组)
        /// </summary>
        private int[] mMapDataChoiceValues;

        private void Awake()
        {
            InitTarget();
            InitProperties();
            InitGUIStyles();
            mMapWidthProperty.intValue = mMapWidthProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapWidth : mMapWidthProperty.intValue;
            mMapHeightProperty.intValue = mMapHeightProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapHeight : mMapHeightProperty.intValue;
            CreateAllNodes();
            UpdateMapObjectDataChoiceDatas();
            UpdateMapDataChoiceDatas();
            UpdateAddMapObjectDataPreviewAsset();
            UpdateMapSizeDrawDatas();
            UpdateMapGOPosition();
            UpdateTerrianSizeAndPos();
            UpdateMapObjectDataPositionDatas();
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
            mMapObjectSceneGUISwitchProperty ??= serializedObject.FindProperty("MapObjectSceneGUISwitch");
            mMapDataSceneGUISwitchProperty ??= serializedObject.FindProperty("MapDataSceneGUISwitch");
            mMapWidthProperty ??= serializedObject.FindProperty("MapWidth");
            mMapHeightProperty ??= serializedObject.FindProperty("MapHeight");
            mMapStartPosProperty ??= serializedObject.FindProperty("MapStartPos");
            mMapTerrianAssetProperty ??= serializedObject.FindProperty("MapTerrianAsset");
            mMapObjectDataListProperty ??= serializedObject.FindProperty("MapObjectDataList");
            mMapDataListProperty ??= serializedObject.FindProperty("MapDataList");
            mAddMapObjectIndexProperty ??= serializedObject.FindProperty("AddMapObjectIndex");
            mAddMapDataIndexProperty ??= serializedObject.FindProperty("AddMapDataIndex");
            mExportTypeProperty ??= serializedObject.FindProperty("ExportType");
        }

        /// <summary>
        /// 初始化GUIStyles
        /// </summary>
        private void InitGUIStyles()
        {
            if(mLabelGUIStyle == null)
            {
                mLabelGUIStyle = new GUIStyle();
                mLabelGUIStyle.fontSize = 15;
                mLabelGUIStyle.alignment = TextAnchor.MiddleCenter;
                mLabelGUIStyle.normal.textColor = Color.red;
            }
        }

        /// <summary>
        /// 创建所有节点
        /// </summary>
        private void CreateAllNodes()
        {
            CreateAllMapObjectParentNodes();
            CreateMapTerrianNode();
            CreateNavMeshSurface();
        }

        /// <summary>
        /// 创建所有地图对象父节点挂点
        /// </summary>
        private void CreateAllMapObjectParentNodes()
        {
            var mapObjectParentNode = MapEditorUtilities.GetOrCreateMapObjectParentNode(mTarget.gameObject);
            if(mapObjectParentNode != null)
            {
                mapObjectParentNode.transform.localPosition = Vector3.zero;
            }

            var mapObjectTypeValues = Enum.GetValues(MapConst.MapObjectType);
            foreach(var mapObjectTypeValue in mapObjectTypeValues)
            {
                var mapObjectType = (MapObjectType)mapObjectTypeValue;
                var mapObjectTypeParentNodeTransform = MapEditorUtilities.GetOrCreateMapObjectTypeParentNode(mTarget.gameObject, mapObjectType);
                mapObjectTypeParentNodeTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 创建地图地形节点
        /// </summary>
        private void CreateMapTerrianNode()
        {
            var customAsset = mMapTerrianAssetProperty.objectReferenceValue as GameObject;
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget.gameObject, customAsset);
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
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget.gameObject);
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
            MapEditorUtilities.GetOrCreateNavMeshSurface(mTarget.gameObject);
        }

        /// <summary>
        /// 更新地图地块大小和位置
        /// </summary>
        private void UpdateTerrianSizeAndPos()
        {
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapTerrianNode(mTarget.gameObject);
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
        /// 更新地图对象数据的位置数据(对象存在时才更新记录位置)
        /// </summary>
        private void UpdateMapObjectDataPositionDatas()
        {
            // 动态地图对象可能删除还原，所以需要逻辑层面记录数据
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
                    var mapObjectGO = goProperty.objectReferenceValue as GameObject;
                    positionProperty.vector3Value = mapObjectGO.transform.position;
                    if(mapObjectConfig.IsDynamic)
                    {
                        var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                        var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                        var colliderRadiusProperty = mapObjectDataProperty.FindPropertyRelative("ColliderRadius");
                        MapUtilities.UpdateColliderByColliderData(mapObjectGO, colliderCenterProperty.vector3Value,
                                                                    colliderSizeProperty.vector3Value, colliderRadiusProperty.floatValue);
                    }
                }
            }
        }

        /// <summary>
        /// 矫正添加地图对象索引值(避免因为配置面板修改导致索引超出有效范围问题)
        /// </summary>
        private void CorrectAddMapObjectIndexValue()
        {
            var totalOptionNum = mMapObjectDataChoiceOptions.Length;
            if(mAddMapObjectIndexProperty.intValue > totalOptionNum)
            {
                mAddMapObjectIndexProperty.intValue = Math.Clamp(mAddMapObjectIndexProperty.intValue, 0, totalOptionNum);
            }
        }

        /// <summary>
        /// 矫正添加地图埋点索引值(避免因为配置面板修改导致索引超出有效范围问题)
        /// </summary>
        private void CorrectAddMapDataIndexValue()
        {
            var totalOptionNum = mMapDataChoiceOptions.Length;
            if (mAddMapDataIndexProperty.intValue > totalOptionNum)
            {
                mAddMapDataIndexProperty.intValue = Math.Clamp(mAddMapDataIndexProperty.intValue, 0, totalOptionNum);
            }
        }

        /// <summary>
        /// 更新地图对象选择数据
        /// </summary>
        private void UpdateMapObjectDataChoiceDatas()
        {
            var allMapObjectConfigs = MapSetting.GetEditorInstance().ObjectSetting.AllMapObjectConfigs;
            var totalMapObjectConfigNum = allMapObjectConfigs.Count;
            if (mMapObjectDataChoiceOptions == null || mMapObjectDataChoiceOptions.Length != totalMapObjectConfigNum)
            {
                mMapObjectDataChoiceOptions = new string[totalMapObjectConfigNum];
            }
            if (mMapObjectDataChoiceValues == null || mMapObjectDataChoiceValues.Length != totalMapObjectConfigNum)
            {
                mMapObjectDataChoiceValues = new int[totalMapObjectConfigNum];
            }
            for(int i = 0, length = totalMapObjectConfigNum; i < length; i++)
            {
                mMapObjectDataChoiceOptions[i] = allMapObjectConfigs[i].GetOptionName();
                mMapObjectDataChoiceValues[i] = allMapObjectConfigs[i].UID;
            }
        }

        /// <summary>
        /// 更新地图埋点选择数据
        /// </summary>
        private void UpdateMapDataChoiceDatas()
        {
            var allMapDataConfigs = MapSetting.GetEditorInstance().DataSetting.AlllMapDataConfigs;
            var totalMapDataConfigNum = allMapDataConfigs.Count;
            if (mMapDataChoiceOptions == null || mMapDataChoiceOptions.Length != totalMapDataConfigNum)
            {
                mMapDataChoiceOptions = new string[totalMapDataConfigNum];
            }
            if (mMapDataChoiceValues == null || mMapDataChoiceValues.Length != totalMapDataConfigNum)
            {
                mMapDataChoiceValues = new int[totalMapDataConfigNum];
            }
            for (int i = 0, length = totalMapDataConfigNum; i < length; i++)
            {
                mMapDataChoiceOptions[i] = allMapDataConfigs[i].GetOptionName();
                mMapDataChoiceValues[i] = allMapDataConfigs[i].UID;
            }
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
        /// 创建之地给你地图对象UID配置的实体对象(未配置Asset返回null)
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        private GameObject CreateGameObjectByUID(int uid)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if(mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持创建地图实体对象！");
                return null;
            }
            var instanceGo = mapObjectConfig.Asset != null ? GameObject.Instantiate(mapObjectConfig.Asset) : null;
            if(instanceGo != null)
            {
                var mapObjectType = mapObjectConfig.ObjectType;
                var parentNodeTransform = MapEditorUtilities.GetOrCreateMapObjectTypeParentNode(mTarget.gameObject, mapObjectType);
                instanceGo.transform.SetParent(parentNodeTransform);
                instanceGo.transform.position = mMapStartPosProperty.vector3Value;
                var instanceGoName = instanceGo.name.RemoveSubStr("(Clone)");
                instanceGoName = $"{instanceGoName}_{uid}";
                instanceGo.name = instanceGoName;
                MapEditorUtilities.AddOrUpdateMapObjectDataMono(instanceGo, uid);
            }
            return instanceGo;
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
        /// 添加指定地图对象UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private bool AddMapObjectData(int uid, int insertIndex = -1)
        {
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
            if(mapObjectConfig == null)
            {
                Debug.LogError($"未配置地图对象UID:{uid}配置数据，不支持添加此地图对象数据！");
                return false;
            }
            var mapObjectDataTotalNum = mMapObjectDataListProperty.arraySize;
            var maxInsertIndex = mapObjectDataTotalNum == 0 ? 0 : mapObjectDataTotalNum;
            var insertPos = 0;
            if(insertIndex == -1)
            {
                insertPos = maxInsertIndex;
            }
            else
            {
                insertPos = Math.Clamp(insertIndex, 0, maxInsertIndex);
            }
            var mapObjectPosition = mMapStartPosProperty.vector3Value;
            if(mapObjectDataTotalNum != 0)
            {
                var insertMapObjectPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapObjectProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertMapObjectPos);
                var insertMapObjectData = insertMapObjectProperty.managedReferenceValue as MapObjectData;
                mapObjectPosition = insertMapObjectData.Go != null ? insertMapObjectData.Go.transform.position : insertMapObjectData.Position;
            }
            var instanceGo = CreateGameObjectByUID(uid);
            if(instanceGo != null)
            {
                Selection.SetActiveObjectWithContext(instanceGo, instanceGo);
            }
            instanceGo.transform.position = mapObjectPosition;
            MapUtilities.UpdateColliderByColliderDataMono(instanceGo);
            var newMapObjectData = new MapObjectData(uid, instanceGo);
            mMapObjectDataListProperty.InsertArrayElementAtIndex(insertPos);
            var newMapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertPos);
            newMapObjectDataProperty.managedReferenceValue = newMapObjectData;
            serializedObject.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// 移除指定索引的地图对象数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemoveMapObjectDataByIndex(int index)
        {
            var mapObjectDataNum = mMapObjectDataListProperty.arraySize;
            if(index < 0 || index >= mapObjectDataNum)
            {
                Debug.LogError($"指定索引:{index}不是有效索引范围:0-{mapObjectDataNum - 1},移除地图对象数据失败！");
                return false;
            }
            var mapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(index);
            var goProperty = mapObjectDataProperty.FindPropertyRelative("Go");
            if(goProperty.objectReferenceValue != null)
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
        /// 添加指定地图埋点UID数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="insertIndex"></param>
        /// <returns></returns>
        private bool AddMapData(int uid, int insertIndex = -1)
        {
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (mapDataConfig == null)
            {
                Debug.LogError($"未配置地图埋点UID:{uid}配置数据，不支持添加此地图埋点数据！");
                return false;
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
            if(mapDataTotalNum != 0)
            {
                var insertMapDataPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(insertMapDataPos);
                var insertMapData = insertMapDataProperty.managedReferenceValue as MapData;
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
            }
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition);
            var newMapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertPos);
            newMapObjectDataProperty.managedReferenceValue = newMapData;
            serializedObject.ApplyModifiedProperties();
            return true;
        }

        /// <summary>
        /// 移除指定索引的地图埋点数据
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool RemoveMapDataByIndex(int index)
        {
            var mapDataNum = mMapObjectDataListProperty.arraySize;
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
        /// 清除动态地图对象GameObjects
        /// </summary>
        private void CleanDynamicMaoObjectGos()
        {
            UpdateMapObjectDataPositionDatas();
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
                if(mapObjectConfig.IsDynamic && goProperty.objectReferenceValue != null)
                {
                    var go = goProperty.objectReferenceValue as GameObject;
                    GameObject.DestroyImmediate(go);
                    goProperty.objectReferenceValue = null;
                }
            }
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 回复动态地图对象GameObjects
        /// </summary>
        private void RecoverDynamicMapObjectGos()
        {
            UpdateMapObjectDataPositionDatas();
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
                    var instanceGo = CreateGameObjectByUID(mapObjectUID);
                    if(instanceGo != null)
                    {
                        var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                        var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                        var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                        var colliderRadiusProperty = mapObjectDataProperty.FindPropertyRelative("ColliderRadius");
                        instanceGo.transform.position = positionProperty.vector3Value;
                        goProperty.objectReferenceValue = instanceGo;
                        MapUtilities.UpdateColliderByColliderData(instanceGo, colliderCenterProperty.vector3Value, colliderSizeProperty.vector3Value, colliderRadiusProperty.floatValue);
                    }
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
                Debug.LogError($"找不到寻路NavMeshSurface组件，烘焙和拷贝寻路数据Asset失败！");
                return;
            }
            var targetAssetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(mTarget);
            if(string.IsNullOrEmpty(targetAssetPath))
            {
                Debug.LogError($"当前对象:{mTarget.name}未保存成任何本地Asset，复制寻路数据Asset失败！");
                return;
            }
            var navMeshAssetPath = AssetDatabase.GetAssetPath(navMeshSurface.navMeshData);
            if(navMeshSurface.navMeshData == null || string.IsNullOrEmpty(navMeshAssetPath))
            {
                Debug.LogError($"未烘焙任何有效寻路数据Asset，复制寻路数据Asset失败！");
                return;
            }
            var targetAssetFolderPath = Path.GetDirectoryName(targetAssetPath);
            var navMeshAssetName = Path.GetFileName(navMeshAssetPath);
            var newNavMeshAssetPath = Path.Combine(targetAssetFolderPath, navMeshAssetName);
            if(!string.Equals(navMeshAssetPath, newNavMeshAssetPath))
            {
                AssetDatabase.DeleteAsset(newNavMeshAssetPath);
            }
            AssetDatabase.MoveAsset(navMeshAssetPath, newNavMeshAssetPath);
            Debug.Log($"移动寻路数据Asset:{navMeshAssetPath}到{newNavMeshAssetPath}成功！");
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
                MapEditorUtilities.AddOrUpdateMapObjectDataMono(go, uidProperty.intValue);
            }
        }

        /// <summary>
        /// 导出地图数据
        /// </summary>
        private void ExportMapData()
        {
            if(!MapEditorUtilities.CheckIsGameMapAvalibleExport(mTarget))
            {
                Debug.LogError($"场景数据有问题，不满足导出条件，导出场景数据失败！");
                return;
            }
            // 流程上说场景给客户端使用一定会经历导出流程
            // 在导出时确保MapObjectDataMono和地图对象配置数据一致
            // 从而确保场景资源被使用时挂在数据和配置匹配
            UpdateAllMapObjectDataMonos();
            // 确保所有数据运用到最新
            serializedObject.ApplyModifiedProperties();
            var isPrefabAssetInstance = PrefabUtility.IsPartOfPrefabInstance(mTarget.gameObject);
            // 确保数据应用到对应Asset上
            if(isPrefabAssetInstance)
            {
                PrefabUtility.ApplyPrefabInstance(mTarget.gameObject, InteractionMode.AutomatedAction);
            }
            MapEditorUtilities.ExportGameMapData(mTarget);
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
            EditorGUILayout.PropertyField(mMapObjectSceneGUISwitchProperty);
            EditorGUILayout.PropertyField(mMapDataSceneGUISwitchProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mMapWidthProperty);
            EditorGUILayout.PropertyField(mMapHeightProperty);
            if(EditorGUI.EndChangeCheck())
            {
                UpdateMapSizeDrawDatas();
                UpdateMapGOPosition();
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
            if(GUILayout.Button("清除动态对象显示", GUILayout.ExpandWidth(true)))
            {
                CleanDynamicMaoObjectGos();
            }
            if(GUILayout.Button("恢复动态对象显示", GUILayout.ExpandWidth(true)))
            {
                RecoverDynamicMapObjectGos();
            }
            if(GUILayout.Button("拷贝NavMesh Asset", GUILayout.ExpandWidth(true)))
            {
                CopyNavMeshAsset();
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
            if(mMapObjectDataChoiceOptions.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("地图对象选择:", GUILayout.Width(100f));
                EditorGUI.BeginChangeCheck();
                mAddMapObjectIndexProperty.intValue = EditorGUILayout.IntPopup(mAddMapObjectIndexProperty.intValue, mMapObjectDataChoiceOptions, mMapObjectDataChoiceValues, GUILayout.ExpandWidth(true));
                if(EditorGUI.EndChangeCheck())
                {
                    UpdateAddMapObjectDataPreviewAsset();
                }
                if(GUILayout.Button("+", GUILayout.Width(40f)))
                {
                    var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
                    AddMapObjectData(addMapObjectValue);
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
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("是否动态", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("实体对象", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
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
            EditorGUILayout.LabelField($"{uid}", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(uid);
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
            EditorGUILayout.ObjectField(goProperty.objectReferenceValue, MapConst.GameObjectType, true, GUILayout.Width(100f));
            var des = mapObjectConfig != null ? mapObjectConfig.Des : "";
            EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            if (GUILayout.Button("↑", GUILayout.Width(40f)))
            {
                MovePropertyDataUpByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("↓", GUILayout.Width(40f)))
            {
                MovePropertyDataDownByIndex(mMapObjectDataListProperty, mapObjectDataIndex);
            }
            if (GUILayout.Button("+", GUILayout.Width(40f)))
            {
                var addMapObjectValue = mAddMapObjectIndexProperty.intValue;
                AddMapObjectData(addMapObjectValue, mapObjectDataIndex);
            }
            if (GUILayout.Button("-", GUILayout.Width(40f)))
            {
                RemoveMapObjectDataByIndex(mapObjectDataIndex);
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
            if (mMapDataChoiceOptions.Length > 0)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("地图埋点选择:", GUILayout.Width(100f));
                mAddMapDataIndexProperty.intValue = EditorGUILayout.IntPopup(mAddMapDataIndexProperty.intValue, mMapDataChoiceOptions, mMapDataChoiceValues, GUILayout.ExpandWidth(true));
                if (GUILayout.Button("+", GUILayout.Width(40f)))
                {
                    var addMapDataValue = mAddMapDataIndexProperty.intValue;
                    AddMapData(addMapDataValue);
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
            EditorGUILayout.LabelField("索引", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            EditorGUILayout.LabelField("埋点类型", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            EditorGUILayout.LabelField("配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            EditorGUILayout.LabelField("位置", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
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
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            EditorGUILayout.LabelField($"{mapDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            EditorGUILayout.LabelField($"{uid}", MapStyles.TabMiddleStyle, GUILayout.Width(60f));
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
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
            newVector3Value.x = EditorGUILayout.FloatField(newVector3Value.x, GUILayout.Width(40f));
            EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
            newVector3Value.y = EditorGUILayout.FloatField(newVector3Value.y, GUILayout.Width(40f));
            EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
            newVector3Value.z = EditorGUILayout.FloatField(newVector3Value.z, GUILayout.Width(40f));
            if(EditorGUI.EndChangeCheck())
            {
                positionProperty.vector3Value = newVector3Value;
            }
            var des = mapDataConfig != null ? mapDataConfig.Des : "";
            EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
            if (GUILayout.Button("↑", GUILayout.Width(40f)))
            {
                MovePropertyDataUpByIndex(mMapDataListProperty, mapDataIndex);
            }
            if (GUILayout.Button("↓", GUILayout.Width(40f)))
            {
                MovePropertyDataDownByIndex(mMapDataListProperty, mapDataIndex);
            }
            if (GUILayout.Button("+", GUILayout.Width(40f)))
            {
                var addMapDataValue = mAddMapDataIndexProperty.intValue;
                AddMapData(addMapDataValue, mapDataIndex);
            }
            if (GUILayout.Button("-", GUILayout.Width(40f)))
            {
                RemoveMapDataByIndex(mapDataIndex);
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
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUI()
        {
            if(mSceneGUISwitchProperty != null && mSceneGUISwitchProperty.boolValue)
            {
                if(Event.current.type == EventType.Repaint)
                {
                    DrawMapLines();
                    if(mMapObjectSceneGUISwitchProperty != null && mMapObjectSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapObjectDataLabels();
                    }
                    if(mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
                    {
                        DrawMapDataLabels();
                        DrawMapDataSpheres();
                    }
                }
                if(mMapDataSceneGUISwitchProperty != null && mMapDataSceneGUISwitchProperty.boolValue)
                {
                    DrawMapDataPositionHandles();
                }
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
                Handles.Label(labelPos, mapObjectLabelName, mLabelGUIStyle);
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
                var labelPos = mapDataPositionProperty.vector3Value + MapEditorConst.MapDAtaLabelPosOffset;
                Handles.Label(labelPos, mapDataLabelName, mLabelGUIStyle);
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
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
                var preHandlesColor = Handles.color;
                Handles.color = mapDataConfig != null ? mapDataConfig.SceneSphereColor : Color.gray;
                Handles.SphereHandleCap(i, mapDataPositionProperty.vector3Value, Quaternion.identity, MapEditorConst.MapDataSphereSize, EventType.Repaint);
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
                var newTargetPosition = Handles.PositionHandle(mapDataPositionProperty.vector3Value, Quaternion.identity);
                if(EditorGUI.EndChangeCheck())
                {
                    mapDataPositionProperty.vector3Value = newTargetPosition;
                    serializedObject.ApplyModifiedProperties();
                }
            }
        }
    }
}
