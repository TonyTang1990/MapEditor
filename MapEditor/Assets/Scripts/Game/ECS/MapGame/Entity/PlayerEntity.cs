/*
 * Description:             PlayerEntity.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/13
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