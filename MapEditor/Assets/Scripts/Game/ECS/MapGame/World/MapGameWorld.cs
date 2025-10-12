/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using UnityEngine;

/// <summary>
/// MapGameWorld.cs
/// 地图游戏世界
/// </summary>
public class MapGameWorld : BaseWorld
{
    public MapGameWorld() : base()
    {

    }

    /// <summary>
    /// 响应地图游戏世界创建
    /// </summary>
    public override void OnCreate()
    {
        base.OnCreate();
        CreateAllSystem();
    }

    /// <summary>
    /// 响应地图游戏世界销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
    }

    /// <summary>
    /// 创建所有系统
    /// </summary>
    private void CreateAllSystem()
    {
        CreateSystem<MapGameEntitySpawnSystem>();
        CreateSystem<InputControlSystem>();
        CreateSystem<PlayerSpawnSystem>();
        CreateSystem<MapObjectEntitySpawnSystem>();
        CreateSystem<CameraFollowSystem>();
    }

    /// <summary>
    /// 创建MapGameEntity
    /// </summary>
    /// <returns></returns>
    public MapGameEntity CreateMapGameEntity()
    {
        var mapGameEntity = CreateEntity<MapGameEntity>();
        MapGameEntityUtilities.InitEntityComponents(mapGameEntity);
        mapGameEntity.Init();
        return mapGameEntity;
    }

    /// <summary>
    /// 创建CameraEntity
    /// </summary>
    /// <param name="bindCameraGo"></param>
    /// <param name="isAutoDestroy"></param>
    /// <returns></returns>
    public CameraEntity CreateCameraEntity(GameObject bindCameraGo, bool isAutoDestroy = false)
    {
        var cameraEntity = CreateEntity<CameraEntity>();
        MapGameEntityUtilities.InitEntityComponents(cameraEntity);
        cameraEntity.Init(bindCameraGo, isAutoDestroy);
        return cameraEntity;
    }

    /// <summary>
    /// 创建PlayerEntity
    /// </summary>
    /// <returns></returns>
    public PlayerEntity CreatePlayerEntity(string prefabPath, Vector3 worldPos, Vector3 worldRotation)
    {
        var playerEntity = CreateEntity<PlayerEntity>();
        MapGameEntityUtilities.InitEntityComponents(playerEntity);
        playerEntity.Init(prefabPath);
        var entityParent = GetEntityTypeParent(playerEntity);
        MapGameManager.Singleton.LoadObjectEntityEmptyNavPrefab(playerEntity, entityParent, worldPos, worldRotation);
        return playerEntity;
    }

    /// <summary>
    /// 创建MonsterEntity
    /// </summary>
    /// <returns></returns>
    public MonsterEntity CreateMonsterEntity(string prefabPath, Vector3 worldPos)
    {
        var monsterEntity = CreateEntity<MonsterEntity>();
        MapGameEntityUtilities.InitEntityComponents(monsterEntity);
        monsterEntity.Init(prefabPath);
        var entityParent = GetEntityTypeParent(monsterEntity);
        MapGameManager.Singleton.LoadObjectEntityEmptyNavPrefab(monsterEntity, entityParent, worldPos, Vector3.zero);
        return monsterEntity;
    }

    /// <summary>
    /// 创建TreasureBoxEntity
    /// </summary>
    /// <returns></returns>
    public TreasureBoxEntity CreateTreasureBoxEntity(string prefabPath, Vector3 worldPos)
    {
        var treasureBoxEntity = CreateEntity<TreasureBoxEntity>();
        MapGameEntityUtilities.InitEntityComponents(treasureBoxEntity);
        treasureBoxEntity.Init(prefabPath);
        var entityParent = GetEntityTypeParent(treasureBoxEntity);
        ECSManager.Singleton.LoadObjectEntityEmptyPrefab(treasureBoxEntity, entityParent, worldPos, Vector3.zero);
        return treasureBoxEntity;
    }


    /// <summary>
    /// 创建TrapEntity
    /// </summary>
    /// <returns></returns>
    public TrapEntity CreateTrapEntity(string prefabPath, Vector3 worldPos)
    {
        var trapEntity = CreateEntity<TrapEntity>();
        MapGameEntityUtilities.InitEntityComponents(trapEntity);
        trapEntity.Init(prefabPath);
        var entityParent = GetEntityTypeParent(trapEntity);
        ECSManager.Singleton.LoadObjectEntityEmptyPrefab(trapEntity, entityParent, worldPos, Vector3.zero);
        return trapEntity;
    }
}