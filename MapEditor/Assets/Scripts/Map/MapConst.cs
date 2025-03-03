/*
 * Description:             MapConst.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapConst.cs
    /// 地图编辑器常量
    /// </summary>
    public static class MapConst
    {
        /// <summary>
        /// 游戏地图设置保存路径
        /// </summary>
        public const string MapSettingSavePath = "Assets/Editor/Map/MapSetting.asset";

        /// <summary>
        /// 地图导出目录相对路径(相对Asset目录)
        /// </summary>
        public const string MapExportRelativePath = "Assets/Resources/MapExport";

        /// <summary>
        /// 地图横向默认大小
        /// </summary>
        public const int DefaultMapWidth = 6;

        /// <summary>
        /// 地图纵向默认大小
        /// </summary>
        public const int DefaultMapHeight = 30;

        /// <summary>
        /// 地图九宫格默认大小
        /// </summary>
        public const float DefaultGridSize = 6f;

        /// <summary>
        /// GameObject类型信息
        /// </summary>
        public static readonly Type GameObjectType = typeof(UnityEngine.GameObject);

        /// <summary>
        /// MapObjectType类型信息
        /// </summary>
        public static readonly Type MapObjectType = typeof(MapObjectType);

        /// <summary>
        /// MapDataType类型信息
        /// </summary>
        public static readonly Type MapDataType = typeof(MapDataType);

        /// <summary>
        /// LevelMapData类型信息
        /// </summary>
        public static readonly Type LevelMapDataType = typeof(LevelMapData);

        /// <summary>
        /// BaseActorEntity类型信息
        /// </summary>
        public static readonly Type BaseActorEntityType = typeof(BaseActorEntity);

        /// <summary>
        /// 地图对象父节点挂点名
        /// </summary>
        public const string MapObjectParentNodeName = "MapObjectParent";

        /// <summary>
        /// 地图地块节点名
        /// </summary>
        public const string MapTerrianNodeName = "MapTerrian";

        /// <summary>
        /// 游戏虚拟摄像机父节点名
        /// </summary>
        public const string GameVirtualCameraParentNodeName = "GameVirtualCamera";

        /// <summary>
        /// 游戏虚拟摄像机节点名
        /// </summary>
        public const string GameVirtualCameraNodeName = "VirtualCamera";

        /// <summary>
        /// 默认地图地形预制件路径
        /// </summary>
        public const string DetaulMapTerrianPrefabPath = "Assets/Resources/Terrian/Terrian.prefab";

        /// <summary>
        /// 游戏虚拟摄像机优先级
        /// </summary>
        public const int GameVirtualCameraPriority = 100;

        /// <summary>
        /// 游戏虚拟摄像机默认位置
        /// </summary>
        public static readonly Vector3 GameVirtualCameraDefaultPos = new Vector3(3, 15, -2);

        /// <summary>
        /// 游戏虚拟摄像机默认旋转
        /// </summary>
        public static readonly Vector3 GameVirtualCameraDefaultRot = new Vector3(45, 0, 0);

        /// <summary>
        /// 默认怪物创建半径
        /// </summary>
        public const float DefaultMonsterCreateRadius = 8f;

        /// <summary>
        /// 默认怪物警戒半径
        /// </summary>
        public const float DefaultMonsterActiveRadius = 5f;

        /// <summary>
        /// 玩家默认移动速度
        /// </summary>
        public const float PlayerDefaultMoveSpeed = 5f;
    }
}