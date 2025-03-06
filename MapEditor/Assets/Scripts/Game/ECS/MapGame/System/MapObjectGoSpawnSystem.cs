/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using System;
using System.Collections.Generic;

/// <summary>
/// MapObjectGoSpawnSystem.cs
/// 地图对象GameObject生成系统
/// </summary>
public class MapObjectGoSpawnSystem : BaseSystem
{
    /// <summary>
    /// Entity过滤
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public override bool Filter(BaseEntity entity)
    {
        if(entity is BaseActorEntity)
        {
            return true;
        }
        return false;
    }

    public override void OnAdd(BaseEntity entity)
    {
        base.OnAdd(entity);
        var actorEntity = entity as BaseActorEntity;
        var prefabPath = actorEntity.PrefabPath;
        var entityType = entity.EntityType;
        var parent = OwnerWorld.GetEntityTypeParent(entityType);
        MapGameManager.Singleton.LoadEntityPrefabByPath(actorEntity, prefabPath, parent);
    }

    /// <summary>
    /// 响应Entity移除
    /// </summary>
    /// <param name="entity"></param>
    public override void OnRemove(BaseEntity entity)
    {
        base.OnRemove(entity);
        var actorEntity = entity as BaseActorEntity;
        actorEntity?.DestroyInstance();
    }
}
