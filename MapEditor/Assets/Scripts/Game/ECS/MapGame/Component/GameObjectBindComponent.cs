/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:16:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:16:00
* @ Description:
*
*/

/// <summary>
/// GameObjectBindComponent.cs
/// GameObject绑定组件
/// </summary>
public class GameObjectBindComponent : BaseComponent
{
    /// <summary>
    /// 是否自动销毁绑定对象
    /// </summary>
    public bool IsAutoDestroyBindGo;

    /// <summary>
    /// 重置数据
    /// </summary>
    protected override void ResetDatas()
    {
        base.ResetDatas();
        IsAutoDestroyBindGo = false;
    }
}
