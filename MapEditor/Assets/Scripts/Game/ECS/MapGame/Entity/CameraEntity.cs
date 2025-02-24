/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:05
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:56
 * @ Description:
 */

using System;
using System.Collections.Generic;
using System.Web.Configuration;

/// <summary>
/// CameraEntity.cs
/// 摄像机Entity
/// </summary>
public class CameraEntity : BaseBindEntity
{
    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="parameters"></param>
    public override void Init(params object[] parameters)
    {
        base.Init(parameters);
    }

    /// <summary>
    /// 初始化Entity类型
    /// </summary>
    protected override void InitEntityType()
    {
        EntityType = EntityType.Camera;
    }
}