/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

using UnityEngine;

/// <summary>
/// AnimatorPlayComponent.cs
/// Animator动画播放组件
/// </summary>
public class AnimatorPlayComponent : BaseComponent
{
    /// <summary>
    /// 动画组件爱你
    /// </summary>
    public Animator Animator;

    /// <summary>
    /// 播放动画名
    /// </summary>
    public string PlayAnimName;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        Animator = null;
        PlayAnimName = null;
    }
}
