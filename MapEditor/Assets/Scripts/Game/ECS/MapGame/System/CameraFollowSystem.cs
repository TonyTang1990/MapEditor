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
/// CameraFollowSystem.cs
/// 摄像机跟随系统
/// </summary>
public class CameraFollowSystem : BaseSystem
{
    /// <summary>
    /// 关联的摄像机Entity Uuid
    /// </summary>
    private int mCameraEntityUuid;

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public override void OnAddToWorld()
    {
        base.OnAddToWorld();
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var cameraEntity = OwnerWorld.CreateEtity<CameraEntity>(mapGameWorld.GameVirtualCamera.transform, false);
        mCameraEntityUuid = cameraEntity.Uuid;
    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public override void OnRemoveFromWorld()
    {
        base.OnRemoveFromWorld();
        OwnerWorld.DestroyEntityByUuid(mCameraEntityUuid);
    }

    /// <summary>
    /// LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void LateUpdate(float deltaTime)
    {
        base.LateUpdate(deltaTime);
        var firstPlayerEntity = OwnerWorld.GetFirstEntityByType<PlayerEntity>(EntityType.Player);
        if(firstPlayerEntity == null)
        {
            return;
        }
        var cameraEntity = OwnerWorld.GetEntityByUuid<CameraEntity>(mCameraEntityUuid);
        if(cameraEntity == null)
        {
            return;
        }
        var playerPos = firstPlayerEntity.Position;
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var playerCameraPosOffset = mapGameWorld.PlayerCameraPosOffset;
        var newCameraPos = playerPos + playerCameraPosOffset;
        cameraEntity.SetPosition(newCameraPos.x, newCameraPos.y, newCameraPos.z);
    }
}