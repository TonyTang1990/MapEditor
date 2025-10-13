/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-19 18:34:26
* @ Description:
*/

using MapEditor;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

// Note:
// 这里的BaseWorld,BaseSystem和BaseEntity并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计，参考的tiny-ecs设计

/// <summary>
/// BaseWorld.cs
/// 逻辑世界基类抽象(相当于tiny里的World)
/// </summary>
public abstract class BaseWorld
{
    #region World成员定义部分开始
    /// <summary>
    /// 世界名(唯一ID)
    /// </summary>
    public string WorldName
    {
        get;
        private set;
    }

    /// <summary>
    /// 世界更新类型
    /// </summary>
    public WorldUpdateType WorldUpdateType
    {
        get;
        private set;
    }

    /// <summary>
    /// 当前逻辑帧数
    /// </summary>
    public int LogicFrame
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑帧经历时长
    /// </summary>
    public float LogicFramePassedTime
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑帧数
    /// </summary>
    public int LogicFrameRate
    {
        get;
        private set;
    }

    /// <summary>
    /// 单帧时长
    /// </summary>
    public float LogicFrameTime
    {
        get;
        private set;
    }

    /// <summary>
    /// 世界根节点GameObject
    /// </summary>
    protected GameObject mWorldRootGo;
    #endregion

    #region System成员定义部分开始
    /// <summary>
    /// 所有系统类型和系统Map<系统类型，系统>
    /// </summary>
    protected Dictionary<Type, BaseSystem> mAllSystemTypeAndSystemMap;

    /// <summary>
    /// 所有系统列表
    /// Note:
    /// 用于确保确定性更新顺序
    /// </summary>
    protected List<BaseSystem> mAllSystems;

    /// <summary>
    /// 等待添加的系统列表
    /// </summary>
    protected List<BaseSystem> mWaitAddSystems;

    /// <summary>
    /// 临时等待添加系统列表
    /// </summary>
    protected List<BaseSystem> mTempWaitAddSystems;

    /// <summary>
    /// 等待移除的系统列表
    /// </summary>
    protected List<BaseSystem> mWaitRemoveSystems;

    /// <summary>
    /// 临时等待移除系统列表
    /// </summary>
    protected List<BaseSystem> mTempWaitRemoveSystems;
    #endregion

    #region Entity成员定义开始
    /// <summary>
    /// 下一个Entity Uuid
    /// </summary>
    protected int mNextEntityUuid;

    /// <summary>
    /// Entity根节点GameObject
    /// </summary>
    protected GameObject mEntityRootGo;

    /// <summary>
    /// Entity类型信息父节点Map
    /// </summary>
    protected Dictionary<Type, Transform> mEntityClassTypeParentMap;

    /// <summary>
    /// Entity Uuid Map<Entitiy Uuid, Entity>
    /// </summary>
    protected Dictionary<int, BaseEntity> mEntityMap;

    /// <summary>
    /// 所有的Entity
    /// Note:
    /// 用于确保有序访问
    /// </summary>
    protected List<BaseEntity> mAllEntity;

    /// <summary>
    /// 所有Entity类型信息和Entity列表Map<Entity类型信息, Entity列表>
    /// </summary>
    protected Dictionary<Type, List<BaseEntity>> mEntityTypeAndEntitiesMap;

    /// <summary>
    /// 等待更新的Entity类型信息和Entity列表Map<Entity类型信息，Entity列表>
    /// </summary>
    protected Dictionary<Type, List<BaseEntity>> mUpadteEntityTypeAndEntitiesMap;

    /// <summary>
    /// 等待添加的Entity列表
    /// </summary>
    protected List<BaseEntity> mWaitAddEntities;

    /// <summary>
    /// 临时等待添加Entity列表
    /// </summary>
    protected List<BaseEntity> mTempWaitAddEntities;

    /// <summary>
    /// 等待移除的Entity列表
    /// </summary>
    protected List<BaseEntity> mWaitRemoveEntities;

    /// <summary>
    /// 临时等待移除Entity列表
    /// </summary>
    protected List<BaseEntity> mTempWaitRemoveEntities;
    #endregion

    #region EntityView相关
    /// <summary>
    /// EntityView开关
    /// </summary>
    public bool EntityViewSwitch
    {
        get;
        set;
    }

    /// <summary>
    /// Entity Uuid和EntityView Map
    /// </summary>
    protected Dictionary<int, MonoBehaviour> mEntityViewMap;
    #endregion

