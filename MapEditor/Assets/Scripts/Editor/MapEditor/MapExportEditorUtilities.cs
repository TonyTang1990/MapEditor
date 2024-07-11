/*
 * Description:             MapExportEditorUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/29
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapExportEditorUtilities.cs
    /// 地图导出工具类
    /// </summary>
    public static class MapExportEditorUtilities
    {
        /// <summary>
        /// 获取地图导出目录全路径
        /// </summary>
        /// <returns></returns>
        public static string GetGameMapExportFolderFullPath(ExportType exportType)
        {
            var gameMapExportFolderFullPath = PathUtilities.GetAssetFullPath(MapConst.MapExportRelativePath);
            var folderName = exportType.ToString();
            gameMapExportFolderFullPath = Path.Combine(gameMapExportFolderFullPath, folderName);
            return gameMapExportFolderFullPath;
        }

        /// <summary>
        /// 检查或创建地图导出目录
        /// </summary>
        public static void CheckOrCreateGameMapExportFolder(ExportType exportType)
        {
            var gameMapExportFolderFullPath = GetGameMapExportFolderFullPath(exportType);
            FolderUtilities.CheckAndCreateSpecificFolder(gameMapExportFolderFullPath);
        }

        /// <summary>
        /// 获取指定导出类型和导出文件名的导出全路径
        /// </summary>
        /// <param name="exportType"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static string GetGameMapExportFileFullPath(ExportType exportType, string fileName)
        {
            var gameMapExportFolderFullPath = GetGameMapExportFolderFullPath(exportType);
            var filePostFix = GetExportFilePostFix(exportType);
            var fileFullName = $"{fileName}{filePostFix}";
            var gameMapExportFileFullPath = Path.Combine(gameMapExportFolderFullPath, fileFullName);
            return gameMapExportFileFullPath;
        }

        /// <summary>
        /// 导出类型和文件后缀名Map
        /// </summary>
        private static Dictionary<ExportType, string> ExportTypePostFixMap = new Dictionary<ExportType, string>()
        {
            { ExportType.JSON, ".json" },
        };

        /// <summary>
        /// 获取制定导出类型的文件后缀名
        /// </summary>
        /// <param name="exportType"></param>
        /// <returns></returns>
        public static string GetExportFilePostFix(ExportType exportType)
        {
            string postFix = string.Empty;
            if (!ExportTypePostFixMap.TryGetValue(exportType, out postFix))
            {
                Debug.LogError($"找不到导出类型:{exportType.ToString()}的文件后缀名!");
            }
            return postFix;
        }

        /// <summary>
        /// 临时StringBuild(优化字符串拼接问题)
        /// </summary>
        private static StringBuilder TempStringBuild = new StringBuilder();

        /// <summary>
        /// 获取地图脚本的所有地图对象类型和地图对象数据列表Map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static Dictionary<MapObjectType, List<MapObjectData>> GetMapObjecTypeDatas(Map map)
        {
            if (map == null)
            {
                return null;
            }
            Dictionary<MapObjectType, List<MapObjectData>> mapObjectTypeDatasMap = new Dictionary<MapObjectType, List<MapObjectData>>();
            MapObjectConfig mapObjectConfig;
            foreach (var mapObjectData in map.MapObjectDataList)
            {
                mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    continue;
                }
                List<MapObjectData> mapObjectDataList;
                if (!mapObjectTypeDatasMap.TryGetValue(mapObjectConfig.ObjectType, out mapObjectDataList))
                {
                    mapObjectDataList = new List<MapObjectData>();
                    mapObjectTypeDatasMap.Add(mapObjectConfig.ObjectType, mapObjectDataList);
                }
                mapObjectDataList.Add(mapObjectData);
            }
            return mapObjectTypeDatasMap;
        }

        /// <summary>
        /// 获取指定地图埋点数据列表的埋点类型和埋点数据列表Map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static Dictionary<MapDataType, List<MapData>> GetMapDataTypeDatas(List<MapData> mapDatas)
        {
            if (mapDatas == null)
            {
                return null;
            }
            Dictionary<MapDataType, List<MapData>> mapDataTypeDatasMap = new Dictionary<MapDataType, List<MapData>>();
            MapDataConfig mapDataConfig;
            foreach (var mapData in mapDatas)
            {
                mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if (mapDataConfig == null)
                {
                    continue;
                }
                List<MapData> mapDataList;
                if (!mapDataTypeDatasMap.TryGetValue(mapDataConfig.DataType, out mapDataList))
                {
                    mapDataList = new List<MapData>();
                    mapDataTypeDatasMap.Add(mapDataConfig.DataType, mapDataList);
                }
                mapDataList.Add(mapData);
            }
            return mapDataTypeDatasMap;
        }

        /// <summary>
        /// 导出指定地图数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="exportType"></param>
        public static void ExportGameMapData(Map map)
        {
            if (map == null)
            {
                Debug.LogError($"不允许导出空的Map脚本地图数据，导出地图数据失败！");
                return;
            }
            CheckOrCreateGameMapExportFolder(map.ExportType);
            if (map.ExportType == ExportType.JSON)
            {
                DoExportGameMapJsonData(map);
            }
            else
            {
                Debug.LogError($"不支持的导出类型:{map.ExportType.ToString()}，导出地图数据失败！");
            }
        }

        /// <summary>
        /// 导出指定地图组件的Json格式数据
        /// </summary>
        /// <param name="map"></param>
        private static void DoExportGameMapJsonData(Map map)
        {
            var mapExport = GetMapExport(map);
            var mapDataJsonContent = JsonUtility.ToJson(mapExport, true);
            var fileName = map.gameObject.name;
            var exportFileFullPath = GetGameMapExportFileFullPath(map.ExportType, fileName);
            using (var sw = File.CreateText(exportFileFullPath))
            {
                sw.Write(mapDataJsonContent);
            }
            Debug.Log($"导出地图数据:{exportFileFullPath}成功！");
        }

        /// <summary>
        /// 获取指定Map组件的导出数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static MapExport GetMapExport(Map map)
        {
            MapExport mapExport = new MapExport();
            var mapDataTypeDatasMap = GetMapDataTypeDatas(map.MapDataList);
            mapExport.MapData.Width = map.MapWidth;
            mapExport.MapData.Height = map.MapHeight;
            mapExport.MapData.StartPos = map.MapStartPos;
            mapExport.MapData.GridSize = map.GridSize;

            UpdateMapExportByObjects(mapExport, map.MapObjectDataList);
            UpdateMapExportByMapDatas(mapExport, map.MapDataList);
            return mapExport;
        }

        /// <summary>
        /// 指定地图对象数列表更新地图导出数据
        /// </summary>
        /// <param name="mapExport"></param>
        /// <param name="mapObjectDatas"></param>
        private static void UpdateMapExportByObjects(MapExport mapExport, List<MapObjectData> mapObjectDatas)
        {
            if(mapObjectDatas == null)
            {
                return;
            }
            foreach (var mapObjectData in mapObjectDatas)
            {
                var mapObjectUID = mapObjectData.UID;
                var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
                var isDynamic = MapSetting.GetEditorInstance().ObjectSetting.IsDynamicMapObjectType(mapObjectConfig.ObjectType);
                if (isDynamic)
                {
                    var mapDataExport = GetColliderMapDynamicExport(mapObjectData, mapExport.MapData.GridSize);
                    mapExport.AllColliderMapDynamicExportDatas.Add(mapDataExport);
                }
                else
                {
                    var mapDynamicExport = GetBaseMapDynamicExport(mapObjectData);
                    mapExport.AllBaseMapObjectExportDatas.Add(mapDynamicExport);
                }
            }
        }

        /// <summary>
        /// 指定地图埋点数据列表和模板替换规则数据列表更新地图导出数据
        /// </summary>
        /// <param name="mapExport"></param>
        /// <param name="mapDatas"></param>
        /// <param name="basePosition"></param>
        /// <param name="baseRotation"></param>
        /// <param name="mapTemplateStrategyData"></param>
        private static void UpdateMapExportByMapDatas(MapExport mapExport, List<MapData> mapDatas, Vector3? basePosition = null,
                                                        Vector3? baseRotation = null, MapTemplateStrategyData mapTemplateStrategyData = null)
        {
            if(mapDatas == null)
            {
                return;
            }
            var finalBasePosition = basePosition != null ? (Vector3)basePosition : Vector3.zero;
            var finalBaseRotation = baseRotation != null ? (Vector3)baseRotation : Vector3.zero;
            var mapDataTypeDatasMap = GetMapDataTypeDatas(mapDatas);
            List<MapData> playerSpawnDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.PlayerSpawn, out playerSpawnDatas))
            {
                foreach (var playerSpawnData in playerSpawnDatas)
                {
                    var playerSpawnPosition = finalBasePosition + playerSpawnData.Position;
                    mapExport.MapData.BirthPos.Add(playerSpawnPosition);
                }
            }

            Dictionary<int, MapData> monsterGroupMap = new Dictionary<int, MapData>();
            List<MapData> monsterGroupDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.MonsterGroup, out monsterGroupDatas))
            {
                foreach (var mapData in monsterGroupDatas)
                {
                    var monsterGroupData = mapData as MonsterGroupMapData;
                    var monsterGroupIdReplaceData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetMonsterGroupIdReplaceData(monsterGroupData.GroupId) : null;
                    var monsterGroupId = monsterGroupIdReplaceData != null ? monsterGroupIdReplaceData.NewData : monsterGroupData.GroupId;
                    if (!monsterGroupMap.ContainsKey(monsterGroupId))
                    {
                        monsterGroupMap.Add(monsterGroupId, monsterGroupData);
                    }
                    else
                    {
                        Debug.LogError($"有重复组Id:{monsterGroupId}的怪物组埋点数据，请检查埋点配置！");
                    }
                }
            }

            Dictionary<int, List<MapData>> monsterMap = new Dictionary<int, List<MapData>>();
            List<MapData> monsterDatas;
            List<MapData> noGroupMonsterDatas = new List<MapData>();
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.Monster, out monsterDatas))
            {
                foreach (var mapData in monsterDatas)
                {
                    var monsterData = mapData as MonsterMapData;
                    var monsterGroupIdReplaceData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetMonsterGroupIdReplaceData(monsterData.GroupId) : null;
                    var monsterGroupId = monsterGroupIdReplaceData != null ? monsterGroupIdReplaceData.NewData : monsterData.GroupId;
                    if (!monsterGroupMap.ContainsKey(monsterGroupId))
                    {
                        noGroupMonsterDatas.Add(monsterData);
                    }

                    List<MapData> groupMonsterDatas;
                    if (!monsterMap.TryGetValue(monsterGroupId, out groupMonsterDatas))
                    {
                        groupMonsterDatas = new List<MapData>();
                        monsterMap.Add(monsterGroupId, groupMonsterDatas);
                    }
                    groupMonsterDatas.Add(monsterData);
                }
            }

            foreach (var monsterGroup in monsterGroupMap)
            {
                var monsterGroupMapData = monsterGroup.Value as MonsterGroupMapData;
                var groupId = monsterGroupMapData.GroupId;
                // 因为模板递归判定，指定组id的导出数据可能已经存在了
                // 会导致嵌套的组id相同位置不同，导出不知道应该已谁为准的问题
                // 流程上采用导出时提前检查是否嵌套重复组id的方式避免，导出这里就不检查了
                var monsterGroupMapDataExport = mapExport.GetMonsterGroupMapDataExportByGroupId(groupId);
                if(monsterGroupMapDataExport != null)
                {
                    Debug.LogWarning($"有重复怪物组Id:{groupId}的数据，理论上导出前已经做检查，不应该进入这里，请检查代码！");
                }
                monsterGroupMapDataExport = GetMonsterGroupMapDataExport(monsterGroupMapData, finalBasePosition, finalBaseRotation, mapTemplateStrategyData);
                mapExport.AllMonsterGroupMapDatas.Add(monsterGroupMapDataExport);
                List<MapData> groupMonsterMapDatas;
                if (monsterMap.TryGetValue(groupId, out groupMonsterMapDatas))
                {
                    foreach (var mapData in groupMonsterMapDatas)
                    {
                        var monsterMapDataExport = GetMonsterMapDataExport(mapData, finalBasePosition, finalBaseRotation, mapTemplateStrategyData);
                        monsterGroupMapDataExport.AddMonsterMapExportData(monsterMapDataExport);
                    }
                }
            }

            if (noGroupMonsterDatas != null)
            {
                foreach (var noGroupMonsterData in noGroupMonsterDatas)
                {
                    var monsterMapDataExport = GetMonsterMapDataExport(noGroupMonsterData, finalBasePosition, finalBaseRotation, mapTemplateStrategyData);
                    mapExport.ALlNoGroupMonsterMapDatas.Add(monsterMapDataExport);
                }
            }

            foreach (var mapData in mapDatas)
            {
                var uidReplaceData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetUIDReplaceData(mapData.UID) : null;
                var mapDataUID = uidReplaceData != null ? uidReplaceData.NewData : mapData.UID;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
                if(mapDataConfig == null)
                {
                    continue;
                }
                var mapDataType = mapDataConfig.DataType;
                if (IsOtherMapDataType(mapDataType))
                {
                    continue;
                }
                var mapDataExport = GetBaseMapDataExport(mapData, finalBasePosition, finalBaseRotation, mapTemplateStrategyData);
                mapExport.AllOtherMapDatas.Add(mapDataExport);
            }
            // 模板类型递归支持嵌套模板
            List<MapData> templateMapDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.Template, out templateMapDatas))
            {
                foreach(var templateMapData in templateMapDatas)
                {
                    var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(templateMapData.UID);
                    if(mapDataConfig == null)
                    {
                        Debug.LogError($"找不到模板UID:{templateMapData.UID}的地图数据配置，不参与数据导出！");
                        continue;
                    }
                    var templateDataAsset = mapDataConfig.TemplateDataAsset;
                    if(templateDataAsset == null)
                    {
                        continue;
                    }
                    var realTemplateMapData = templateMapData as TemplateMapData;
                    var strategyTemplateData = templateDataAsset.GetMapTemlateStrategyDataByUID(realTemplateMapData.StrategyUID);
                    // 嵌套模板时位置需要反向剔除模板的参考位置
                    var nestedBasePosition = finalBasePosition + templateMapData.Position - templateDataAsset.TemplateReferencePosition;
                    var nestedBaseRotation = finalBaseRotation + templateMapData.Rotation;
                    UpdateMapExportByMapDatas(mapExport, templateDataAsset.MapDataList, nestedBasePosition, nestedBaseRotation, strategyTemplateData);
                }
            }
        }

        /// <summary>
        /// 获取指定地图对象数据的地图动态对象碰撞体导出数据
        /// </summary>
        /// <param name="mapObjectData"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        private static ColliderMapDynamicExport GetColliderMapDynamicExport(MapObjectData mapObjectData, float gridSize)
        {
            if (mapObjectData == null)
            {
                Debug.LogError("不允许获取空地图对象数据的地图动态对象碰撞体导出数据失败！");
                return null;
            }
            var mapObjectUID = mapObjectData.UID;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，获取地图动态对象数据碰撞体导出数据失败！");
                return null;
            }
            var colliderGridGUIDs = GetGridUIDs(mapObjectData.Position, mapObjectData.Rotation, mapObjectData.ColliderCenter, mapObjectData.ColliderSize, gridSize);
            return new ColliderMapDynamicExport(mapObjectConfig.ObjectType, mapObjectConfig.ConfId, mapObjectData.Position,
                                                mapObjectData.Rotation, mapObjectData.LocalScale, colliderGridGUIDs, mapObjectData.ColliderCenter, mapObjectData.ColliderSize);
        }

        /// <summary>
        /// 获取指定地图对象数据的地图动态对象基础导出数据
        /// </summary>
        /// <param name="mapObjectData"></param>
        /// <returns></returns>
        private static BaseMapDynamicExport GetBaseMapDynamicExport(MapObjectData mapObjectData)
        {
            if (mapObjectData == null)
            {
                Debug.LogError("不允许获取空地图对象数据的地图动态对象基础导出数据失败！");
                return null;
            }
            var mapObjectUID = mapObjectData.UID;
            var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
            if (mapObjectConfig == null)
            {
                Debug.LogError($"找不到地图对象UID:{mapObjectUID}的配置，获取地图动态对象数据基础导出数据失败！");
                return null;
            }
            return new BaseMapDynamicExport(mapObjectConfig.ObjectType, mapObjectConfig.ConfId, mapObjectData.Position, mapObjectData.Rotation, mapObjectData.LocalScale);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点怪物组导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="basePosition"></param>
        /// <param name="baseRotation"></param>
        /// <param name="mapTemplateStrategyData"></param>
        /// <returns></returns>
        private static MonsterGroupMapDataExport GetMonsterGroupMapDataExport(MapData mapData, Vector3? basePosition = null,
                                                                                Vector3? baseRotation = null, MapTemplateStrategyData mapTemplateStrategyData = null)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点怪物组导出数据失败！");
                return null;
            }
            var replaceUIDData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetUIDReplaceData(mapData.UID) : null;
            var mapDataUID = replaceUIDData != null ? replaceUIDData.NewData : mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点怪物组导出数据失败！");
                return null;
            }
            var finalBasePosition = basePosition != null ? (Vector3)basePosition : Vector3.zero;
            var finalBaseRotation= baseRotation != null ? (Vector3)baseRotation : Vector3.zero;
            var monsterGroupMapData = mapData as MonsterGroupMapData;
            var replaceGroupIdData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetMonsterGroupIdReplaceData(monsterGroupMapData.GroupId) : null;
            var groupId = replaceGroupIdData != null ? replaceGroupIdData.NewData : monsterGroupMapData.GroupId;
            var finalPosition = finalBasePosition + mapData.Position;
            var finalRotation = finalBaseRotation + mapData.Rotation;
            return new MonsterGroupMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, finalPosition, finalRotation,
                                                    groupId, monsterGroupMapData.MonsterCreateRadius, monsterGroupMapData.MonsterActiveRadius);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点怪物导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="basePosition"></param>
        /// <param name="baseRotation"></param>
        /// <param name="mapTemplateStrategyData"></param>
        /// <returns></returns>
        private static MonsterMapDataExport GetMonsterMapDataExport(MapData mapData, Vector3? basePosition = null,
                                                                    Vector3? baseRotation = null, MapTemplateStrategyData mapTemplateStrategyData = null)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点怪物导出数据失败！");
                return null;
            }
            var replaceUIDData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetUIDReplaceData(mapData.UID) : null;
            var mapDataUID = replaceUIDData != null ? replaceUIDData.NewData : mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点怪物导出数据失败！");
                return null;
            }
            var finalBasePosition = basePosition != null ? (Vector3)basePosition : Vector3.zero;
            var finalBaseRotation = baseRotation != null ? (Vector3)baseRotation : Vector3.zero;
            var monsterMapData = mapData as MonsterMapData;
            var replaceGroupIdData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetMonsterGroupIdReplaceData(monsterMapData.GroupId) : null;
            var groupId = replaceGroupIdData != null ? replaceGroupIdData.NewData : monsterMapData.GroupId;
            var finalPosition = finalBasePosition + mapData.Position;
            var finalRotation = finalBaseRotation + mapData.Rotation;
            return new MonsterMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, finalPosition, finalRotation, groupId);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点基础导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <param name="basePosition"></param>
        /// <param name="baseRotation"></param>
        /// <param name="mapTemplateStrategyData"></param>
        /// <returns></returns>
        private static BaseMapDataExport GetBaseMapDataExport(MapData mapData, Vector3? basePosition = null,
                                                                Vector3? baseRotation = null, MapTemplateStrategyData mapTemplateStrategyData = null)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点基础导出数据失败！");
                return null;
            }
            var replaceUIDData = mapTemplateStrategyData != null ? mapTemplateStrategyData.GetUIDReplaceData(mapData.UID) : null;
            var mapDataUID = replaceUIDData != null ? replaceUIDData.NewData : mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点基础导出数据失败！");
                return null;
            }
            var finalBasePosition = basePosition != null ? (Vector3)basePosition : Vector3.zero;
            var finalBaseRotation = baseRotation != null ? (Vector3)baseRotation : Vector3.zero;
            var finalPosition = finalBasePosition + mapData.Position;
            var finalRotation = finalBaseRotation + mapData.Rotation;
            return new BaseMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, mapData.Position, mapData.Rotation);
        }

        /// <summary>
        /// 获取指定位置的九宫格的X和Z
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static KeyValuePair<int, int> GetGridXZByPosition(Vector3 position, float gridSize)
        {
            // 不考虑地图起点偏移，默认参考0,0点计算
            var gridX = position.x >= 0 ? Mathf.FloorToInt(position.x / gridSize) : -Mathf.CeilToInt(-position.x / gridSize);
            var gridZ = position.z >= 0 ? Mathf.FloorToInt(position.z / gridSize) : -Mathf.CeilToInt(-position.z / gridSize);
            return new KeyValuePair<int, int>(gridX, gridZ);
        }

        /// <summary>
        /// 获取指定位置的九宫格UID
        /// </summary>
        /// <param name="position"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static int GetGridUIDByPosition(Vector3 position, float gridSize)
        {
            var gridXZ = GetGridXZByPosition(position, gridSize);
            return GetGridUID(gridXZ.Key, gridXZ.Value);
        }

        /// <summary>
        /// 获取指定X和Z的九宫格UID
        /// </summary>
        /// <param name="gridX"></param>
        /// <param name="gridZ"></param>
        /// <returns></returns>
        public static int GetGridUID(int gridX, int gridZ)
        {
            return gridX + gridZ * 10000;
        }

        /// <summary>
        /// 获取指定数据所占据的UID列表
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="colliderCenter"></param>
        /// <param name="colliderSize"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        public static List<int> GetGridUIDs(Vector3 position, Vector3 rotation, Vector3 colliderCenter, Vector3 colliderSize, float gridSize)
        {
            // 目前当做2D考虑，只考虑Y轴旋转
            var gridUIDs = new List<int>();
            var colCenter = position + colliderCenter;
            var halfColliderSize = colliderSize / 2;
            var boxBLPos = new Vector3(-halfColliderSize.x, 0, -halfColliderSize.z);
            var boxTLPos = new Vector3(-halfColliderSize.x, 0, halfColliderSize.z);
            var boxTRPos = new Vector3(-halfColliderSize.x, 0, halfColliderSize.z);
            var boxBRPos = new Vector3(-halfColliderSize.x, 0, -halfColliderSize.z);
            var rotationQuaterion = Quaternion.AngleAxis(rotation.y, Vector3.up);
            boxBLPos = rotationQuaterion * boxBLPos + colCenter;
            boxTLPos = rotationQuaterion * boxTLPos + colCenter;
            boxTRPos = rotationQuaterion * boxTRPos + colCenter;
            boxBRPos = rotationQuaterion * boxBRPos + colCenter;
            var boxBLGridXZ = GetGridXZByPosition(boxBLPos, gridSize);
            var boxTLGridXZ = GetGridXZByPosition(boxTLPos, gridSize);
            var boxTRGridXZ = GetGridXZByPosition(boxTRPos, gridSize);
            var boxBRGridXZ = GetGridXZByPosition(boxBRPos, gridSize);
            var boxMinGridX = Mathf.Min(boxBLGridXZ.Key, boxTLGridXZ.Key, boxTRGridXZ.Key, boxBRGridXZ.Key);
            var boxMaxGridX = Mathf.Max(boxBLGridXZ.Key, boxTLGridXZ.Key, boxTRGridXZ.Key, boxBRGridXZ.Key);
            var boxMinGridZ = Mathf.Min(boxBLGridXZ.Value, boxTLGridXZ.Value, boxTRGridXZ.Value, boxBRGridXZ.Value);
            var boxMaxGridZ = Mathf.Max(boxBLGridXZ.Value, boxTLGridXZ.Value, boxTRGridXZ.Value, boxBRGridXZ.Value);
            for (int gridX = boxMinGridX; gridX <= boxMaxGridX; gridX++)
            {
                for (int gridZ = boxMinGridZ; gridZ <= boxMaxGridZ; gridZ++)
                {
                    var gridUID = GetGridUID(gridX, gridZ);
                    if (!gridUIDs.Contains(gridUID))
                    {
                        gridUIDs.Add(gridUID);
                    }
                }
            }
            return gridUIDs;
        }

        /// <summary>
        /// 自定地图埋点类型是否归属其他地图埋点类型
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private static bool IsOtherMapDataType(MapDataType mapDataType)
        {
            return mapDataType == MapDataType.PlayerSpawn || mapDataType == MapDataType.Monster
                    || mapDataType == MapDataType.MonsterGroup || mapDataType == MapDataType.Template;
        }
    }
}
