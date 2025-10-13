/*
 * Description:             TreasureBoxEntity.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/13
 */

using System;

/// <summary>
/// TreasureBoxEntity.cs
/// 宝箱Entity
/// </summary>
public class TreasureBoxEntity : BaseObjectEntity
{
    /// <summary>
    /// 获取EntityView类型信息
    /// </summary>
    /// <returns></returns>
    public override Type GetEntityViewType()
    {
        return ECSConst.TreasureBoxEntityViewType;
    }
}