    #region World部分开始
    public BaseWorld()
    {
        LogicFrame = 0;
        LogicFramePassedTime = 0f;
        LogicFrameRate = 0;
        LogicFrameTime = 0f;

        mAllSystemTypeAndSystemMap = new Dictionary<Type, BaseSystem>();
        mAllSystems = new List<BaseSystem>();
        mWaitAddSystems = new List<BaseSystem>();
        mTempWaitAddSystems = new List<BaseSystem>();
        mWaitRemoveSystems = new List<BaseSystem>();
        mTempWaitRemoveSystems = new List<BaseSystem>();

        mNextEntityUuid = 1;
        mEntityMap = new Dictionary<int, BaseEntity>();
        mAllEntity = new List<BaseEntity>();
        mEntityTypeAndEntitiesMap = new Dictionary<Type, List<BaseEntity>>();
        mUpadteEntityTypeAndEntitiesMap = new Dictionary<Type, List<BaseEntity>>();
        mWaitAddEntities = new List<BaseEntity>();
        mTempWaitAddEntities = new List<BaseEntity>();
        mWaitRemoveEntities = new List<BaseEntity>();
        mTempWaitRemoveEntities = new List<BaseEntity>();

        EntityViewSwitch = true;
        mEntityViewMap = new Dictionary<int, MonoBehaviour>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="worldBasicData"></param>
    /// <param name="parameters"></param>
    public virtual void Init(WorldBasicData worldBasicData, params object[] parameters)
    {
        WorldName = worldBasicData.WorldName;
        WorldUpdateType = worldBasicData.WorldUpdateType;
        LogicFrame = 0;
        LogicFramePassedTime = 0f;
        LogicFrameRate = worldBasicData.LogicFrameRate;
        LogicFrameTime = 1f / LogicFrameRate;
    }

    /// <summary>
    /// 响应世界创建
    /// </summary>
    public virtual void OnCreate()
    {
        CreateWorldRoot();
        CreateEntityRoot();
    }

    /// <summary>
    /// 响应世界销毁
    /// </summary>
    public virtual void OnDestroy()
    {
        DestroyAllEntity();
        RemoveAllSystem();
        ManagerSystems();
        ManagerEntities();
        DestroyEntityClassTypeParent();
        DestroyEntityRoot();
        DestroyWorldRoot();
    }

    /// <summary>
    /// Update
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void Update(float deltaTime)
    {
        LogicFramePassedTime += Time.deltaTime;
        while(LogicFramePassedTime >= LogicFrameTime)
        {
            LogicFramePassedTime -= LogicFrameTime;
            LogicUpdate(LogicFrameTime);
        }

        if(WorldUpdateType == WorldUpdateType.UPDATE)
        {
            WorldUpdate(deltaTime);
        }
    }

    /// <summary>
    /// LogicUpdate
    /// </summary>
    /// <param name="logicFrameTime"></param>
    public virtual void LogicUpdate(float logicFrameTime)
    {
        if(WorldUpdateType == WorldUpdateType.LOGIC_UPDATE)
        {
            WorldUpdate(logicFrameTime);
        }

        SystemLogicUpdate(logicFrameTime);
    }

    /// <summary>
    /// 处理系统的逻辑更新
    /// </summary>
    /// <param name="logicFrameTime"></param>
    protected void SystemLogicUpdate(float logicFrameTime)
    {
        foreach (var system in mAllSystems)
        {
            if (system == null)
            {
                continue;
            }
            system.LogicUpdate(logicFrameTime);
        }
    }

    /// <summary>
    /// 世界更新
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void WorldUpdate(float deltaTime)
    {
        ManagerSystems();
        ManagerEntities();

        ManagerSystemsUpdate(deltaTime);
    }

    /// <summary>
    /// 处理System增删
    /// </summary>
    protected void ManagerSystems()
    {
        // 先处理移除系统，再处理新增系统
        mTempWaitRemoveSystems.Clear();
        mTempWaitRemoveSystems.AddRange(mWaitRemoveSystems);
        mWaitRemoveSystems.Clear();

        mTempWaitAddSystems.Clear();
        mTempWaitAddSystems.AddRange(mWaitAddSystems);
        mWaitAddSystems.Clear();

        if(mTempWaitRemoveSystems.Count == 0 && mTempWaitAddSystems.Count == 0)
        {
            return;
        }

        foreach(var waitRemoveSystem in mTempWaitRemoveSystems)
        {
            DoRemoveSystem(waitRemoveSystem);
        }

        foreach (var waitAddSystem in mTempWaitAddSystems)
        {
            DoAddSystem(waitAddSystem);
        }
    }

    /// <summary>
    /// 处理Entity增删
    /// </summary>
    protected void ManagerEntities()
    {
        // 先处理新增Entity，再处理删除Entity
        mTempWaitRemoveEntities.Clear();
        mTempWaitRemoveEntities.AddRange(mWaitRemoveEntities);
        mWaitRemoveEntities.Clear();

        mTempWaitAddEntities.Clear();
        mTempWaitAddEntities.AddRange(mWaitAddEntities);
        mWaitAddEntities.Clear();

        if (mTempWaitRemoveEntities.Count == 0 && mTempWaitAddEntities.Count == 0)
        {
            return;
        }

        foreach (var waitAddEntity in mTempWaitAddEntities)
        {
            DoOnAddEntity(waitAddEntity);
        }

        foreach (var waitRemoveEntity in mTempWaitRemoveEntities)
        {
            DoOnRemoveEntity(waitRemoveEntity);
        }
    }

    /// <summary>
    /// 处理System更新
    /// </summary>
    /// <param name="deltaTime"></param>
    protected void ManagerSystemsUpdate(float deltaTime)
    {
        foreach(var system in mAllSystems)
        {
            if(system == null)
            {
                continue;
            }
            system.PreProcess(deltaTime);

            foreach(var entityData in mUpadteEntityTypeAndEntitiesMap)
            {
                foreach(var entity in entityData.Value)
                {
                    if (system.Filter(entity))
                    {
                        system.Process(entity, deltaTime);
                    }
                }
            }
            system.PostProcess(deltaTime);
        }
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    /// <param name="fixedDeltaTime"></param>
    public virtual void FixedUpdate(float fixedDeltaTime)
    {
        foreach (var system in mAllSystems)
        {
            system?.FixedUpdate(fixedDeltaTime);
        }
    }

    /// <summary>
    /// LateUpdate
    /// </summary>
    /// <param name="deltaTime"></param>
    public virtual void LateUpdate(float deltaTime)
    {
        foreach (var system in mAllSystems)
        {
            if(system == null)
            {
                continue;
            }
            system.LateUpdate(deltaTime);
        }
    }

    /// <summary>
    /// 创建世界根节点
    /// </summary>
    protected void CreateWorldRoot()
    {
        mWorldRootGo = new GameObject($"{WorldName}_Root");
    }
    #endregion

    #region System部分开始
    /// <summary>
    /// 移除所有系统
    /// </summary>
    protected virtual void RemoveAllSystem()
    {
        for (var index = mAllSystems.Count - 1; index >= 0; index--)
        {
            var system = mAllSystems[index];
            RemoveSystem(system);
        }
    }

    /// <summary>
    /// 销毁Entity类型信息挂在父节点
    /// </summary>
    protected void DestroyEntityClassTypeParent()
    {
        foreach(var entityClassTypeParent in mEntityClassTypeParentMap)
        {
            if(entityClassTypeParent.Value != null)
            {
                var entityClassTypeParentGo = entityClassTypeParent.Value.gameObject;
                GameObject.Destroy(entityClassTypeParentGo);
            }
        }
        mEntityClassTypeParentMap.Clear();
    }

    /// <summary>
    /// 销毁Entity根节点
    /// </summary>
    protected void DestroyEntityRoot()
    {
        if (mEntityRootGo != null)
        {
            GameObject.Destroy(mEntityRootGo);
            mEntityRootGo = null;
        }
    }

    /// <summary>
    /// 销毁世界根节点
    /// </summary>
    protected void DestroyWorldRoot()
    {
        if (mWorldRootGo != null)
        {
            GameObject.Destroy(mWorldRootGo);
            mWorldRootGo = null;
        }
    }

    /// <summary>
    /// 创建指定系统名系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="systemName"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public T CreateSystem<T>(params object[] parameters) where T : BaseSystem, new()
    {
        var systemType = typeof(T);
        var targetSystem = GetSystemByType(systemType);
        if (targetSystem == null)
        {
            var system = new T();
            system.Init(this);
            system.Enable = true;
            mWaitAddSystems.Add(system);
            mWaitRemoveSystems.Remove(system);
            return system;
        }
        Debug.LogError($"重复创建系统类型:{systemType.Name}，创建系统失败！");
        return null;
    }

    /// <summary>
    /// 移除指定系统
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    public bool RemoveSystem<T>(T system) where T : BaseSystem
    {
        if (system == null)
        {
            Debug.LogError($"不允许移除空系统！");
            return false;
        }
        var systemType = system.ClassType;
        if(!ExistSystem(system))
        {
            Debug.LogError($"系统类型:{systemType.Name}未找到符合的系统对象，移除系统失败！");
            return false;
        }
        return RemoveSystemByType(systemType);
    }

    /// <summary>
    /// 移除指定系统类型的系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public bool RemoveSystem<T>() where T : BaseSystem
    {
        var systemType = typeof(T);
        return RemoveSystemByType(systemType);
    }

    /// <summary>
    /// 移除指定系统类型的系统
    /// </summary>
    /// <param name="systemType"></param>
    /// <returns></returns>
    public bool RemoveSystemByType(Type systemType)
    {
        var system = GetSystemByType(systemType);
        if(system == null)
        {
            Debug.LogError($"World:{WorldName}找不到系统类型:{systemType.Name}的系统，移除指定系统类型失败！");
            return false;
        }
        system.Enable = false;
        mWaitRemoveSystems.Add(system);
        mWaitAddSystems.Remove(system);
        return true;
    }

    /// <summary>
    /// 执行指定系统移除
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    protected bool DoRemoveSystem(BaseSystem system)
    {
        var systemIndex = mAllSystems.IndexOf(system);
        if(systemIndex != -1)
        {
            foreach(var entity in system.SystemEntityList)
            {
                system.OnRemove(entity);
            }
            mAllSystemTypeAndSystemMap.Remove(system.ClassType);
            mAllSystems.Remove(system);
            system.RemoveEvents();
            system.OnRemoveFromWorld();
            OnRemoveSystem(system);
            system.RemoveSystemAllEntity();
            return true;
        }
        Debug.LogError($"找不到系统类型:{system.ClassType}的系统，执行移除系统失败！");
        return false;
    }

    /// <summary>
    /// 获取指定系统类型的系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="systemName"></param>
    /// <returns></returns>
    public T GetSystem<T>() where T : BaseSystem
    {
        var systemType = typeof(T);
        BaseSystem targetSystem = GetSystemByType(systemType);
        if (targetSystem == null)
        {
            return null;
        }
        return targetSystem as T;
    }

    /// <summary>
    /// 获取指定系统类型的系统
    /// </summary>
    /// <param name="systemType"></param>
    /// <returns></returns>
    public BaseSystem GetSystemByType(Type systemType)
    {
        BaseSystem targetSystem;
        if (!mAllSystemTypeAndSystemMap.TryGetValue(systemType, out targetSystem))
        {
            return null;
        }
        return targetSystem;
    }

    /// <summary>
    /// 响应世界增加系统
    /// </summary>
    /// <param name="system"></param>
    protected virtual void OnAddSystem(BaseSystem system)
    {
        Debug.Log($"世界名:{WorldName}响应添加系统名:{system.ClassType.Name}");
    }

    /// <summary>
    /// 响应世界移除系统
    /// </summary>
    /// <param name="system"></param>
    protected virtual void OnRemoveSystem(BaseSystem system)
    {
        Debug.Log($"世界名:{WorldName}响应移除系统名:{system.ClassType.Name}");
    }

    /// <summary>
    /// 指定系统是否存在
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    protected bool ExistSystem(BaseSystem system)
    {
        var systemType = system.ClassType;
        var targetSystem = GetSystemByType(systemType);
        return targetSystem != null && targetSystem != system;
    }

    /// <summary>
    /// 执行添加指定系统对象
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    protected bool DoAddSystem(BaseSystem system)
    {
        if (system == null)
        {
            Debug.LogError($"不允许添加空系统，执行添加系统失败！");
            return false;
        }
        if (!ExistSystem(system))
        {
            var systemType = system.ClassType;
            mAllSystemTypeAndSystemMap.Add(systemType, system);
            mAllSystems.Add(system);
            system.AddEvents();
            system.OnAddToWorld();
            OnAddSystem(system);
            foreach(var entityData in mUpadteEntityTypeAndEntitiesMap)
            {
                foreach (var entity in entityData.Value)
                {
                    if (system.Filter(entity))
                    {
                        system.AddSystemEntity(entity);
                        system.OnAdd(entity);
                    }
                }
            }
            return true;
        }
        else
        {
            Debug.LogError($"已包含系统类型:{system.ClassType}的系统，添加系统失败！");
            return false;
        }
    }
    #endregion

    #region Entity部分开始
    /// <summary>
    /// 创建Entity根节点GameObject
    /// </summary>
    private void CreateEntityRoot()
    {
        mEntityRootGo = new GameObject("EntityRoot");
        var entityRootTransform = mEntityRootGo.transform;
        entityRootTransform.position = Vector3.zero;
        entityRootTransform.rotation = Quaternion.identity;
        entityRootTransform.SetParent(mWorldRootGo.transform);

        mEntityClassTypeParentMap = new Dictionary<Type, Transform>();
    }

    /// <summary>
    /// 获取下一个Entity Uuid
    /// </summary>
    /// <returns></returns>
    protected int GetNextEntityUuid()
    {
        return ++mNextEntityUuid;
    }

    /// <summary>
    /// 获取指定Entity的Entity类型实体挂载父节点
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public Transform GetEntityTypeParent(BaseEntity entity)
    {
        var entityClassType = entity.ClassType;
        Transform entityTypeParent = null;
        if (mEntityClassTypeParentMap.TryGetValue(entityClassType, out entityTypeParent))
        {
            return entityTypeParent;
        }
        entityTypeParent = CreateEntityClassTypeParent(entityClassType);
        return entityTypeParent;
    }

    /// <summary>
    /// 创建指定Entity类型信息的挂在父节点
    /// </summary>
    /// <param name="entityClassType"></param>
    /// <returns></returns>
    protected Transform CreateEntityClassTypeParent(Type entityClassType)
    {
        Transform entityClassTypeParent = null;
        if(mEntityClassTypeParentMap.TryGetValue(entityClassType, out entityClassTypeParent))
        {
            Debug.LogError($"已存在Entity类型:{entityClassType.Name}的挂载父节点，请勿重复创建挂在父节点！");
            return entityClassTypeParent;
        }
        var entityClassTypeParentGo = new GameObject(entityClassType.Name);
        entityClassTypeParent = entityClassTypeParentGo.transform;
        entityClassTypeParent.SetParent(mEntityRootGo.transform);
        mEntityClassTypeParentMap.Add(entityClassType, entityClassTypeParent);
        return entityClassTypeParent;
    }
    
    /// <summary>
    /// 获取指定Uuid的Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public T GetEntityByUuid<T>(int uuid) where T : BaseEntity
    {
        BaseEntity entity;
        if (!mEntityMap.TryGetValue(uuid, out entity))
        {
            return null;
        }
        return entity as T;
    }

    /// <summary>
    /// 添加指定Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected bool AddEntity<T>(T entity) where T : BaseEntity
    {
        if (entity == null)
        {
            Debug.LogError($"不允许添加空Entity！");
            return false;
        }
        var entityUuid = GetNextEntityUuid();
        entity.SetUuid(entityUuid);
        mEntityMap.Add(entityUuid, entity);
        mAllEntity.Add(entity);
        var entityClassType = entity.ClassType;
        List<BaseEntity> entityList;
        if (!mEntityTypeAndEntitiesMap.TryGetValue(entityClassType, out entityList))
        {
            entityList = new List<BaseEntity>();
            mEntityTypeAndEntitiesMap.Add(entityClassType, entityList);
        }
        entityList.Add(entity);
        mWaitAddEntities.Add(entity);
        mWaitRemoveEntities.Remove(entity);
        return true;
    }

    /// <summary>
    /// 移除指定Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected bool RemoveEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许移除空Entity！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var targetEntity = GetEntityByUuid<BaseEntity>(entityUuid);
        if(targetEntity != entity)
        {
            Debug.LogError($"指定Entity Uuid:{entityUuid}和已存在的目标Entity不一致，移除指定Entity失败！");
            return false;
        }
        mEntityMap.Remove(entityUuid);
        mAllEntity.Remove(entity);
        mWaitRemoveEntities.Add(entity);
        mWaitAddEntities.Remove(entity);
        return true;
    }

    /// <summary>
    /// 执行响应添加指定Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected bool DoOnAddEntity<T>(T entity) where T : BaseEntity
    {
        if (entity == null)
        {
            Debug.LogError($"不允许执行响应添加空Entity！");
            return false;
        }
        var entityType = entity.ClassType;
        List<BaseEntity> entityList;
        if(!mUpadteEntityTypeAndEntitiesMap.TryGetValue(entityType, out entityList))
        {
            entityList = new List<BaseEntity>();
            mUpadteEntityTypeAndEntitiesMap.Add(entityType, entityList);
        }
        entityList.Add(entity);
        foreach(var system in mAllSystems)
        {
            if(system.Filter(entity))
            {
                system.AddSystemEntity(entity);
                system.OnAdd(entity);
            }
        }
        return true;
    }

    /// <summary>
    /// 获取所有Entity列表
    /// </summary>
    /// <returns></returns>
    public List<BaseEntity> GetAllEntity()
    {
        return mAllEntity;
    }

    /// <summary>
    /// 获取指定Entity类型信息的Entity列表
    /// </summary>
    /// <param name="entityClassType"></param>
    /// <returns></returns>
    public List<BaseEntity> GetEntityListByType(Type entityClassType)
    {
        List<BaseEntity> entityList;
        if (!mEntityTypeAndEntitiesMap.TryGetValue(entityClassType, out entityList))
        {
            return null;
        }
        return entityList;
    }

    /// <summary>
    /// 获取指定Entity类型的首个Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public T GetFirstEntityByType<T>(EntityType entityType) where T : BaseEntity
    {
        var entityClassType = typeof(T);
        List<BaseEntity> entityList;
        if(!mEntityTypeAndEntitiesMap.TryGetValue(entityClassType, out entityList))
        {
            return null;
        }
        if(entityList == null || entityList.Count == 0)
        {
            return null;
        }
        return entityList[0] as T;
    }

    /// <summary>
    /// 创建指定Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public T CreateEntity<T>(params object[] parameters) where T : BaseEntity, new()
    {
        T entity = ObjectPool.Singleton.Pop<T>();
        AddEntity(entity);
        CreateEntityView(entity);
        return entity;
    }

    /// <summary>
    /// 销毁指定Uuid的Entity
    /// </summary>
    /// <param name="uuid"></param>
    /// <returns></returns>
    public bool DestroyEntityByUuid(int uuid)
    {
        var entity = GetEntityByUuid<BaseEntity>(uuid);
        if (entity == null)
        {
            Debug.LogError($"找不大Uuid:{uuid}的Entity，销毁指定Uuid的Entity失败！");
            return false;
        }
        var result = RemoveEntity(entity);
        if(result)
        {
            DestroyEntityView(entity);
        }
        return result;
    }

    /// <summary>
    /// 销毁指定Entity
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool DestroyEntity(BaseEntity entity)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许销毁空Entity！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var getEntity = GetEntityByUuid<BaseEntity>(entityUuid);
        if (getEntity == null)
        {
            Debug.LogError($"找不到Uuid:{entityUuid}的Entity，销毁指定Entity失败！");
            return false;
        }
        if (getEntity != entity)
        {
            Debug.LogError($"Uuid:{entityUuid}的Entity与传入的Entity不一致，销毁指定Entity失败！");
            return false;
        }
        return DestroyEntityByUuid(entityUuid);
    }

