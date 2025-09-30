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
/// MapGameEntitySpawnSystem.cs
/// 地图游戏Entity生成系统
/// </summary>
public class MapGameEntitySpawnSystem : BaseSystem
{
    /// <summary>
    /// 关联的地图游戏Entity Uuid
    /// </summary>
    private int mMapGameEntityUuid;

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public override void OnAddToWorld()
    {
        base.OnAddToWorld();
        CreateMapGameEntity();
    }

    /// <summary>
    /// 创建地图游戏Entity
    /// </summary>
    private void CreateMapGameEntity()
    {
        var mapGameEntity = GetWorld<MapGameWorld>.CreateGameEntity();
        mMapGameEntityUuid = mapGameEntity.Uuid;
    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public override void OnRemoveFromWorld()
    {
        base.OnRemoveFromWorld();
        OwnerWorld.DestroyEntityByUuid(mMapGameEntityUuid);
        mMapGameEntityUuid = 0;
    }
}
