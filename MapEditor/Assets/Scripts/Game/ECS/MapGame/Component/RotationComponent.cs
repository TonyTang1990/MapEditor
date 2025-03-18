

using UnityEngine;

/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*//// <summary>
  /// RotationComponent.cs
  /// 旋转组件
  /// </summary>
public class RotationComponent : BaseComponent
{
    /// <summary>
    /// 旋转
    /// </summary>
    public Vector3 Rotation;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        Rotation = Vector3.zero;
    }
}
