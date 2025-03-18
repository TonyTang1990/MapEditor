

using UnityEngine;

/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

/// <summary>
/// PositionComponent.cs
/// 位置组件
/// </summary>
public class PositionComponent : BaseComponent
{
    /// <summary>
    /// 位置
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        Position = Vector3.zero;
    }
}
