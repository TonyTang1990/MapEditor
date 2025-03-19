/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using MapEditor;
using System;
using System.Collections.Generic;
using TH.Module.Collision2D;
using UnityEngine;

/// <summary>
/// MapObjectEntitySpawnSystem.cs
/// 地图对象Entity生成系统
/// </summary>
public class MapObjectEntitySpawnSystem : BaseSystem
{
    /// <summary>
    /// 摄像机指定平面映射区域顶点数据列表
    /// </summary>
    private List<Vector3> mAreaPointsList = new List<Vector3>();

    /// <summary>
    /// 摄像机指定平面映射矩形区域顶点数据列表
    /// </summary>
    private List<Vector3> mRectAreaPointsList = new List<Vector3>();

    /// <summary>
    /// 已生成的BaseMapDataExport数据和对应Entity Uuid Map
    /// </summary>
    private Dictionary<BaseMapDataExport, int> mSpawnedMapDataEntityMap = new Dictionary<BaseMapDataExport, int>();

    /// <summary>
    /// 临时需要移除的地图埋点导出数据列表
    /// </summary>
    private List<BaseMapDataExport> mTempRemoveSpawnedMapDataExportList = new List<BaseMapDataExport>();

    /// <summary>
    /// 摄像机矩形区域
    /// </summary>
    private AABB2D mCameraRectArea = new AABB2D();

    /// <summary>
    /// 地图埋点临时Vector2位置数据
    /// </summary>
    private Vector2 mTempMapDataExportPos = new Vector2();

    /// <summary>
    /// 指定MapDataExport数据是否已生成Entity
    /// </summary>
    /// <param name="mapDataExport"></param>
    /// <returns></returns>
    private bool IsMapDataSpawned(BaseMapDataExport mapDataExport)
    {
        return mSpawnedMapDataEntityMap.ContainsKey(mapDataExport);
    }

    /// <summary>
    /// 添加指定地图埋点数据和对应生成Entity Uuid
    /// </summary>
    /// <param name="mapDataExport"></param>
    /// <param name="uuid"></param>
    /// <returns></returns>
    private bool AddMapDataSpawnedEntityUuid(BaseMapDataExport mapDataExport, int uuid)
    {
        if(IsMapDataSpawned(mapDataExport))
        {
            Debug.LogError($"MapDataExport.MapDataType:{mapDataExport.MapDataType},MapDataExport.ConfId:{mapDataExport.ConfId}已生成Entity，添加生成Entity Uuid:{uuid}失败！");
            return false;
        }
        mSpawnedMapDataEntityMap.Add(mapDataExport, uuid);
        Debug.Log($"添加MapDataExport.MapDataType:{mapDataExport.MapDataType},MapDataExport.ConfId:{mapDataExport.ConfId}的生成Entity Uuid:{uuid}成功！");
        return true;
    }

    /// <summary>
    /// 移除指定地图埋点数据的Entity生成数据
    /// </summary>
    /// <param name="mapDataExport"></param>
    /// <returns></returns>
    private bool RemoveMapDataSpawned(BaseMapDataExport mapDataExport)
    {
        int uuid;
        if(mSpawnedMapDataEntityMap.TryGetValue(mapDataExport, out uuid))
        {
            mSpawnedMapDataEntityMap.Remove(mapDataExport);
            Debug.Log($"移除MapDataExport.MapDataType:{mapDataExport.MapDataType},MapDataExport.ConfId:{mapDataExport.ConfId}的生成Entity Uuid:{uuid}成功！");
            return true;
        }
        Debug.LogError($"MapDataExport.MapDataType:{mapDataExport.MapDataType},MapDataExport.ConfId:{mapDataExport.ConfId}未生成对应Entity，移除Entity Uuid:{uuid}失败！");
        return false;
    }

    /// <summary>
    /// Entity过滤
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public override bool Filter(BaseEntity entity)
    {
        var entityTypeComponent = entity.GetComponent<EntityTypeComponent>();
        if(entityTypeComponent == null)
        {
            return false;
        }
        var entityType = entityTypeComponent.EntityType;
        return entityType == EntityType.Camera;
    }

    /// <summary>
    /// Entity处理
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deltaTime"></param>
    public override void Process(BaseEntity entity, float deltaTime)
    {
        base.Process(entity, deltaTime);
        var gameObjectSyncComponent = entity.GetComponent<GameObjectSyncComponent>();
        if(gameObjectSyncComponent == null || !gameObjectSyncComponent.SyncPosition)
        {
            return;
        }
        UpdateCameraDatas();
        CheckAllMapDataExportEntitySpawn();
        CheckAllSpawnEntityRemove();
    }

    /// <summary>
    /// 更新摄像机数据
    /// </summary>
    private void UpdateCameraDatas()
    {
        var mainCamera = MapGameManager.Singleton.MainCamera;
        CameraUtilities.GetCameraVisibleArea(mainCamera, MapGameConst.AreaPoint, MapGameConst.AreaNormal, ref mAreaPointsList, ref mRectAreaPointsList);

        var minX = Math.Min(mRectAreaPointsList[0].x, mRectAreaPointsList[1].x);
        var maxX = Math.Max(mRectAreaPointsList[2].x, mRectAreaPointsList[3].x);
        var minZ = Math.Min(mRectAreaPointsList[0].z, mRectAreaPointsList[3].z);
        var maxZ = Math.Max(mRectAreaPointsList[1].z, mRectAreaPointsList[2].z);
        var width = maxX - minX;
        var height = maxZ - minZ;
        var centerX = mRectAreaPointsList[1].x + width / 2;
        var centerZ = mRectAreaPointsList[0].z + height / 2;
        mCameraRectArea.Center.x = centerX;
        mCameraRectArea.Center.y = centerZ;
        mCameraRectArea.Extents.x = width;
        mCameraRectArea.Extents.y = height;
    }

