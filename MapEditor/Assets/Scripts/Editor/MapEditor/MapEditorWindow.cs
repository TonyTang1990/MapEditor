﻿/*
 * Description:             MapEditorWindow.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapEditorWindow.cs
    /// 地图编辑器配置窗口
    /// </summary>
    public class MapEditorWindow : EditorWindow
    {
        [MenuItem("ToolsWindow/Map/地图编辑器窗口")]
        public static void openMapEditorWindow()
        {
            var mapEditorWindow = EditorWindow.GetWindow<MapEditorWindow>(false, "地图编辑器窗口");
            mapEditorWindow.Show();
        }

        /// <summary>
        /// 地图编辑器面板类型
        /// </summary>
        private enum GameMapPanelType
        {
            MapBuild = 0,           // 地图编辑面板
            DataEditor = 1,         // 数据埋点面板
        }

        /// <summary>
        /// 操作面板标题列表
        /// </summary>
        private string[] mPanelToolBarString = { "地图编辑", "数据埋点" };

        /// <summary>
        /// 操作面板选择索引
        /// </summary>
        private int mPanelToolBarSelectIndex = 0;

        /// <summary>
        /// 操作面板选择游戏编辑器面板类型
        /// </summary>
        private GameMapPanelType mSelectedPanelType = GameMapPanelType.MapBuild;

        /// <summary>
        /// 地图编辑操作面板滚动位置
        /// </summary>
        private Vector2 mMapObjectDataPanelScrollPos;

        /// <summary>
        /// 数据埋点操作面板滚动位置
        /// </summary>
        private Vector2 mMapDataPanelScrollPos;

        /// <summary>
        /// 计时器
        /// </summary>
        private System.Diagnostics.Stopwatch mTimeWatcher = new System.Diagnostics.Stopwatch();

        /// <summary>
        /// 地图编辑器设置数据
        /// </summary>
        private MapSetting mMapSettingAsset;

        /// <summary>
        /// 当前选中添加的地图对象类型配置
        /// </summary>
        private MapObjectType mAddMapObjectTypeConfigType = MapObjectType.Scene;

        /// <summary>
        /// 当前选中地图对象类型配置类型添加的是否是动态对象
        /// </summary>
        private bool mAddMapObjectTypeConfigTypeIsDynaic = true;

        /// <summary>
        /// 添加的地图对象UID
        /// </summary>
        private int mAddMapObjectUID;

        /// <summary>
        /// 当前选中添加的地图对象类型
        /// </summary>
        private MapObjectType mAddMapObjectType;

        /// <summary>
        /// 添加的地图埋点UID
        /// </summary>
        private int mAddMapDataUID;

        /// <summary>
        /// 当前选中的地图埋点类型
        /// </summary>
        private MapDataType mSelectedMapDataType;

        private void Awake()
        {
            InitData();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        private void InitData()
        {
            InitSettingData();
        }

        /// <summary>
        /// 初始化设置数据
        /// </summary>
        private void InitSettingData()
        {
            mMapSettingAsset = MapSetting.GetEditorInstance();
        }

        /// <summary>
        /// 执行添加新地图对象类型配置数据
        /// </summary>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        private bool DoAddMapObjectTypeConfig(MapObjectType mapObjectType)
        {
            return mMapSettingAsset.ObjectSetting.AddMapObjectTypeConfig(mapObjectType);
        }

        /// <summary>
        /// 执行移除地图对象类型配置数据
        /// </summary>
        /// <param name="mapObjectTypeConfig"></param>
        /// <returns></returns>
        private bool DoRemoveMapObjectTypeConfig(MapObjectTypeConfig mapObjectTypeConfig)
        {
            return mMapSettingAsset.ObjectSetting.RemoveMapObjectTypeConfig(mapObjectTypeConfig);
        }

        /// <summary>
        /// 执行添加新地图对象配置数据
        /// </summary>
        /// <param name="mapObjectUid"></param>
        /// <param name="mapObjectType"></param>
        /// <returns></returns>
        private bool DoAddMapObjectConfig(int mapObjectUid, MapObjectType mapObjectType)
        {
            if (mapObjectUid == 0)
            {
                mapObjectUid = mMapSettingAsset.ObjectSetting.GetAvalibleUID();
            }
            var newMapObjectConfig = new MapObjectConfig(mapObjectUid, mapObjectType);
            return mMapSettingAsset.ObjectSetting.AddMapObjectDataConfig(newMapObjectConfig);
        }

        /// <summary>
        /// 执行移除地图对象配置数据
        /// </summary>
        /// <param name="mapObjectConfig"></param>
        /// <returns></returns>
        private bool DoRemoveMapObjectConfig(MapObjectConfig mapObjectConfig)
        {
            return mMapSettingAsset.ObjectSetting.RemoveMapObjectConfig(mapObjectConfig);
        }

        /// <summary>
        /// 执行添加新地图埋点配置数据
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private bool DoAddMapDataConfig(int uid, MapDataType mapDataType)
        {
            if(uid == 0)
            {
                uid = mMapSettingAsset.DataSetting.GetAvalibleUID();
            }
            var mapDataTypeColor = MapEditorUtilities.GetMapDataColor(mapDataType);
            var newMapDataConfig = new MapDataConfig(uid, mapDataType, mapDataTypeColor);
            return mMapSettingAsset.DataSetting.AddMapDataConfig(newMapDataConfig);
        }

        /// <summary>
        /// 执行移除地图埋点配置数据
        /// </summary>
        /// <param name="mapDataConfig"></param>
        /// <returns></returns>
        private bool DoRemoveMapDataConfig(MapDataConfig mapDataConfig)
        {
            return mMapSettingAsset.DataSetting.RemoveMapDataConfig(mapDataConfig);
        }

        /// <summary>
        /// 响应地图对象配置关联id变化
        /// </summary>
        private void OnMapObjectConfigIdChange()
        {
            mMapSettingAsset.ObjectSetting.DoSortMapObjectConfigs();
        }

        /// <summary>
        /// 响应地图埋点配置关联id变化
        /// </summary>
        private void OnMapDataConfigIdChange()
        {

        }
        

        /// <summary>
        /// 执行地图编辑配置数据保存
        /// </summary>
        private void DoSaveGameMapSetting()
        {
            if (mMapSettingAsset == null)
            {
                Debug.LogError($"不允许保存空地图编辑配置数据，保存地图编辑配置数据失败！");
                return;
            }
            EditorUtility.SetDirty(mMapSettingAsset);
            AssetDatabase.SaveAssets();
        }

        /// <summary>
        /// 打开地图编辑器场景
        /// </summary>
        private void DoOpenGameMapScene()
        {
            EditorSceneManager.OpenScene(MapEditorConst.MapEditorScenePath);
        }

        /// <summary>
        /// 快速选中常经理的地图编辑对象(挂在Map脚本的)
        /// </summary>
        private void DoQuickSelectGameMapInScene()
        {
            var currentPrefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if(currentPrefabStage != null)
            {
                var map = GameObject.FindObjectOfType<Map>();
                if (map != null)
                {
                    Selection.SetActiveObjectWithContext(map.gameObject, map.gameObject);
                }
            }
            else
            {
                var prefabRootContent = currentPrefabStage.prefabContentsRoot;
                Selection.SetActiveObjectWithContext(prefabRootContent, prefabRootContent);
            }
        }
        
        /// <summary>
        /// 一键导出所有关卡地图数据
        /// </summary>
        private void DoExportAllMapDatas()
        {
            var allMapAssetPath = MapEditorUtilities.GetAllMapAssetPath();
            foreach(var prefabAssetPath in allMapAssetPath)
            {
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabAssetPath);
                if(prefab == null)
                {
                    continue;
                }
                var map = prefab.GetComponent<Map>();
                if(map == null)
                {
                    continue;
                }
                MapExportEditorUtilities.ExportGameMapData(map);
                Debug.Log($"地图预制件:{prefab.name}地图数据导出完成！");
            }
        }
   
        /// <summary>
        /// 一键烘焙和导出所有地图数据
        /// </summary>
        private async void DoBakeAndExportAllMapDatas()
        {
            EditorUtility.ClearProgressBar();
            StageUtility.GoToMainStage();
            var allMapAssetPath = MapEditorUtilities.GetAllMapAssetPath();
            for(int i = 0, length = allMapAssetPath.Count; i < length; i++)
            {
                var mapAssetPath = allMapAssetPath[i];
                EditorUtility.DisplayCancelableProgressBar("一键拷贝和导出地图数据中", $"当前处理:{mapAssetPath}", i / length);
                var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(mapAssetPath);
                if (prefab == null)
                {
                    continue;
                }
                var prefabStage = PrefabStageUtility.OpenPrefab(mapAssetPath);
                var prefabContentRoot = prefabStage.prefabContentsRoot;
                var map = prefabContentRoot.GetComponent<Map>();
                if(map == null)
                {
                    StageUtility.GoToMainStage();
                    continue;
                }
                await map.OneKeyBakeAndExport();
                StageUtility.GoToMainStage();
            }
            EditorUtility.ClearProgressBar();
            Debug.Log($"一键烘焙和导出所有地图数据完成！");
        }

        private void OnGUI()
        {
            DrawOperationPanelArea();
            DrawDefaultSettingArea();
            DrawGameMapPanelArea();
        }

        /// <summary>
        /// 绘制操作面板
        /// </summary>
        private void DrawOperationPanelArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("快捷操作面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("保存地图配置数据", GUILayout.Width(150f)))
            {
                DoSaveGameMapSetting();
            }
            if (GUILayout.Button("打开地图编辑场景", GUILayout.Width(150f)))
            {
                DoOpenGameMapScene();
            }
            if (GUILayout.Button("快速选中地编对象", GUILayout.Width(150f)))
            {
                DoQuickSelectGameMapInScene();
            }
            if (GUILayout.Button("一键导出所有地图", GUILayout.Width(150f)))
            {
                DoExportAllMapDatas();
            }
            if (GUILayout.Button("一键烘焙导出所有地图", GUILayout.Width(150f)))
            {
                DoBakeAndExportAllMapDatas();
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制默认设置面板
        /// </summary>
        private void DrawDefaultSettingArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("默认设置", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("默认地图宽:", GUILayout.Width(80f));
            mMapSettingAsset.DefaultMapWidth = EditorGUILayout.IntField(mMapSettingAsset.DefaultMapWidth, GUILayout.Width(80f));
            EditorGUILayout.LabelField("默认地图长:", GUILayout.Width(80f));
            mMapSettingAsset.DefaultMapHeight = EditorGUILayout.IntField(mMapSettingAsset.DefaultMapHeight, GUILayout.Width(80f));
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制游戏地图面板显示
        /// </summary>
        private void DrawGameMapPanelArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            mPanelToolBarSelectIndex = GUILayout.Toolbar(mPanelToolBarSelectIndex, mPanelToolBarString, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            mSelectedPanelType = (GameMapPanelType)mPanelToolBarSelectIndex;
            EditorGUILayout.EndHorizontal();
            if (mSelectedPanelType == GameMapPanelType.MapBuild)
            {
                DrawMapBuildPanelArea();
            }
            else if (mSelectedPanelType == GameMapPanelType.DataEditor)
            {
                DrawDataEditorPanelArea();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图编辑器面板显示
        /// </summary>
        private void DrawMapBuildPanelArea()
        {
            DrawMapObjectTypeConfigArea();
            DrawMapObjectConfigArea();
        }

        /// <summary>
        /// 绘制地图对象类型配置区域
        /// </summary>
        private void DrawMapObjectTypeConfigArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("地图对象类型编辑面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            mMapSettingAsset.ObjectSetting.IsMapObjectTypeConfigUnfold = EditorGUILayout.Foldout(mMapSettingAsset.ObjectSetting.IsMapObjectTypeConfigUnfold, "地图对象类型配置");
            if(mMapSettingAsset.ObjectSetting.IsMapObjectTypeConfigUnfold)
            {
                DrawMapObjectTypeConfigAddArea();
                DrawMapObjectTypeConfigTitleArea();
                if(mMapSettingAsset.ObjectSetting.AllMapObjectTypeConfigs.Count > 0)
                {
                    for(int i = 0; i < mMapSettingAsset.ObjectSetting.AllMapObjectTypeConfigs.Count; i++)
                    {
                        DrawOneMapObjectTypeConfig(mMapSettingAsset.ObjectSetting.AllMapObjectTypeConfigs[i]);
                    }
                }
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图编辑器对象类型配置添加显示
        /// </summary>
        private void DrawMapObjectTypeConfigAddArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图对象类型:", GUILayout.Width(80f));
            mAddMapObjectTypeConfigType = (MapObjectType)EditorGUILayout.EnumPopup(mAddMapObjectTypeConfigType, GUILayout.Width(150f));
            if(GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                DoAddMapObjectTypeConfig(mAddMapObjectTypeConfigType);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图编辑对象类型配置数据标题显示
        /// </summary>
        private void DrawMapObjectTypeConfigTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("地图对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeConfigUIWidth));
            EditorGUILayout.LabelField("地图对象类型描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeConfigDesUIWidth));
            EditorGUILayout.LabelField("操作", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制单个地图对象类型配置数据显示
        /// </summary>
        /// <param name="mapObjectTypeConfig"></param>
        private void DrawOneMapObjectTypeConfig(MapObjectTypeConfig mapObjectTypeConfig)
        {
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true));
            if(mapObjectTypeConfig != null)
            {
                var preColor = GUI.color;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(mapObjectTypeConfig.ObjectType.ToString(), MapStyles.ButtonLeftStyle, GUILayout.Width(MapEditorConst.MapObjectTypeConfigUIWidth));
                mapObjectTypeConfig.Des = EditorGUILayout.TextField(mapObjectTypeConfig.Des, GUILayout.Width(MapEditorConst.MapObjectTypeConfigDesUIWidth));
                if(GUILayout.Button("-", GUILayout.ExpandWidth(true)))
                {
                    DoRemoveMapObjectTypeConfig(mapObjectTypeConfig);
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = preColor;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图对象配置区域
        /// </summary>
        private void DrawMapObjectConfigArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("地图编辑面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawMapObjectConfigAddArea();
            DrawMapObjectConfigTitleArea();
            mMapObjectDataPanelScrollPos = EditorGUILayout.BeginScrollView(mMapObjectDataPanelScrollPos);
            if (mMapSettingAsset.ObjectSetting.AllMapObjectConfigs.Count > 0)
            {
                for (int i = 0; i < mMapSettingAsset.ObjectSetting.AllMapObjectConfigs.Count; i++)
                {
                    DrawOneMapObjectConfigArea(mMapSettingAsset.ObjectSetting.AllMapObjectConfigs[i]);
                }
            }
            else
            {
                EditorGUILayout.LabelField("待添加地图对象数据", MapStyles.CenterLabelStyle, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图编辑对象添加显示
        /// </summary>
        private void DrawMapObjectConfigAddArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图对象UID(0表示自动生成):", GUILayout.Width(170f));
            mAddMapObjectUID = EditorGUILayout.IntField(mAddMapObjectUID, GUILayout.Width(100f));
            EditorGUILayout.LabelField("地图对象类型:", GUILayout.Width(80f));
            mAddMapObjectType = (MapObjectType)EditorGUILayout.EnumPopup(mAddMapObjectType, GUILayout.Width(150f));
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                DoAddMapObjectConfig(mAddMapObjectUID, mAddMapObjectType);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图编辑对象配置数据标题显示
        /// </summary>
        private void DrawMapObjectConfigTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectUIDUIWidth));
            EditorGUILayout.LabelField("地图对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeUIWidth));
            EditorGUILayout.LabelField("关联配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectConfIdUIWidth));
            EditorGUILayout.LabelField("地图对象Asset", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectAssetUIWidth));
            EditorGUILayout.LabelField("地图对象描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectDesUIWidth));
            EditorGUILayout.LabelField("地图对象预览", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectPreviewUIWidth));
            EditorGUILayout.LabelField("操作", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制单个地图对象配置数据显示
        /// </summary>
        /// <param name="mapObjectConfig"></param>
        private void DrawOneMapObjectConfigArea(MapObjectConfig mapObjectConfig)
        {
            var configViewMaxHeight = Math.Max(20f, MapEditorConst.MapObjectPreviewUIHeight);
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.Height(configViewMaxHeight));
            if (mapObjectConfig != null)
            {
                var preColor = GUI.color;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField(mapObjectConfig.UID, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectUIDUIWidth));
                EditorGUILayout.LabelField(mapObjectConfig.ObjectType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeUIWidth));
                EditorGUI.BeginChangeCheck();
                mapObjectConfig.ConfId = EditorGUILayout.IntField(mapObjectConfig.ConfId, GUILayout.Width(MapEditorConst.MapObjectConfIdUIWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    OnMapObjectConfigIdChange();
                }
                mapObjectConfig.Asset = (GameObject)EditorGUILayout.ObjectField(mapObjectConfig.Asset, MapConst.GameObjectType, false, GUILayout.Width(MapEditorConst.MapObjectAssetUIWidth));
                mapObjectConfig.Des = EditorGUILayout.TextField(mapObjectConfig.Des, GUILayout.Width(MapEditorConst.MapObjectDesUIWidth));
                var assetPreview = mapObjectConfig != null ? MapEditorUtilities.GetAssetPreview(mapObjectConfig.Asset) : null;
                GUILayout.Box(assetPreview, GUILayout.Width(MapEditorConst.MapObjectPreviewUIWidth), GUILayout.Height(MapEditorConst.MapObjectPreviewUIHeight));
                if (GUILayout.Button("-", GUILayout.ExpandWidth(true)))
                {
                    DoRemoveMapObjectConfig(mapObjectConfig);
                }
                EditorGUILayout.EndHorizontal();
                GUI.color = preColor;
            }
            else
            {
                EditorGUILayout.LabelField("空地图对象数据", MapStyles.CenterLabelStyle, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制数据埋点面板显示
        /// </summary>
        private void DrawDataEditorPanelArea()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("数据埋点面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawMapDataConfigAddArea();
            DrawMapDataConfigTitleArea();
            mMapDataPanelScrollPos = EditorGUILayout.BeginScrollView(mMapDataPanelScrollPos);
            if (mMapSettingAsset.DataSetting.AllMapDataConfigs.Count > 0)
            {
                for (int i = 0; i < mMapSettingAsset.DataSetting.AllMapDataConfigs.Count; i++)
                {
                    DrawOneMapDataConfigArea(mMapSettingAsset.DataSetting.AllMapDataConfigs[i]);
                }
            }
            else
            {
                EditorGUILayout.LabelField("待添加地图埋点数据", MapStyles.CenterLabelStyle, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图埋点对象添加显示
        /// </summary>
        private void DrawMapDataConfigAddArea()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图埋点UID(0表示自动生成):", GUILayout.Width(170f));
            mAddMapDataUID = EditorGUILayout.IntField(mAddMapDataUID, GUILayout.Width(100f));
            EditorGUILayout.LabelField("地图埋点类型:", GUILayout.Width(80f));
            mSelectedMapDataType = (MapDataType)EditorGUILayout.EnumPopup(mSelectedMapDataType, GUILayout.Width(150f));
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                DoAddMapDataConfig(mAddMapDataUID, mSelectedMapDataType);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图埋点对象配置数据标题显示
        /// </summary>
        private void DrawMapDataConfigTitleArea()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataUIDUIWidth));
            EditorGUILayout.LabelField("地图埋点类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataTypeUIWidth));
            EditorGUILayout.LabelField("关联配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataConfIdUIWidth));
            EditorGUILayout.LabelField("场景球体颜色", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataColorUIWidth));
            //EditorGUILayout.LabelField("初始旋转", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataRotationUIWidth));
            EditorGUILayout.LabelField("地图埋点描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataDesUIWidth));
            EditorGUILayout.LabelField("操作", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制单个地图埋点配置数据显示
        /// </summary>
        /// <param name="mapDataConfig"></param>
        private void DrawOneMapDataConfigArea(MapDataConfig mapDataConfig)
        {
            EditorGUILayout.BeginVertical("box");
            if (mapDataConfig != null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField(mapDataConfig.UID, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataUIDUIWidth));
                EditorGUILayout.LabelField(mapDataConfig.DataType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataTypeUIWidth));
                EditorGUI.BeginChangeCheck();
                mapDataConfig.ConfId = EditorGUILayout.IntField(mapDataConfig.ConfId, GUILayout.Width(MapEditorConst.MapDataConfIdUIWidth));
                if (EditorGUI.EndChangeCheck())
                {
                    OnMapDataConfigIdChange();
                }
                mapDataConfig.SceneSphereColor = EditorGUILayout.ColorField(mapDataConfig.SceneSphereColor, GUILayout.Width(MapEditorConst.MapDataColorUIWidth));
                //var newRotationVector3 = mapDataConfig.Rotation;
                //EditorGUI.BeginChangeCheck();
                //var rotationPositionWidth = (MapEditorConst.MapDataRotationUIWidth - 50f) / 3;
                //EditorGUILayout.LabelField("X", GUILayout.Width(10f));
                //newRotationVector3.x = EditorGUILayout.FloatField(newRotationVector3.x, GUILayout.Width(rotationPositionWidth));
                //EditorGUILayout.LabelField("Y", GUILayout.Width(10f));
                //newRotationVector3.y = EditorGUILayout.FloatField(newRotationVector3.y, GUILayout.Width(rotationPositionWidth));
                //EditorGUILayout.LabelField("Z", GUILayout.Width(10f));
                //newRotationVector3.z = EditorGUILayout.FloatField(newRotationVector3.z, GUILayout.Width(rotationPositionWidth));
                //if(EditorGUI.EndChangeCheck())
                //{
                //    mapDataConfig.Rotation = newRotationVector3;
                //}
                mapDataConfig.Des = EditorGUILayout.TextField(mapDataConfig.Des, GUILayout.Width(MapEditorConst.MapDataDesUIWidth));
                if (GUILayout.Button("-", GUILayout.ExpandWidth(true)))
                {
                    DoRemoveMapDataConfig(mapDataConfig);
                }
                EditorGUILayout.EndHorizontal();
            }
            else
            {
                EditorGUILayout.LabelField("空地图埋点数据", MapStyles.CenterLabelStyle, GUILayout.ExpandWidth(true));
            }
            EditorGUILayout.EndVertical();
        }
    }
}