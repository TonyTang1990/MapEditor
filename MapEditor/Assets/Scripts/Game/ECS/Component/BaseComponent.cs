/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 16:00:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 16:00:00
* @ Description:
*
*/

// Note:
// 这里的BaseWorld,BaseSystem,BaseEntity,BaseComponent并非完整的ECS模式
// 只是设计上借鉴ECS和tiny-ecs的概念设计

/// <summary>
/// ECS裡Component基类抽象
/// </summary>
public abstract class BaseComponent : IRecycle
{
    public BaseComponent()
    {

    }

    /// <summary>
    /// 出池
    /// </summary>
    public virtual void OnCreate()
    {

    }

    /// <summary>
    /// 入池
    /// </summary>
    public virtual void OnDispose()
    {
        ResetDatas();
    }

    /// <summary>
    /// 重置数据(子类重写)
    /// </summary>
    protected virtual void ResetDatas()
    {

    }
}