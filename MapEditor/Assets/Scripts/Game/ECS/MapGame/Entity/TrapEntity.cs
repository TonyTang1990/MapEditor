/*
 * Description:             TrapEntity.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/13
 */

using System;

/// <summary>
/// TrapEntity.cs
/// 陷阱Entity
/// </summary>
public class TrapEntity : BaseObjectEntity
{
    /// <summary>
    /// 获取EntityView类型信息
    /// </summary>
    /// <returns></returns>
    public override Type GetEntityViewType()
    {
        return ECSConst.TrapEntityViewType;
    }
}