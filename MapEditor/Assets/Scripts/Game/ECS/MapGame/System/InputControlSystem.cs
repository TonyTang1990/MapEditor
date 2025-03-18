/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-24 18:18:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-17 16:02:41
 * @ Description:
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// InputControlSystem.cs
/// 操作控制系统
/// </summary>
public class InputControlSystem : BaseSystem
{
    /// <summary>
    /// Entity过滤
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public override bool Filter(BaseEntity entity)
    {
        var entityTypeComponent = entity.GetComponent<EntityTypeComponent>();
        var entityType = entityTypeComponent.EntityType;
        return entityType == EntityType.MapGame;
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deltaTime"></param>
    public override void Process(BaseEntity entity, float deltaTime)
    {
        base.Process(entity, deltaTime);
        if(Input.GetKey("w"))
        {
            MoveUp(deltaTime);
        }
        if (Input.GetKey("s"))
        {
            MoveDown(deltaTime);
        }
        if (Input.GetKey("a"))
        {
            MoveLeft(deltaTime);
        }
        if (Input.GetKey("d"))
        {
            MoveRight(deltaTime);
        }
    }

    /// <summary>
    /// 向上移动
    /// </summary>
    /// <param name="deltaTime"></param>
    private void MoveUp(float deltaTime)
    {
        var firstPlayerEntity = OwnerWorld.GetFirstEntityByType<PlayerEntity>(EntityType.Player);
        if (firstPlayerEntity == null)
        {
            return;
        }
        var positionComponent = firstPlayerEntity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            return;
        }
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var playerOldPosition = positionComponent.Position;
        EntityUtilities.SetPositionOnNav(firstPlayerEntity, playerOldPosition.x, playerOldPosition.y, playerOldPosition.z + playerMoveSpeed * deltaTime);
    }

    /// <summary>
    /// 向下移动
    /// </summary>
    /// <param name="deltaTime"></param>
    private void MoveDown(float deltaTime)
    {
        var firstPlayerEntity = OwnerWorld.GetFirstEntityByType<PlayerEntity>(EntityType.Player);
        if (firstPlayerEntity == null)
        {
            return;
        }
         var positionComponent = firstPlayerEntity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            return;
        }
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var playerOldPosition = positionComponent.Position;
        EntityUtilities.SetPositionOnNav(firstPlayerEntity, playerOldPosition.x, playerOldPosition.y, playerOldPosition.z - playerMoveSpeed * deltaTime);
    }

    /// <summary>
    /// 向左移动
    /// </summary>
    /// <param name="deltaTime"></param>
    private void MoveLeft(float deltaTime)
    {
        var firstPlayerEntity = OwnerWorld.GetFirstEntityByType<PlayerEntity>(EntityType.Player);
        if (firstPlayerEntity == null)
        {
            return;
        }
        var positionComponent = firstPlayerEntity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            return;
        }
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var playerOldPosition = positionComponent.Position;
        EntityUtilities.SetPositionOnNav(firstPlayerEntity, playerOldPosition.x - playerMoveSpeed * deltaTime, playerOldPosition.y, playerOldPosition.z);
    }

    /// <summary>
    /// 向右移动
    /// </summary>
    /// <param name="deltaTime"></param>
    private void MoveRight(float deltaTime)
    {
        var firstPlayerEntity = OwnerWorld.GetFirstEntityByType<PlayerEntity>(EntityType.Player);
        if (firstPlayerEntity == null)
        {
            return;
        }
        var positionComponent = firstPlayerEntity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            return;
        }
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var playerOldPosition = positionComponent.Position;
        EntityUtilities.SetPositionOnNav(firstPlayerEntity, playerOldPosition.x + playerMoveSpeed * deltaTime, playerOldPosition.y, playerOldPosition.z);
    }
}