/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:05
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-17 15:55:15
 * @ Description:
 */

using System;

/// <summary>
/// PlayerEntity.cs
/// 玩家Entity
/// </summary>
public class PlayerEntity : BaseObjectEntity
{
    /// <summary>
    /// 获取EntityView类型信息
    /// </summary>
    /// <returns></returns>
    public override Type GetEntityViewType()
    {
        return ECSConst.PlayerEntityViewType;
    }
}