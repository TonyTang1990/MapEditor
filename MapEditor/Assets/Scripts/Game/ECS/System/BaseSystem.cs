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
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被激活！");
    }

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public virtual void OnAddToWorld()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被添加到世界！");
    }

    /// <summary>
    /// 响应取消激活
    /// </summary>
    public virtual void OnDisable()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被取消激活！");
    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public virtual void OnRemoveFromWorld()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被从世界移除！");
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void Update(float deltaTime)
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
    /// <param name="fixedDeltaTime"></param>
    public virtual void FixedUpdate(float fixedDeltaTime)
    {

    }

    /// <summary>
    /// LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void LateUpdate(float deltaTime)
    {

    }
}