    /// <summary>
    /// 执行响应Entity移除
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    protected bool DoOnRemoveEntity(BaseEntity entity)
    {
        var entityType = entity.ClassType;
        List<BaseEntity> entityList;
        if (!mUpadteEntityTypeAndEntitiesMap.TryGetValue(entityType, out entityList))
        {
            Debug.LogError($"找不到Entity类型:{entityType.Name}的更新Entity列表，响应移除Entity失败！");
            return false;
        }
        var result = entityList.Remove(entity);
        if(!result)
        {
            Debug.LogError($"指定Entity类型:{entityType.Name}的更新Entity列表里找不到目标Entity Uuid:{entity.Uuid}的Entity对象，响应移除Entity失败！");
            return false;
        }
        foreach (var system in mAllSystems)
        {
            if(system.Filter(entity))
            {
                system.RemoveSystemEntity(entity);
                system.OnRemove(entity);
            }
        }
        entity.OnDestroy();
        ObjectPool.Singleton.Push(entity);
        return true;
    }

    /// <summary>
    /// 销毁所有Entity
    /// </summary>
    public void DestroyAllEntity()
    {
        for(int index = mAllEntity.Count - 1; index >=0; index--)
        {
            var entity = mAllEntity[index];
            var entityUuid = entity.Uuid;
            DestroyEntityByUuid(entityUuid);
        }
    }
    #endregion

