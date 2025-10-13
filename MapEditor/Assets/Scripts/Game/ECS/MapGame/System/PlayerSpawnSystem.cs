/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// PlayerSpawnSystem.cs
/// 玩家生成系统
/// </summary>
public class PlayerSpawnSystem : BaseSystem
{
    /// <summary>
    /// 关联的玩家Entity Uuid
    /// </summary>
    private int mPlayerEntityUuid;

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public override void OnAddToWorld()
    {
        base.OnAddToWorld();
        CreatePlayerEntity();
    }

    /// <summary>
    /// 创建玩家Entity
    /// </summary>
    private void CreatePlayerEntity()
    {
        var levelConfig = MapGameManager.Singleton.LevelConfig;
        var birthPos = levelConfig.MapData.BirthPos[0];
        var playerEntity = GetWorld<MapGameWorld>().CreatePlayerEntity(MapGameConst.PlayerPrefabPath, birthPos, Vector3.zero);
        EntityUtilities.SetObjectEntityWorldPosOnNav(playerEntity, birthPos.x, birthPos.y, birthPos.z);
        mPlayerEntityUuid = playerEntity.Uuid;
    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public override void OnRemoveFromWorld()
    {
        base.OnRemoveFromWorld();
        OwnerWorld.DestroyEntityByUuid<PlayerEntity>(mPlayerEntityUuid);
        mPlayerEntityUuid = 0;
    }
}
