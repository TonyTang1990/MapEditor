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
        public static bool ExportGameMapData(Map map)
        {
            if(!CheckIsValideToExport(map))
            {
                Debug.LogError($"地图预制件:{map.gameObject.name}不符合导出条件，地图数据导出失败！");
                return false;
            }
            CheckOrCreateGameMapExportFolder(map.ExportType);
            var exportFileName = MapEditorUtilities.GetMapExportFileName(map);
            if (map.ExportType == ExportType.JSON)
            {
                DoExportGameMapJsonData(map, exportFileName);
                return true;
            }
            else
            {
                Debug.LogError($"不支持的导出类型:{map.ExportType.ToString()}，导出地图数据失败！");
                return false;
            }
        }

        /// <summary>
        /// 指定Map脚本检查是否符合导出条件
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool CheckIsValideToExport(Map map)
        {
            if (map == null)
            {
                Debug.LogError($"不允许导出空的Map脚本地图数据，导出地图数据失败！");
                return false;
            }
            if(CheckHasInvalideMapDataUID(map))
            {
                return false;
            }
            if (CheckHasInvalideMapObjectUID(map))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查Map脚本是否有无效埋点类型数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool CheckHasInvalideMapDataUID(Map map)
        {
            if (map == null)
            {
                return false;
            }
            return HasInvalideUIDByDatas(map.MapDataList);
        }

        /// <summary>
        /// 指定地图埋点数据列表和地图模板策略数据是否有无效UID配置
        /// </summary>
        /// <param name="mapDatas"></param>
        /// <returns></returns>
        private static bool HasInvalideUIDByDatas(List<MapData> mapDatas)
        {
            if (mapDatas == null)
            {
                return false;
            }
            MapDataConfig mapDataConfig;
            foreach (var mapData in mapDatas)
            {
                mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapData.UID);
                if (mapDataConfig == null)
                {
                    Debug.LogError($"地图埋点数据有配置不支持的地图埋点UID:{mapData.UID}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查Map脚本是否有无效地图对象UID数据
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        private static bool CheckHasInvalideMapObjectUID(Map map)
        {
            if (map == null)
            {
                return false;
            }
            MapObjectConfig mapObjectConfig;
            foreach (var mapObjectData in map.MapObjectDataList)
            {
                mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectData.UID);
                if (mapObjectConfig == null)
                {
                    Debug.LogError($"地图对象数据有配置不支持的地图对象UID:{mapObjectData.UID}");
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 导出指定地图组件的Json格式数据
        /// </summary>
        /// <param name="map"></param>
        /// <param name="exportFileName"></param>
        private static void DoExportGameMapJsonData(Map map, string exportFileName)
        {
            var mapExport = GetMapExport(map);
            var mapDataJsonContent = JsonUtility.ToJson(mapExport, true);
            var exportFileFullPath = GetGameMapExportFileFullPath(map.ExportType, exportFileName);
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
            mapExport.MapData.Width = map.MapWidth;
            mapExport.MapData.Height = map.MapHeight;
            mapExport.MapData.StartPos = map.MapStartPos;

            //UpdateMapExportByObjects(mapExport, map.MapObjectDataList);
            UpdateMapExportByMapDatas(mapExport, map.MapDataList);
            return mapExport;
        }

        /// TODO:
        /// 未来大地图场景对象走逻辑创建时才需要
        /// <summary>
        /// 指定地图对象数列表更新地图导出数据
        /// </summary>
        /// <param name="mapExport"></param>
        /// <param name="mapObjectDatas"></param>
        // private static void UpdateMapExportByObjects(MapExport mapExport, List<MapObjectData> mapObjectDatas)
        // {
        //     if(mapObjectDatas == null)
        //     {
        //         return;
        //     }
        //     foreach (var mapObjectData in mapObjectDatas)
        //     {
        //         var mapObjectUID = mapObjectData.UID;
        //         var mapObjectConfig = MapSetting.GetEditorInstance().ObjectSetting.GetMapObjectConfigByUID(mapObjectUID);
        //         if(mapObjectConfig == null)
        //         {
        //             Debug.LogError($"找不到UID:{mapObjectUID}的地图对象UID配置，更新地图对象导出数据失败！");
        //             continue;
        //         }
        //         var mapDynamicExport = GetMapObjectExport(mapObjectData, mapExport.MapData.GridSize);
        //         mapExport.AllMapObjectExportDatas.Add(mapDynamicExport);
        //     }
        // }

        /// <summary>
        /// 指定地图埋点数据列表更新地图导出数据
        /// </summary>
        /// <param name="mapExport"></param>
        /// <param name="mapDatas"></param>
        private static void UpdateMapExportByMapDatas(MapExport mapExport, List<MapData> mapDatas)
        {
            if(mapDatas == null)
            {
                return;
            }
            var mapDataTypeDatasMap = GetMapDataTypeDatas(mapDatas);
            List<MapData> playerSpawnDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.PlayerSpawn, out playerSpawnDatas))
            {
                foreach (var playerSpawnData in playerSpawnDatas)
                {
                    mapExport.MapData.BirthPos.Add(playerSpawnData.Position);
                }
            }

            List<MapData> monsterDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.Monster, out monsterDatas))
            {
                foreach (var monsterData in monsterDatas)
                {
                    var monsterMapDataExport = GetMonsterMapDataExport(monsterData);
                    mapExport.ALlMonsterMapDatas.Add(monsterMapDataExport);
                }
            }

            List<MapData> treasureBoxDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.TreasureBox, out treasureBoxDatas))
            {
                foreach (var treasureData in treasureBoxDatas)
                {
                    var treasureBoxMapDataExport = GetTreasureBoxMapDataExport(treasureData);
                    mapExport.AllTreasureBoxMapDatas.Add(treasureBoxMapDataExport);
                }
            }
            
            List<MapData> trapDatas;
            if (mapDataTypeDatasMap.TryGetValue(MapDataType.Trap, out trapDatas))
            {
                foreach (var trapData in trapDatas)
                {
                    var trapMapDataExport = GetTrapMapDataExport(trapData);
                    mapExport.AllTrapMapDatas.Add(trapMapDataExport);
                }
            }

            foreach (var mapData in mapDatas)
            {
                var mapDataUID = mapData.UID;
                var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
                if(mapDataConfig == null)
                {
                    continue;
                }
                var mapDataType = mapDataConfig.DataType;
                if (!IsOtherMapDataType(mapDataType))
                {
                    continue;
                }
                var mapDataExport = GetBaseMapDataExport(mapData);
                mapExport.AllOtherMapDatas.Add(mapDataExport);
            }
        }

        /// <summary>
        /// 获取指定地图对象数据的地图动态对象基础导出数据
        /// </summary>
        /// <param name="mapObjectData"></param>
        /// <param name="gridSize"></param>
        /// <returns></returns>
        private static MapObjectExport GetMapObjectExport(MapObjectData mapObjectData, float gridSize)
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
            return new MapObjectExport(mapObjectConfig.ObjectType, mapObjectConfig.ConfId, mapObjectData.Position, mapObjectData.Rotation, mapObjectData.LocalScale);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点怪物导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static MonsterMapDataExport GetMonsterMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点怪物导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点怪物导出数据失败！");
                return null;
            }
            var monsterMapData = mapData as MonsterMapData;
            var monsterCreateRadius = monsterMapData.MonsterCreateRadius;
            var monsterActiveRadius = monsterMapData.MonsterActiveRadius;
            return new MonsterMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, mapData.Position, mapData.Rotation, monsterCreateRadius, monsterActiveRadius);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点宝箱导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static TreasureBoxMapDataExport GetTreasureBoxMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点宝箱导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点宝箱导出数据失败！");
                return null;
            }
            var treasureBoxMapData = mapData as TreasureBoxMapData;
            return new TreasureBoxMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, treasureBoxMapData.Position, treasureBoxMapData.Rotation);
        }
        
        /// <summary>
        /// 获取指定地图埋点数据的地图埋点陷阱导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static TrapMapDataExport GetTrapMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点陷阱导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点陷阱导出数据失败！");
                return null;
            }
            var trapMapData = mapData as TrapMapData;
            return new TrapMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, trapMapData.Position, trapMapData.Rotation);
        }

        /// <summary>
        /// 获取指定地图埋点数据的地图埋点基础导出数据
        /// </summary>
        /// <param name="mapData"></param>
        /// <returns></returns>
        private static BaseMapDataExport GetBaseMapDataExport(MapData mapData)
        {
            if (mapData == null)
            {
                Debug.LogError("不允许获取空地图埋点数据的地图埋点基础导出数据失败！");
                return null;
            }
            var mapDataUID = mapData.UID;
            var mapDataConfig = MapSetting.GetEditorInstance().DataSetting.GetMapDataConfigByUID(mapDataUID);
            if (mapDataConfig == null)
            {
                Debug.LogError($"找不到地图埋点UID:{mapDataUID}的配置，获取地图埋点基础导出数据失败！");
                return null;
            }
            return new BaseMapDataExport(mapDataConfig.DataType, mapDataConfig.ConfId, mapData.Position, mapData.Rotation);
        }

        /// <summary>
        /// 自定地图埋点类型是否归属其他地图埋点类型
        /// </summary>
        /// <param name="mapDataType"></param>
        /// <returns></returns>
        private static bool IsOtherMapDataType(MapDataType mapDataType)
        {
            return false;
        }
    }
}
