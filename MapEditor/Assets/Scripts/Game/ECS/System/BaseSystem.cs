/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:39:04
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 16:54:54
 * @ Description:
 */

// Note:
// 这里的BaseWorld,BaseSystem和BaseEntity并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计

/// <summary>
/// BaseSystem.cs
/// 逻辑系统基类抽象(相当于ECS里的S部分)
/// </summary>
public abstract class BaseSystem
{
    /// <summary>
    /// 所属世界
    /// </summary>
    public BaseWorld OwnerWorld
    {
        get;
        private set;
    }

    /// <summary>
    /// 系统名
    /// </summary>
    public string SystemName
    {
        get;
        private set;
    }

    /// <summary>
    /// 系统系统激活
    /// </summary>
    public bool Enable
    {
        get;
        set;
    }

    public BaseSystem()
    {

    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="ownerWorld"></param>
    /// <param name="systemName"></param>
    /// <param name="parameters"></param>
    public virtual void Init(BaseWorld ownerWorld, string systemName, params object[] parameters)
    {
        OwnerWorld = ownerWorld;
        SystemName = systemName;
    }

    /// <summary>
    /// 响应激活
    /// </summary>
    public virtual void OnEnable()
    {

    }

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public virtual void OnAddToWorld()
    {

    }

    /// <summary>
    /// 响应取消激活
    /// </summary>
    public virtual void OnDisable()
    {

    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public virtual void OnRemoveFromWorld()
    {

    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {

    }

    /// <summary>
    /// LogicUpdate
    /// </summary>
    public virtual void LogicUpdate()
    {

    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    public virtual void FixedUpdate()
    {

    }

    /// <summary>
    /// LateUpdate
    /// </summary>
    public virtual void LateUpdate()
    {

    }
}