    #region EntityView相关
    /// <summary>
    /// 添加指定EntityView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityView"></param>
    /// <returns></returns>
    public bool AddEntityView<T>(BaseEntityView<T> entityView) where T : BaseEntity
    {
        var entityUuid = entityView.Uuid;
        if(mEntityViewMap.ContainsKey(entityUuid))
        {
            Debug.LogError($"不允许重复添加Entity Uuid:{entityUuid}的EntityView，添加EntityView失败！");)
            return false;
        }
        mEntityViewMap.Add(entityUuid, entityView);
        return true;
    }

    /// <summary>
    /// 移除指定
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityView"></param>
    /// <returns></returns>
    public bool RemoveEntityView<T>(BaseEntityView<T> entityView) where T : BaseEntity
    {
        var entityUuid = entityView.Uuid;
        if(!mEntityViewMap.ContainsKey(entityUuid))
        {
            Debug.LogError($"找不到Entity Uuid:{entityUuid}的EntityView，移除EntityView失败！");
            return false;
        }
        mEntityViewMap.Remove(entityUuid);
        return true;
    }

    /// <summary>
    /// 获取指定Entity Uuid的可视化EntityView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entityUuid"></param>
    /// <returns></returns>
    public BaseEntityView<T> GetEntityView<T>(int entityUuid) where T : BaseEntity
    {
        MonoBehaviour entityView;
        if(!mEntityViewMap.TryGetValue(entityUuid, out entityView))
        {
            Debug.LogError($"找不到Entity Uuid:{entityUuid}的EntityView，获取EntityView失败！");)
            return null;
        }
        return entityView as BaseEntityView<T>;
    }

    /// <summary>
    /// 同步指定Entity的EntityView数据
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool SyncEntityViewData<T>(T entity) where T : BaseEntity
    {
        if(entity == null)
        {
            Debug.LogError($"不允许同步空Entity的EntityView数据，同步Entity数据失败！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var entityView = GetEntityView<T>(entityUuid);
        if(entityView == null)
        {
            Debug.LogError($"找不到Entity Uuid:{entityUuid}的EntityView，同步Entity的EntityView数据失败！");
            return false;
        }
        entityView.SyncData();
        return true;
    }

    /// <summary>
    /// 获取指定Entity的EntityView GameObject名字
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    private string GetEntityViewGoName<T>(T entity) where T : BaseEntity
    {
        if(entity == null)
        {
            return string.Empty;
        }
        var entityUuid = entity.Uuid;
        var entityType = entity.GetType();
        var entityTypeName = entityType.Name;
        var entityViewGoName = $"{entityTypeName}_{entityUuid}_View";
        return entityViewGoName;
    }

    /// <summary>
    /// 创建指定Entity的可视化EntityView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public BaseEntityView<T> CreateEntityView<T>(T entity) where T : BaseEntity
    {
        if(!EntityViewSwitch)
        {
            return null;
        }
        if(entity == null)
        {
            Debug.LogError($"不允许创建空Entity的EntityView，创建EntityView失败！");
            return null;
        }
        var entityViewType = entity.GetEntityViewType();
        if(entityViewType == null)
        {
            return null;
        }
        var entityViewGoName = GetEntityViewGoName(entity);
        // TODO:用池
        var entityViewGo = new GameObject(entityViewGoName);
        var entityViewInstance = entityViewGo.AddComponent(entityViewType) as BaseEntityView<T>;
        entityViewInstance.Init(entity);
        AddEntityView(entityViewInstance);
        return entityViewInstance;
    }

    /// <summary>
    /// 销毁指定Entity的EntityView
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public bool DestroyEntityView<T>(T entity) where T : BaseEntity
    {
        if (entity == null)
        {
            Debug.LogError($"不允许销毁空Entity的EntityView，销毁EntityView失败！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var entityView = GetEntityView<T>(entityUuid);
        if(entityView == null)
        {
            Debug.LogError($"找不到Entity Uuid:{entityUuid}的EntityView，销毁EntityView失败！");
            return false;
        }
        var result = RemoveEntityView(entityView);
        if(result)
        {
            // TODO:用池
            GameObject.Destroy(entityView.gameObject);
        }
        return result;
    }
    #endregion
}