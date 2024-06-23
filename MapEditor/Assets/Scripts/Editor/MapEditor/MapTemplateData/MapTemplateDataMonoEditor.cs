/*
 * Description:             MapTemplateDataMonoEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/22
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapTemplateDataMonoEditor.cs
    /// 地图锚点模版数据编辑Editor
    /// </summary>
    [CustomEditor(typeof(MapTemplateDataMono))]
    public class MapTemplateDataMonoEditor : Editor
    {
        /// <summary>
        /// 目标组件
        /// </summary>
        private Map mTarget;

        /// <summary>
        /// MapDataList属性
        /// </summary>
        private SerializedProperty mTemplateDataProperty;

        /// <summary>
        /// MapDataList属性
        /// </summary>
        private SerializedProperty mMapDataListProperty;

        /// <summary>
        /// SceneGUISwitch属性
        /// </summary>
        private SerializedProperty mSceneGUISwitchProperty;

        /// <summary>
        /// mAddMapDataType属性
        /// </summary>
        private SerializedProperty mAddMapDataTypeProperty;

        /// <summary>
        /// AddMapDataValue属性
        /// </summary>
        private SerializedProperty mAddMapDataIndexProperty;

        /// <summary>
        /// MapDataUnfoldData属性
        /// </summary>
        private SerializedProperty mMapDataUnfoldDataProperty;

        /// <summary>
        /// PlayerSpawnMapGroupUnfoldDataList属性
        /// </summary>
        private SerializedProperty mPlayerSpawnMapGroupUnfoldDataListProperty;

        /// <summary>
        /// MonsterMapGroupUnfoldDataList属性
        /// </summary>
        private SerializedProperty mMonsterMapGroupUnfoldDataListProperty;

        /// <summary>
        /// MonsterGroupMapGroupUnfoldDataList属性
        /// </summary>
        private SerializedProperty mMonsterGroupMapGroupUnfoldDataListProperty;

        /// <summary>
        /// BatchTickRangeStartIndex属性
        /// </summary>
        private SerializedProperty mBatchTickRangeStartIndexProperty;

        /// <summary>
        /// BatchTickRangeEndIndex属性
        /// </summary>
        private SerializedProperty mBatchTickRangeEndIndexProperty;

        /// <summary>
        /// 红色Label显示GUIStyle
        /// </summary>
        private GUIStyle mRedLabelGUIStyle;

        /// <summary>
        /// 黄色Label显示GUIStyle
        /// </summary>
        private GUIStyle mYellowLabelGUIStyle;

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
            UpdateMapDataTypeIndexDatas();
            UpdateMapDataChoiceDatas();
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
            mTemplateDataProperty ??= serializedObject.FindProperty("TemplateData");
            mMapDataListProperty ??= serializedObject.FindProperty("MapDataList");
            mAddMapDataTypeProperty ??= serializedObject.FindProperty("AddMapDataType");
            mAddMapDataIndexProperty ??= serializedObject.FindProperty("AddMapDataIndex");
            mMapDataUnfoldDataProperty ??= serializedObject.FindProperty("MapDataUnfoldData");
            mPlayerSpawnMapGroupUnfoldDataListProperty ??= serializedObject.FindProperty("PlayerSpawnMapGroupUnfoldDataList");
            mMonsterMapGroupUnfoldDataListProperty ??= serializedObject.FindProperty("MonsterMapGroupUnfoldDataList");
            mMonsterGroupMapGroupUnfoldDataListProperty ??= serializedObject.FindProperty("MonsterGroupMapGroupUnfoldDataList");
            mBatchTickRangeStartIndexProperty ??= serializedObject.FindProperty("BatchTickRangeStartIndex");
            mBatchTickRangeEndIndexProperty ??= serializedObject.FindProperty("BatchTickRangeEndIndex");
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
        /// 执行将指定属性对象和指定索引的数据向上移动
        /// </summary>
        /// <param name="propertyList"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool DoMovePropertyDataUpByIndex(SerializedProperty propertyList, int index)
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
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
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
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
            var mapDataPosition = mTarget.gameObject.transform.position;
            if (mapDataTotalNum != 0)
            {
                var insertMapDataPos = Math.Clamp(insertPos, 0, maxInsertIndex - 1);
                var insertMapDataProperty = GetMapDataSerializedPropertyByIndex(insertMapDataPos);
                var insertMapData = insertMapDataProperty.managedReferenceValue as MapData;
                mapDataPosition = insertMapData != null ? insertMapData.Position : mapDataPosition;
            }
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition, mapDataConfig.Rotation);
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
            var mapTemplateData = mTemplateDataProperty.managedReferenceValue as MapTemplateData;
            if (mapTemplateData == null)
            {
                Debug.LogError($"不允许给空地图埋点模版数据移除地图埋点数据，移除地图模版埋点数据失败！");
                return false;
            }
            var mapDataList = mapTemplateData.MapDataList;
            return MapUtilities.RemoveMapDataFromListByIndex(mapDataList, index);
        }

        /// <summary>
        /// 标记指定按键按下
        /// </summary>
        /// <param name="keyCode"></param>
        private void MarkKeyDown(KeyCode keyCode)
        {
            if (!mKeyCodeDownMap.ContainsKey(keyCode))
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
            if (!mKeyCodeDownMap.ContainsKey(keyCode))
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
            if (mKeyCodeDownMap.Count == 0)
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
            if (mKeyCodeDownMap.TryGetValue(keyCode, out result))
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
        /// 一键操作地图埋点批量勾选
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchMapDataBatchOperation(bool isOn)
        {
            UpdateAllMapDataBatchOperation(isOn);
        }

        /// <summary>
        /// 一键勾选指定范围的地图埋点批量数据
        /// </summary>
        /// <param name="isOn"></param>
        private void OneKeySwitchMapDataRangeOperation(bool isOn)
        {
            var mapTemplateData = mTemplateDataProperty.managedReferenceValue as MapTemplateData;
            if (mapTemplateData == null)
            {
                Debug.LogError($"空地图模版数据对象，更新地图埋点批量操作数据失败！");
                return;
            }
            var mapDataList = mapTemplateData.MapDataList;
            var totalMapDataNum = mapDataList.Count;
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
                mapDataList[i].BatchOperationSwitch = isOn;
            }
        }

        /// <summary>
        /// 更新所有地图埋点批量选择
        /// </summary>
        /// <param name="isOn"></param>
        private void UpdateAllMapDataBatchOperation(bool isOn)
        {
            var mapTemplateData = mTemplateDataProperty.managedReferenceValue as MapTemplateData;
            if (mapTemplateData == null)
            {
                Debug.LogError($"空地图模版数据对象，更新地图埋点批量操作数据失败！");
                return;
            }
            MapUtilities.UpdateAllMapDataBatchOperationByList(mapTemplateData.MapDataList, isOn);
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
        /// 响应添加地图埋点索引选择变化
        /// </summary>
        private void OnAddMapDataIndexChange()
        {

        }

        /// <summary>
        /// 响应埋点模版数据变化
        /// </summary>
        private void OnTemplateDataChange()
        {

        }

        /// <summary>
        /// 执行指定地图埋点索引的UID变化
        /// </summary>
        /// <param name="mapDataIndex"></param>
        /// <param name="newUID"></param>
        private void DoChangeMapDataUID(int mapDataIndex, int newUID)
        {
            if (!MapUtilities.CheckOperationAvalible(mTarget?.gameObject))
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
                DoMapDataBatchUIDCHangeExcept(mapDataIndex, oldUID, newUID);
            }
        }

        /// <summary>
        /// 执行地图埋点数据UID批量变化(排除指定地图数据索引)
        /// Note:
        /// 只允许批量修改相同埋点类型的UID数据
        /// </summary>
        /// <param name="mapDataIndex"></param>
        private bool DoMapDataBatchUIDCHangeExcept(int mapDataIndex, int oldUID, int newUID)
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
        /// 获取指定地图折叠类型的展开数据列表属性
        /// </summary>
        /// <param name="mapFoldType"></param>
        /// <returns></returns>
        private SerializedProperty GetMapUnfoldDataListProperty(MapFoldType mapFoldType)
        {
            if (mapFoldType == MapFoldType.PlayerSpawnMapDataFold)
            {
                return mPlayerSpawnMapGroupUnfoldDataListProperty;
            }
            else if (mapFoldType == MapFoldType.MonsterMapDataFold)
            {
                return mMonsterMapGroupUnfoldDataListProperty;
            }
            else if (mapFoldType == MapFoldType.MonsterGroupMapDataFold)
            {
                return mMonsterGroupMapGroupUnfoldDataListProperty;
            }
            else
            {
                Debug.LogError($"不支持的地图折叠类型:{mapFoldType}，获取展开数据列表属性失败！");
                return null;
            }
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
        /// 保存埋点模版数据
        /// </summary>
        private void SaveMapTeplateData()
        {

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

            // 确保对SerializedObjec和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mSceneGUISwitchProperty);

            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(mTemplateDataProperty);
            if(EditorGUI.EndChangeCheck())
            {
                OnTemplateDataChange();
            }

            DrawMapTemplateDataButtonArea();
            DrawMapOperationInspectorArea();

            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 绘制埋点模版数据按钮区域
        /// </summary>
        private void DrawMapTemplateDataButtonArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存模版数据", GUILayout.ExpandWidth(true)))
            {
                SaveMapTeplateData();
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
            DrawMapDataInspectorArea();
            EditorGUILayout.EndVertical();
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
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("一键勾选批量", GUILayout.ExpandWidth(true)))
                {
                    OneKeySwitchMapDataBatchOperation(true);
                }
                if (GUILayout.Button("一键清除批量勾选", GUILayout.ExpandWidth(true)))
                {
                    OneKeySwitchMapDataBatchOperation(false);
                }
                EditorGUILayout.EndHorizontal();
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("开始索引:", GUILayout.Width(60f));
                mBatchTickRangeStartIndexProperty.intValue = EditorGUILayout.IntField(mBatchTickRangeStartIndexProperty.intValue, GUILayout.Width(100f));
                EditorGUILayout.LabelField("结束索引:", GUILayout.Width(60f));
                mBatchTickRangeEndIndexProperty.intValue = EditorGUILayout.IntField(mBatchTickRangeEndIndexProperty.intValue, GUILayout.Width(100f));
                if (GUILayout.Button("范围勾选", GUILayout.ExpandWidth(true)))
                {
                    OneKeySwitchMapDataRangeOperation(true);
                }
                if (GUILayout.Button("范围清除勾选", GUILayout.ExpandWidth(true)))
                {
                    OneKeySwitchMapDataRangeOperation(false);
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
                DrawMapDataAreaByType(MapDataType.PlayerSpawn);
                DrawMapDataAreaByType(MapDataType.Monster);
                DrawMapDataAreaByType(MapDataType.MonsterGroup);
            }
        }

        /// <summary>
        /// 绘制指定地图埋点类型数据区域
        /// </summary>
        /// <param name="mapDataType"></param>
        private void DrawMapDataAreaByType(MapDataType mapDataType)
        {
            if (!MapEditorUtilities.IsShowMapDataFoldType(mapDataType))
            {
                return;
            }
            var mapFoldType = MapEditorUtilities.GetMapDataFoldType(mapDataType);
            DrawMapDataOneKeyFoldAreaByType(mapFoldType);
            MapEditorUtilities.DrawMapDataTitleAreaByType(mapDataType);
            DrawMapDataDetailByType(mapDataType);
        }

        /// <summary>
        /// 绘制指定地图埋点类型的一键折叠区域
        /// </summary>
        /// <param name="mapFoldType"></param>
        private void DrawMapDataOneKeyFoldAreaByType(MapFoldType mapFoldType)
        {
            EditorGUILayout.BeginHorizontal("box");
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
                if (mapDataConfig.DataType != mapDataType)
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
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Batch))
            {
                var batchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                EditorGUILayout.Space(10f, false);
                batchOperationSwitchProperty.boolValue = EditorGUILayout.Toggle(batchOperationSwitchProperty.boolValue, GUILayout.Width(30f));
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Index))
            {
                EditorGUILayout.LabelField($"{mapDataIndex}", MapStyles.TabMiddleStyle, GUILayout.Width(40f));
            }
            MapDataConfig mapDataConfig = null;
            var uidProperty = mapDataProperty.FindPropertyRelative("UID");
            var uid = uidProperty.intValue;
            var positionProperty = mapDataProperty.FindPropertyRelative("Position");
            mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(uid);
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.UID))
            {
                EditorGUI.BeginChangeCheck();
                var realMapDataType = mapDataConfig != null ? mapDataConfig.DataType : MapDataType.PlayerSpawn;
                var mapDataChoiceOptions = GetMapDataChoiceOptionsByType(realMapDataType);
                var mapDataChoiceValues = GetMapDataChoiceValuesByType(realMapDataType);
                uid = EditorGUILayout.IntPopup(uid, mapDataChoiceOptions, mapDataChoiceValues, GUILayout.Width(150f));
                if (EditorGUI.EndChangeCheck())
                {
                    DoChangeMapDataUID(mapDataIndex, uid);
                }
            }
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MapDataType))
            //{
            //    if (mapDataConfig != null)
            //    {
            //        EditorGUILayout.LabelField(mapDataConfig.DataType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            //    }
            //    else
            //    {
            //        EditorGUILayout.LabelField("找不到对象类型数据", MapStyles.TabMiddleStyle, GUILayout.Width(150f));
            //    }
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.ConfId))
            {
                if (mapDataConfig != null)
                {
                    EditorGUILayout.IntField(mapDataConfig.ConfId, MapStyles.TabMiddleStyle, GUILayout.Width(100f));
                }
                else
                {
                    EditorGUILayout.LabelField("找不到关联Id数据", MapStyles.TabMiddleStyle, GUILayout.Width(100f));
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterGroupId))
            {
                var groupIdProperty = mapDataProperty.FindPropertyRelative("GroupId");
                if (groupIdProperty != null)
                {
                    groupIdProperty.intValue = EditorGUILayout.IntField(groupIdProperty.intValue, GUILayout.Width(MapEditorConst.InspectorDataGroupIdUIWidth));
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterCreateRadius))
            {
                var monsterCreateRadiusProperty = mapDataProperty.FindPropertyRelative("MonsterCreateRaduis");
                if (monsterCreateRadiusProperty != null)
                {
                    monsterCreateRadiusProperty.floatValue = EditorGUILayout.FloatField(monsterCreateRadiusProperty.floatValue, GUILayout.Width(MapEditorConst.InspectorDataMonsterCreateRadiusUIWidth));
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterActiveRadius))
            {
                var monsterActiveRadiusProperty = mapDataProperty.FindPropertyRelative("MonsterActiveRaduis");
                if (monsterActiveRadiusProperty != null)
                {
                    monsterActiveRadiusProperty.floatValue = EditorGUILayout.FloatField(monsterActiveRadiusProperty.floatValue, GUILayout.Width(MapEditorConst.InspectorDataMonsterActiveRediusUIWidth));
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MonsterGroupGUISwitchOff))
            {
                var guiSwitchOffProperty = mapDataProperty.FindPropertyRelative("GUISwithOff");
                if (guiSwitchOffProperty != null)
                {
                    var space = 20f;
                    EditorGUILayout.Space(space, true);
                    guiSwitchOffProperty.boolValue = EditorGUILayout.Toggle(guiSwitchOffProperty.boolValue, GUILayout.Width(MapEditorConst.InspectorDataMonsterGroupGUISwitchOffUIWidth - space));
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Position))
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
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Rotation))
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
            //if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Des))
            //{
            //    var des = mapDataConfig != null ? mapDataConfig.Des : "";
            //    EditorGUILayout.LabelField(des, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.InspectorDataDesUIWidth));
            //}
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MoveUp))
            {
                if (GUILayout.Button("↑", GUILayout.Width(MapEditorConst.InspectorDataMoveUpUIWidth)))
                {
                    DoMovePropertyDataUpByIndex(mMapDataListProperty, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.MoveDown))
            {
                if (GUILayout.Button("↓", GUILayout.Width(MapEditorConst.InspectorDataMoveDownUIWidth)))
                {
                    DoMovePropertyDataDownByIndex(mMapDataListProperty, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Add))
            {
                if (GUILayout.Button("+", GUILayout.Width(MapEditorConst.InspectorDataRemoveUIWidth)))
                {
                    var addMapDataValue = mAddMapDataIndexProperty.intValue;
                    DoAddMapData(addMapDataValue, mapDataIndex);
                }
            }
            if (MapEditorUtilities.IsShowMapUI(mapDataType, MapDataUIType.Remove))
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
            if (mSceneGUISwitchProperty != null && mSceneGUISwitchProperty.boolValue)
            {
                var currentEvent = Event.current;
                if (currentEvent.type == EventType.Repaint)
                {
                    DrawMapDataLabels();
                    DrawMapDataSpheres();
                }

                if (currentEvent.type == EventType.KeyDown)
                {
                    if (currentEvent.keyCode == KeyCode.W)
                    {
                        MarkKeyDown(KeyCode.W);
                    }
                    if (currentEvent.keyCode == KeyCode.E)
                    {
                        MarkKeyDown(KeyCode.E);
                    }
                }
                if (currentEvent.type == EventType.KeyUp)
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
                if (IsKeyCodeDown(KeyCode.W))
                {
                    OnWKeyboardClick();
                }
                if (IsKeyCodeDown(KeyCode.E))
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
            if (mSceneGUISwitchProperty != null && mSceneGUISwitchProperty.boolValue)
            {
                DrawMapDataPositionHandles();
            }
        }

        /// <summary>
        /// 响应E按键按下
        /// </summary>
        private void OnEKeyboardClick()
        {
            if (mSceneGUISwitchProperty != null && mSceneGUISwitchProperty.boolValue)
            {
                DrawMapDataRotationHandles();
            }
        }

        /// <summary>
        /// 绘制地图埋点标签
        /// </summary>
        private void DrawMapDataLabels()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                string mapDataLabelName = MapEditorUtilities.GetMapDataPropertyLabelName(mMapDataListProperty, i);
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataBatchOperationSwitchProperty = mapDataProperty.FindPropertyRelative("BatchOperationSwitch");
                var labelPos = mapDataPositionProperty.vector3Value + MapEditorConst.MapDataLabelPosOffset;
                var displayGUIStyle = mapDataBatchOperationSwitchProperty.boolValue ? mYellowLabelGUIStyle : mRedLabelGUIStyle;
                Handles.Label(labelPos, mapDataLabelName, displayGUIStyle);
            }
        }

        /// <summary>
        /// 绘制地图埋点球体
        /// </summary>
        private void DrawMapDataSpheres()
        {
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
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
            for (int i = 0, length = mMapDataListProperty.arraySize; i < length; i++)
            {
                EditorGUI.BeginChangeCheck();
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
                var mapDataPositionProperty = mapDataProperty.FindPropertyRelative("Position");
                var mapDataRotationProperty = mapDataProperty.FindPropertyRelative("Rotation");
                var rotationQuaternion = Quaternion.Euler(mapDataRotationProperty.vector3Value);
                var newTargetPosition = Handles.PositionHandle(mapDataPositionProperty.vector3Value, rotationQuaternion);
                if (EditorGUI.EndChangeCheck())
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
                EditorGUI.BeginChangeCheck();
                var mapDataProperty = GetMapDataSerializedPropertyByIndex(i);
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