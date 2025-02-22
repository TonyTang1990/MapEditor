/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:25:59
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:54:52
 * @ Description:
 */

// Note:
// 这里的BaseWorld,BaseSystem和BaseEntity并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计

/// <summary>
/// BaseEntity.cs
/// Entity基类(逻辑数据对象抽象，相当于ECS里的EC部分)
public abstract class BaseEntity : IRecycle
{
    /// <summary>
    /// Entity Uuid
    /// </summary>
    public int Uuid
    {
        get;
        private set;
    }

    /// <summary>
    /// Entity类型
    /// </summary>
    public EntityType EntityType
    {
        get;
        private set;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseEntity()
    {
        InitEntityType();
    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public virtual void OnDestroy()
    {

    }

    /// <summary>
    /// 出池
    /// </summary>
    public virtual void OnCreate()
    {
        InitEntityType();
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
        Uuid = 0;
    }

    /// <summary>
    /// 初始化(子类重写)
    /// </summary>
    /// <param name="parameters">初始化参数</param>
    public virtual void Init(params object[] parameters)
    {
        var uuid = (int)parameters[0];
        Uuid = uuid;
    }

    /// <summary>
    /// 初始化Entity类型
    /// </summary>
    protected abstract void InitEntityType();
}