    /// <summary>
    /// 检查地图埋点导出数据Entity生成
    /// </summary>
    private void CheckAllMapDataExportEntitySpawn()
    {
        var allMonsterMapDatas = MapGameManager.Singleton.LevelConfig.ALlMonsterMapDatas;
        var allTreasureBoxMapDatas = MapGameManager.Singleton.LevelConfig.AllTreasureBoxMapDatas;
        var allTrapMapDatas = MapGameManager.Singleton.LevelConfig.AllTrapMapDatas;
        foreach (var monsterMapDataExport in allMonsterMapDatas)
        {
            CheckAllMapDataExportEntitySpawn(monsterMapDataExport);
        }
        foreach (var treasureBoxMapDataExport in allTreasureBoxMapDatas)
        {
            CheckAllMapDataExportEntitySpawn(treasureBoxMapDataExport);
        }
        foreach (var trapMapDataExport in allTrapMapDatas)
        {
            CheckAllMapDataExportEntitySpawn(trapMapDataExport);
        }
    }

    /// <summary>
    /// 检查指定地图埋点导出数据Entity生成
    /// </summary>
    /// <param name="mapDataExport"></param>
    private void CheckAllMapDataExportEntitySpawn(BaseMapDataExport mapDataExport)
    {
        if(IsMapDataSpawned(mapDataExport))
        {
            return;
        }
        var mapDataExportPosition = mapDataExport.Position;
        mTempMapDataExportPos.x = mapDataExportPosition.x;
        mTempMapDataExportPos.y = mapDataExportPosition.z;
        if(Collision2DUtilities.PointInAABB(mCameraRectArea, mTempMapDataExportPos))
        {
            BaseEntity entity = null;
            var position = mapDataExport.Position;
            if(mapDataExport is MonsterMapDataExport)
            {
                var monsterEntity = OwnerWorld.CreateEntity<MonsterEntity>(MapGameConst.MonsterPrefabPath);
                entity = monsterEntity;
                EntityUtilities.SetPositionOnNav(monsterEntity, position.x, position.y, position.z);
                Debug.Log($"生成在位置:x:{position.x}, y:{position.y}, z:{position.z}生成MonsterEntity！");
            }
            else if(mapDataExport is TreasureBoxMapDataExport)
            {
                var treasureEntity = OwnerWorld.CreateEntity<TreasureBoxEntity>(MapGameConst.TreasureBoxPrefabPath);
                entity = treasureEntity;
                EntityUtilities.SetPositionOnNav(treasureEntity, position.x, position.y, position.z);
                Debug.Log($"生成在位置:x:{position.x}, y:{position.y}, z:{position.z}生成TreasureBoxEntity！");
            }
            else if(mapDataExport is TrapMapDataExport)
            {
                var trapEntity = OwnerWorld.CreateEntity<TrapEntity>(MapGameConst.TrapPrefabPath);
                entity = trapEntity;
                EntityUtilities.SetPositionOnNav(trapEntity, position.x, position.y, position.z);
                Debug.Log($"生成在位置:x:{position.x}, y:{position.y}, z:{position.z}生成TrapEntity！");
            }
            else
            {
                Debug.LogError($"不支持的MapDataExport类型:{mapDataExport.GetType().Name},检测MapDataEntity创建失败！");
            }
            if(entity != null)
            {
                AddMapDataSpawnedEntityUuid(mapDataExport, entity.Uuid);
            }
        }
    }

    /// <summary>
    /// 检查已经生成的Entity回收
    /// </summary>
    private void CheckAllSpawnEntityRemove()
    {
        mTempRemoveSpawnedMapDataExportList.Clear();
        foreach(var spawnedMapDataEntity in mSpawnedMapDataEntityMap)
        {
            var entityUuid = spawnedMapDataEntity.Value;
            var entity = OwnerWorld.GetEntityByUuid<BaseEntity>(entityUuid);
            if (entity == null)
            {
                continue;
            }
            var positionComponent = entity.GetComponent<PositionComponent>();
            var entityPosition = positionComponent.Position;
            mTempMapDataExportPos.x = entityPosition.x;
            mTempMapDataExportPos.y = entityPosition.z;
            if(!Collision2DUtilities.PointInAABB(mCameraRectArea, mTempMapDataExportPos))
            {
                mTempRemoveSpawnedMapDataExportList.Add(spawnedMapDataEntity.Key);
                Debug.Log($"移除在位置:x:{entityPosition.x}, y:{entityPosition.y}, z:{entityPosition.z}的Entity Uuid:{entityUuid}的Entity！");
                OwnerWorld.DestroyEntityByUuid(entityUuid);
            }
        }
        foreach(var removeSpawnMapDataExport in mTempRemoveSpawnedMapDataExportList)
        {
            RemoveMapDataSpawned(removeSpawnMapDataExport);
        }
    }
}
