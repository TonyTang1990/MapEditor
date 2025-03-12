/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-17 16:54:54
* @ Description:
*
*/

// Note:
// 这里的BaseWorld,BaseSystem和BaseEntity并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计，参考的tiny-ecs设计

using System.Collections.Generic;
using UnityEngine;

//// <summary>
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

    /// <summary>
    /// 系统相关Entity列表
    /// </summary>
    public List<BaseEntity> SystemEntityList
    {
        get;
        private set;
    }

    public BaseSystem()
    {
        SystemEntityList = new List<BaseEntity>();
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
    /// Entity过滤
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public virtual bool Filter(BaseEntity entity)
    {
        return false;
    }

    /// <summary>
    /// 添加系统Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool AddSystemEntity(BaseEntity entity)
    {
        if(SystemEntityList.Contains(entity))
        {
            Debug.LogError($"系统名:{SystemName}已存在Entity Uuid:{entity.Uuid}的Entity，添加系统Entity失败！");
            return false;
        }
        SystemEntityList.Add(entity);
        return true;
    }

    /// <summary>
    /// 移除指定系统Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool RemoveSystemEntity(BaseEntity entity)
    {
        var result = SystemEntityList.Remove(entity);
        if(!result)
        {
            Debug.LogError($"系统名:{SystemName}找不到Entity Uuid:{entity.Uuid}的Entity，移除系统Entity失败！");
        }
        return result;
    }

    /// <summary>
    /// 响应激活
    /// </summary>
    public virtual void OnEnable()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被激活！");
    }

    /// <summary>
    /// 添加所有事件
    /// </summary>
    public virtual void AddEvents()
    {

    }

    /// <summary>
    /// 响应系统添加到世界
    /// </summary>
    public virtual void OnAddToWorld()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被添加到世界！");
    }

    /// <summary>
    /// 响应Entity添加
    /// </summary>
    /// <param name="entity"></param>
    public virtual void OnAdd(BaseEntity entity)
    {

    }

    /// <summary>
    /// 响应取消激活
    /// </summary>
    public virtual void OnDisable()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被取消激活！");
    }

    /// <summary>
    /// 移除所有事件
    /// </summary>
    public virtual void RemoveEvents()
    {

    }

    /// <summary>
    /// 响应Entity移除
    /// </summary>
    /// <param name="entity"></param>
    public virtual void OnRemove(BaseEntity entity)
    {

    }

    /// <summary>
    /// 响应系统从世界移除
    /// </summary>
    public virtual void OnRemoveFromWorld()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被从世界移除！");
    }

    /// <summary>
    /// 响应系统销毁
    /// </summary>
    public virtual void OnDestroy()
    {
        Debug.Log($"世界名:{OwnerWorld.WorldName}的系统名:{SystemName}被销毁！");
        SystemEntityList.Clear();
        OwnerWorld = null;
        SystemName = null;
    }

    /// <summary>
    /// PreUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void PreProcess(float deltaTime)
    {

    }

    /// <summary>
    /// Entity Update
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="deltaTime"></param>
    public virtual void Process(BaseEntity entity, float deltaTime)
    {

    }

    /// <summary>
    /// PostUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void PostProcess(float deltaTime)
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