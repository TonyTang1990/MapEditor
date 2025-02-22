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
    /// Update
    /// </summary>
    public override void Update()
    {
        base.Update();
        var allEntity = OwnerWorld.GetAllEntity();
        foreach ( var entity in allEntity )
        {
            var actorEntity = entity as BaseActorEntity;
            if (actorEntity == null)
            {
                continue;
            }
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
    }
}