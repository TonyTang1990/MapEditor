/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:13
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:53
 * @ Description:
 */

using System;
using System.Collections.Generic;

/// <summary>
/// ActorSyncSystem.cs
/// 角色同步系统
/// </summary>
public class ActorSyncSystem : BaseSystem
{
    /// <summary>
    /// Entity过滤
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public override bool Filter(BaseEntity entity)
    {
        if(entity is BaseActorEntity || entity is BaseBindEntity)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// Entity Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void Process(BaseEntity entity, float deltaTime)
    {
        base.Process(entity, deltaTime);
        var actorEntity = entity as BaseActorEntity;
        if (actorEntity != null)
        {
            if (actorEntity.SyncPosition && actorEntity.Go != null)
            {
                actorEntity.Go.transform.position = actorEntity.Position;
                actorEntity.SyncPosition = false;
            }
            if (actorEntity.SyncRotation && actorEntity.Go != null)
            {
                actorEntity.Go.transform.eulerAngles = actorEntity.Rotation;
                actorEntity.SyncRotation = false;
            }
        }
        var bindEntity = entity as BaseBindEntity;
        if (bindEntity != null)
        {
            if (bindEntity.SyncPosition && bindEntity.BindGo != null)
            {
                bindEntity.BindGo.transform.position = bindEntity.Position;
                bindEntity.SyncPosition = false;
            }
            if (bindEntity.SyncRotation && bindEntity.BindGo != null)
            {
                bindEntity.BindGo.transform.eulerAngles = bindEntity.Rotation;
                bindEntity.SyncRotation = false;
            }
        }
    }
}