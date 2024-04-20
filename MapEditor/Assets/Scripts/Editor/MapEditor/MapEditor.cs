/*
 * Description:             MapEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

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
        private SerializedProperty mAddMapObjectValueProperty;

        /// <summary>
        /// AddMapDataValue属性
        /// </summary>
        private SerializedProperty mAddMapDataValueProperty;

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
            mMapWidthProperty.intValue = mMapWidthProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapWidth;
            mMapHeightProperty.intValue = mMapHeightProperty.intValue == 0 ? MapSetting.GetEditorInstance().DefaultMapHeight;
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
            mAddMapObjectValueProperty ??= serializedObject.FindProperty("AddMapObjectValue");
            mAddMapDataValueProperty ??= serializedObject.FindProperty("AddMapDataValue");
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
                mLabelGUIStyle.aligment = TextAnchor.MiddleCenter;
                mLabelGUIStyle.normal.textColor = Color.red;
            }
        }

        /// <summary>
        /// 创建所有节点
        /// </summary>
        private void CreateAllNode()
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
                var mapObjectTypeParentNodeTransform = MapEditorUtilities.GetOrCreateMapObjectTypeParentNode(mapObjectType);
                mapObjectTypeParentNodeTransform.localPosition = Vector3.zero;
            }
        }

        /// <summary>
        /// 创建地图地形节点
        /// </summary>
        private void CreateMapTerrianNode()
        {
            var customAsset = mMapTerrianAssetProperty.objectReferenceValue as GameObject;
            var mapTerrianTransform = MapEditorUtilities.GetOrCreateMapterrianNode(mTarget.gameObject, customAsset);
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
                meshRenderer.shareMaterial.SetTextureScale("_MainTex", new Vector2(mapWidth, mapHeight));
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
                var goProperty = MapObjectData.FindPropertyRelative("Go");
                if(goProperty.objectReferenceValue != null)
                {
                    var positionProperty = mapObjectDataProperty.FindPropertyRelative("Position");
                    var mapObjectGO = goProperty.objectReferenceValue as GameObject;
                    positionProperty.vector3Value = mapObjectGO.transform.position;
                    if(mapObjectConfig.IsDynamic)
                    {
                        var collider = mapObjectGO.GetComponent<BoxCollider>();
                        var colliderCenterProperty = mapObjectDataProperty.FindPropertyRelative("ColliderCenter");
                        var colliderSizeProperty = mapObjectDataProperty.FindPropertyRelative("ColliderSize");
                        colliderCenterProperty.vector3Value = collider.center;
                        colliderSizeProperty.vector3Value = collider.size;
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
            if(mAddMapObjectValueProperty.intValue > totalOptionNum)
            {
                mAddMapObjectValueProperty.intValue = Math.Clamp(mAddMapObjectValueProperty.intValue, 0, totalOptionNum);
            }
        }

        /// <summary>
        /// 矫正添加地图埋点索引值(避免因为配置面板修改导致索引超出有效范围问题)
        /// </summary>
        private void CorrectAddMapDataIndexValue()
        {
            var totalOptionNum = mMapDataChoiceOptions.Length;
            if (mAddMapDataValueProperty.intValue > totalOptionNum)
            {
                mAddMapDataValueProperty.intValue = Math.Clamp(mAddMapDataValueProperty.intValue, 0, totalOptionNum);
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
        private void UpdateAddMapObjectDataPreviewAssset()
        {
            var addMapObjectValue = mAddMapObjectValueProperty.intValue;
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
                var instanceGoName = instanceGo.name.RemoveSubStr("Clone()");
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
            if(propertyList == null || propertyList.isArray)
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
            var mapData2 = mapDataIndex2Property.managerReferenceValue;
            var mapDataIndex1Property = propertyList.GetArrayElementAtIndex(exchangeIndex1);
            var mapData1 = mapDataIndex1Property.managerReferenceValue;
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
                var preInsertPos = Math.Clamp(insertPos - 1, 0, maxInsertIndex);
                var preInsertMapObjectProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(preInsertPos);
                var preMapObjectData = preInsertMapObjectProperty.managerReferenceValue as MapObjectData;
                mapObjectPosition = preMapObjectData.Go != null ? preMapObjectData.Go.transform.position : preMapObjectData.Position;
            }
            var instanceGo = CreateGameObjectByUID(uid);
            if(instanceGo != null)
            {
                Selection.SetActiveObjectWithContext(instanceGo, instanceGo);
            }
            instanceGo.transform.position = mapObjectPosition;
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
            if(goProperty.objectRefrenceValue != null)
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
                var preInsertPos = Math.Clamp(insertPos - 1, 0, maxInsertIndex);
                var preInsertMapDataProperty = mMapDataListProperty.GetArrayElementAtIndex(preInsertPos);
                var preMapData = preInsertMapDataProperty.managedReferenceValue as MapData;
                mapDataPosition = preMapData != null ? preMapData.Position : mapDataPosition;
            }
            var newMapData = MapUtilities.CreateMapDataByType(mapDataType, uid, mapDataPosition);
            var newMapObjectDataProperty = mMapObjectDataListProperty.GetArrayElementAtIndex(insertPos);
            newMapObjectDataProperty.managedReferenceValue = newMapObjectData;
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
            var isPrefabAssetInstance = PrefabUtility.IsPartOfPrefabInstance(mTarget.gameObject);
            if(isPrefabAssetInstance)
            {
                var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                if(prefabStage == null)
                {
                    Debug.LogError($"此功能在预制件Asset下仅支持打开嵌套预制件模式下使用！");
                    return;
                }
            }
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
                    var go = goProperty.objectReference as GameObject;
                    GameObject.DestroyImmediate(go);
                    goProperty.objectReferenceValue = null;
                }
            }
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
                        instanceGo.transform.position = positionProperty.vector3Value;
                        goProperty.objectReferenceValue = instanceGo;
                        var collider = instanceGo.GetComponent<BoxCollider>();
                        collider.center = colliderCenterProperty.vector3Value;
                        collider.size = colliderSizeProperty.vector3Value;
                    }
                }
            }
        }

    }
}
