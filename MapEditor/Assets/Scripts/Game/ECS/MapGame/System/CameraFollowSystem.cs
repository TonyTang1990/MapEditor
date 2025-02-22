/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:13
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:38:53
 * @ Description:
 */

using System;
using System.Collections.Generic;

/// <summary>
/// CameraFollowSystem.cs
/// 摄像机跟随系统
/// </summary>
public class CameraFollowSystem : BaseSystem
{
    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public override void OnAddToWorld()
    {
        base.OnAddToWorld();
        var mapGameWorld = OwnerWorld as MapGameWorld;
        OwnerWorld.CreateEtity<CameraEntity>(mapGameWorld.MainCamera.transform);
    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public override void OnRemoveFromWorld()
    {
        base.OnRemoveFromWorld();
    }
}