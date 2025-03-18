/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:37:52
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-24 15:29:00
* @ Description:
*
*/

using UnityEngine;

/// <summary>
/// BaseBindEntity.cs
/// 带实体对象的Entity基类(无视寻路的物体)
/// </summary>
public abstract class BaseBindEntity : BaseEntity
{
    public BaseBindEntity() : base()
    {

    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        DestroyInstance();
    }

    /// <summary>
    /// 销毁实例
    /// </summary>
    protected virtual void DestroyInstance()
    {
        var gameObjectBindComponent = GetComponent<GameObjectBindComponent>();
        if (gameObjectBindComponent.IsAutoDestroyBindGo)
        {
            EntityUtilities.DestroyInstance(this);
        }
    }
}