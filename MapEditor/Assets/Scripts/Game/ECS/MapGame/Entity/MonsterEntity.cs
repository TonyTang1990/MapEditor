/*
 * Description:             MonsterEntity.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/13
 */

using System;

/// <summary>
/// MonsterEntity.cs
/// 怪物Entity
/// </summary>
public class MonsterEntity : BaseObjectEntity
{
    /// <summary>
    /// 获取EntityView类型信息
    /// </summary>
    /// <returns></returns>
    public override Type GetEntityViewType()
    {
        return ECSConst.MonsterEntityViewType;
    }
}