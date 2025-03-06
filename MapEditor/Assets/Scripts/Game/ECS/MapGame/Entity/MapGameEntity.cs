/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:05
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-02 23:46:36
 * @ Description:
 */

using System;
using System.Collections.Generic;

/// <summary>
/// MapGameEntity.cs
/// 地图游戏Entity
/// </summary>
public class MapGameEntity : BaseEntity
{
    /// <summary>
    /// 初始化Entity类型
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.MapGame;
    }
}