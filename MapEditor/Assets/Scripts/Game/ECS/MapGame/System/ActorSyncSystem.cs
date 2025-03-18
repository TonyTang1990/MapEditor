/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:13
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-17 15:58:10
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
        var actorComponent = entity.GetComponent<ActorComponent>();
        if(actorComponent != null)
        {
            return true;
        }
        var gameObjectBindComponent = entity.GetComponent<GameObjectBindComponent>();
        if(gameObjectBindComponent != null)
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
        if (entity != null)
        {
            var gameObjectSyncComponent = entity.GetComponent<GameObjectSyncComponent>();
            var positionComponent = entity.GetComponent<PositionComponent>();
            if(gameObjectSyncComponent != null && gameObjectSyncComponent.SyncPosition &&
                positionComponent != null)
            {
                EntityUtilities.SetGoPosition(entity, positionComponent.Position.x, positionComponent.Position.y, positionComponent.Position.z);   
                gameObjectSyncComponent.SyncPosition = false;
            }
            var rotationComponent = entity.GetComponent<RotationComponent>();
            if(gameObjectSyncComponent != null && gameObjectSyncComponent.SyncRotation &&
                rotationComponent != null)
            {               
                EntityUtilities.SetGoRotation(entity, rotationComponent.Rotation.x, rotationComponent.Rotation.y, rotationComponent.Rotation.z);   
                gameObjectSyncComponent.SyncRotation = false;
            }
        }
    }
}