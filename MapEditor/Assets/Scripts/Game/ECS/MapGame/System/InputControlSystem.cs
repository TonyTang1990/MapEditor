/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-24 18:18:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-24 18:18:00
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
    /// Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public override void Update(float deltaTime)
    {
        base.Update(deltaTime);
        if(Input.GetButtonDown("W"))
        {
            MoveUp(deltaTime);
        }
        else if (Input.GetButtonDown("S"))
        {
            MoveDown(deltaTime);
        }
        else if (Input.GetButtonDown("A"))
        {
            MoveLeft(deltaTime);
        }
        else if (Input.GetButtonDown("D"))
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
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var playerMoveSpeed = mapGameWorld.PlayerMoveSpeed;
        var playerOldPosition = firstPlayerEntity.Position;
        firstPlayerEntity.SetPosition(playerOldPosition.x, playerOldPosition.y, playerOldPosition.z + playerMoveSpeed * deltaTime);
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
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var playerMoveSpeed = mapGameWorld.PlayerMoveSpeed;
        var playerOldPosition = firstPlayerEntity.Position;
        firstPlayerEntity.SetPosition(playerOldPosition.x, playerOldPosition.y, playerOldPosition.z - playerMoveSpeed * deltaTime);
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
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var playerMoveSpeed = mapGameWorld.PlayerMoveSpeed;
        var playerOldPosition = firstPlayerEntity.Position;
        firstPlayerEntity.SetPosition(playerOldPosition.x - playerMoveSpeed * deltaTime, playerOldPosition.y, playerOldPosition.z);
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
        var mapGameWorld = OwnerWorld as MapGameWorld;
        var playerMoveSpeed = mapGameWorld.PlayerMoveSpeed;
        var playerOldPosition = firstPlayerEntity.Position;
        firstPlayerEntity.SetPosition(playerOldPosition.x + playerMoveSpeed * deltaTime, playerOldPosition.y, playerOldPosition.z);
    }
}