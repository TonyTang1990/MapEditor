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
        /// 添加的地图对象UID
        /// </summary>
        private int mAddMapObjectUID;

        /// <summary>
        /// 当前选中添加的地图对象类型
        /// </summary>
        private MapObjectType mAddMapObjectType;

        /// <summary>
        /// 当前选中添加的是否是动态地图对象
        /// </summary>
        private bool mAddIsDynamic;

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
            mMapSettingAsset = MapUtilities.LoadOrCreateGameMapSetting();
        }

        /// <summary>
        /// 执行添加新地图对象配置数据
        /// </summary>
        /// <param name="mapObjectUid"></param>
        /// <param name="mapObjectType"></param>
        /// <param name="isDynamic"></param>
        /// <returns></returns>
        private bool DoAddMapObjectConfig(int mapObjectUid, MapObjectType mapObjectType, bool isDynamic = false)
        {
            var newMapObjectConfig = new MapObjectConfig(mapObjectUid, mapObjectType, isDynamic);
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
            var map = GameObject.FindObjectOfType<Map>();
            if (map != null)
            {
                Selection.SetActiveObjectWithContext(map.gameObject, map.gameObject);
            }
        }

        private void OnGUI()
        {
            DrawOperationPanelView();
            DrawDefaultSettingView();
            DrawGameMapPanelView();
        }

        /// <summary>
        /// 绘制操作面板
        /// </summary>
        private void DrawOperationPanelView()
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
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制默认设置面板
        /// </summary>
        private void DrawDefaultSettingView()
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
        private void DrawGameMapPanelView()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.BeginHorizontal();
            mPanelToolBarSelectIndex = GUILayout.Toolbar(mPanelToolBarSelectIndex, mPanelToolBarString, EditorStyles.toolbarButton, GUILayout.ExpandWidth(true));
            mSelectedPanelType = (GameMapPanelType)mPanelToolBarSelectIndex;
            EditorGUILayout.EndHorizontal();
            if (mSelectedPanelType == GameMapPanelType.MapBuild)
            {
                DrawMapBuildPanelView();
            }
            else if (mSelectedPanelType == GameMapPanelType.DataEditor)
            {
                DrawDataEditorPanelView();
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// 绘制地图编辑器面板显示
        /// </summary>
        private void DrawMapBuildPanelView()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("地图编辑面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawMapObjectConfigAddView();
            DrawMapObjectConfigTitleView();
            mMapObjectDataPanelScrollPos = EditorGUILayout.BeginScrollView(mMapObjectDataPanelScrollPos);
            if (mMapSettingAsset.ObjectSetting.AllMapObjectConfigs.Count > 0)
            {
                for (int i = 0; i < mMapSettingAsset.ObjectSetting.AllMapObjectConfigs.Count; i++)
                {
                    DrawOneMapObjectConfigView(mMapSettingAsset.ObjectSetting.AllMapObjectConfigs[i]);
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
        private void DrawMapObjectConfigAddView()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图对象UID:", GUILayout.Width(80f));
            mAddMapObjectUID = EditorGUILayout.IntField(mAddMapObjectUID, GUILayout.Width(100f));
            EditorGUILayout.LabelField("地图对象类型:", GUILayout.Width(80f));
            mAddMapObjectType = (MapObjectType)EditorGUILayout.EnumPopup(mAddMapObjectType, GUILayout.Width(150f));
            EditorGUILayout.LabelField("是否动态对象:", GUILayout.Width(80f));
            mAddIsDynamic = EditorGUILayout.Toggle(mAddIsDynamic, GUILayout.ExpandWidth(true));
            if (GUILayout.Button("+", GUILayout.ExpandWidth(true)))
            {
                DoAddMapObjectConfig(mAddMapObjectUID, mAddMapObjectType, mAddIsDynamic);
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制地图编辑对象配置数据标题显示
        /// </summary>
        private void DrawMapObjectConfigTitleView()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectUIDUIWidth));
            EditorGUILayout.LabelField("地图对象类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeUIWidth));
            EditorGUILayout.LabelField("是否动态", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectIsDynamicUIWidth));
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
        private void DrawOneMapObjectConfigView(MapObjectConfig mapObjectConfig)
        {
            var configViewMaxHeight = Math.Max(20f, MapEditorConst.MapObjectPreviewUIHeight);
            EditorGUILayout.BeginVertical("box", GUILayout.ExpandWidth(true), GUILayout.Height(configViewMaxHeight));
            if (mapObjectConfig != null)
            {
                var preColor = GUI.color;
                GUI.color = mapObjectConfig.IsDynamic ? Color.yellow : preColor;
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.IntField(mapObjectConfig.UID, MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectUIDUIWidth));
                EditorGUILayout.LabelField(mapObjectConfig.ObjectType.ToString(), MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapObjectTypeUIWidth));
                EditorGUILayout.Space(MapEditorConst.MapObjectIsDynamicUIWidth / 3, false);
                mapObjectConfig.IsDynamic = EditorGUILayout.Toggle(mapObjectConfig.IsDynamic, GUILayout.Width(MapEditorConst.MapObjectIsDynamicUIWidth * 2 / 3));
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
        private void DrawDataEditorPanelView()
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.LabelField("数据埋点面板", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            DrawMapDataConfigAddView();
            DrawMapDataConfigTitleView();
            mMapDataPanelScrollPos = EditorGUILayout.BeginScrollView(mMapDataPanelScrollPos);
            if (mMapSettingAsset.DataSetting.AlllMapDataConfigs.Count > 0)
            {
                for (int i = 0; i < mMapSettingAsset.DataSetting.AlllMapDataConfigs.Count; i++)
                {
                    DrawOneMapDataConfigView(mMapSettingAsset.DataSetting.AlllMapDataConfigs[i]);
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
        private void DrawMapDataConfigAddView()
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("地图埋点UID:", GUILayout.Width(80f));
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
        private void DrawMapDataConfigTitleView()
        {
            EditorGUILayout.BeginHorizontal("box");
            EditorGUILayout.LabelField("UID", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataUIDUIWidth));
            EditorGUILayout.LabelField("地图埋点类型", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataTypeUIWidth));
            EditorGUILayout.LabelField("关联配置Id", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataConfIdUIWidth));
            EditorGUILayout.LabelField("场景球体颜色", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataColorUIWidth));
            EditorGUILayout.LabelField("地图埋点描述", MapStyles.TabMiddleStyle, GUILayout.Width(MapEditorConst.MapDataDesUIWidth));
            EditorGUILayout.LabelField("操作", MapStyles.TabMiddleStyle, GUILayout.ExpandWidth(true));
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 绘制单个地图埋点配置数据显示
        /// </summary>
        /// <param name="mapDataConfig"></param>
        private void DrawOneMapDataConfigView(MapDataConfig mapDataConfig)
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