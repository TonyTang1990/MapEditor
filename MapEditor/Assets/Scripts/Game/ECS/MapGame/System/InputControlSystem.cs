/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-24 18:18:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-19 11:25:35
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
        if(entityTypeComponent == null)
        {
            return false;
        }
        var entityType = entityTypeComponent.EntityType;
        return entityType == EntityType.MapGame;
    }

    /// <summary>
    /// PreUpdate
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deltaTime"></param>
    public override void PreProcess(float deltaTime)
    {
        base.PreProcess(deltaTime);
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
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var offsetPos = new Vector3(0, 0, playerMoveSpeed * deltaTime);
        EntityUtilities.MoveNavMeshAgentEntity(firstPlayerEntity, offsetPos);
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
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var offsetPos = new Vector3(0, 0, -playerMoveSpeed * deltaTime);
        EntityUtilities.MoveNavMeshAgentEntity(firstPlayerEntity, offsetPos);
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
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var offsetPos = new Vector3(-playerMoveSpeed * deltaTime, 0, 0);
        EntityUtilities.MoveNavMeshAgentEntity(firstPlayerEntity, offsetPos);
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
        var playerMoveSpeed = MapGameManager.Singleton.PlayerMoveSpeed;
        var offsetPos = new Vector3(playerMoveSpeed * deltaTime, 0, 0);
        EntityUtilities.MoveNavMeshAgentEntity(firstPlayerEntity, offsetPos);
    }
}