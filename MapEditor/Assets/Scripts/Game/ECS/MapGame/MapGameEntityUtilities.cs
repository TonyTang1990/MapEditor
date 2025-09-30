/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-09-30 16:00:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-09-30 16:00:00
 * @ Description:
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.Serialization;

/// <summary>
/// MapGameEntityUtilities.cs
/// 地图游戏Entity工具类
/// </summary>
public static class MapGameEntityUtilities
{
    /// <summary>
    /// 创建EntityType组件
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static EntityTypeComponent CreateEntityTypeComponent(EntityType entityType)
    {
        var entityTypeComponent = ObjectPool.Singleton.Pop<EntityTypeComponent>();
        entityTypeComponent.EntityType = entityType;
        return entityTypeComponent;
    }

    /// <summary>
    /// 初始化Entity组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static bool InitEntityComponents<T>(T entity, params object[] parameters) where T : BaseEntity
    {
        if (entity == null)
        {
            Debug.LogError($"不允许初始化空Entity组件！");
            return false;
        }
        var entityClassType = entity.ClassType;
        if (entityClassType == ECSConst.PlayerEntityType)
        {
            InitPlayerEntityComponents(entity as PlayerEntity, parameters);
        }
        else if (entityClassType == ECSConst.MonsterEntityType)
        {
            InitMonsterEntityComponents(entity as MonsterEntity, parameters);
        }
        else if (entityClassType == ECSConst.TreasureBoxEntityType)
        {
            InitTreasureBoxEntityComponents(entity as TreasureBoxEntity, parameters);
        }
        else if (entityClassType == ECSConst.TrapEntityType)
        {
            InitTrapEntityComponents(entity as TrapEntity, parameters);
        }
        else if (entityClassType == ECSConst.CameraEntityType)
        {
            InitCameraEntityComponents(entity as CameraEntity, parameters);
        }
        else if (entityClassType == ECSConst.MapGameEntityType)
        {
            InitMapGameEntityComponents(entity as MapGameEntity, parameters);
        }
        return true;
    }

    /// <summary>
    /// 初始化角色Entity通用组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityType"></param>
    private static void InitActorEntityCommonComponents(BaseEntity entity, EntityType entityType)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(entityType);
        entity.AddComponents(entityTypeComponent);
    }

    /// <summary>
    /// 初始化绑定Entity通用组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityType"></param>
    /// <param name="bindGo"></param>
    /// <param name="IsAutoDestroyBindGo"></param>
    private static void InitBindEntityCommonComponents(BaseEntity entity, EntityType entityType)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(entityType);
        entity.AddComponents(entityTypeComponent);
    }

    /// <summary>
    /// 初始化PlayerEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitPlayerEntityComponents(PlayerEntity entity, params object[] parameters)
    {
        InitActorEntityCommonComponents(entity, EntityType.Player);
    }

    /// <summary>
    /// 初始化MonsterEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitMonsterEntityComponents(MonsterEntity entity, params object[] parameters)
    {
        var prefabPath = parameters[0] as string;
        InitActorEntityCommonComponents(entity, EntityType.Monster, prefabPath);
    }

    /// <summary>
    /// 初始化TreasureBoxEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitTreasureBoxEntityComponents(TreasureBoxEntity entity, params object[] parameters)
    {
        InitActorEntityCommonComponents(entity, EntityType.TreasureBox);
    }

    /// <summary>
    /// 初始化TrapEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitTrapEntityComponents(TrapEntity entity, params object[] parameters)
    {
        InitActorEntityCommonComponents(entity, EntityType.Trap);
    }

    /// <summary>
    /// 初始化CameraEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitCameraEntityComponents(CameraEntity entity, params object[] parameters)
    {
        InitBindEntityCommonComponents(entity, EntityType.Camera);
    }

    /// <summary>
    /// 初始化MapGameEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitMapGameEntityComponents(MapGameEntity entity, params object[] parameters)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(EntityType.MapGame);
        entity.AddComponents(entityTypeComponent);
    }
}