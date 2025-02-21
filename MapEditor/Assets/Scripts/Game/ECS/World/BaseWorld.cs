/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-17 16:54:54
* @ Description:
*/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;

// Note:
// 这里的BaseWorld,BaseSystem和BaseEntity并非完整的ECS模式
// 只是设计上借鉴ECS的概念设计

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
    /// 世界根节点GameObject
    /// </summary>
    protected GameObject mWorldRootGo;
    #region

    #region System成员定义部分开始
    /// <summary>
    /// 所有系统Map<系统名，系统>
    /// </summary>
    protected Dictionary<string, BaseSystem> mAllSystemMap; 

    /// <summary>
    /// 所有系统列表
    /// Note:
    /// 用于确保确定性更新顺序
    /// </summary>
    protected List<BaseSystem> mAllSystems;

    /// <summary>
    /// 所有更新的系统名列表
    /// </summary>
    protected List<string> mAllUpdateSystemNames;
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
    /// Entity类型父节点Map
    /// </summary>
    protected Dictionary<EntityType, Transform> mEntityTypeParentMap;

    /// <summary>
    /// Entity Uuid Map<Entitiy Uuid, Entity>
    /// </summary>
    protected Dictionary<int, BaseEntity> mEntityMap;

    /// <summary>
    /// Entity Type Map<EntityType, Entity列表></EntityType>
    /// </summary>
    protected Dictionary<EntityType, List<BaseEntity>> mEntityTypeMap;

    /// <summary>
    /// 玩家Entity类型信息
    /// </summary>
    protected readonly Type PlayerEntityType = typeof(PlayerEntity);
    #endregion

    #region World部分开始
    public BaseWorld()
    {
        mAllSystemMap = new Dictionary<string, BaseSystem>();
        mAllSystems = new List<BaseSystem>();
        mAllUpdateSystemNames = new List<string>();
        mNextEntityUuid = 1;
        mEntityMap = new Dictionary<int, BaseEntity>();
        mEntityTypeMap = new Dictionary<EntityType, List<BaseEntity>>();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="worldName"></param>
    /// <param name="parameters"></param>
    public virtual void Init(string worldName, params object[] parameters)
    {
        WorldName = worldName;
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
        DestroyEntityRoot();
        DestroyWorldRoot();
    }

    /// <summary>
    /// Update
    /// </summary>
    public virtual void Update()
    {
        UpdateAllUpdateSystemNames();
        foreach(var updateSystemName in mAllUpdateSystemNames)
        {
            var system = GetSystem<BaseSystem>(updateSystemName);
            system?.Update();
        }
    }

    /// <summary>
    /// LogicUpdate
    /// </summary>
    public virtual void LogicUpdate()
    {
        UpdateAllUpdateSystemNames();
        foreach (var updateSystemName in mAllUpdateSystemNames)
        {
            var system = GetSystem<BaseSystem>(updateSystemName);
            system?.LogicUpdate();
        }
    }

    /// <summary>
    /// FixedUpdate
    /// </summary>
    public virtual void FixedUpdate()
    {
        UpdateAllUpdateSystemNames();
        foreach (var updateSystemName in mAllUpdateSystemNames)
        {
            var system = GetSystem<BaseSystem>(updateSystemName);
            system?.FixedUpdate();
        }
    }

    /// <summary>
    /// LateUpdate
    /// </summary>
    public virtual void LateUpdate()
    {
        UpdateAllUpdateSystemNames();
        foreach (var updateSystemName in mAllUpdateSystemNames)
        {
            var system = GetSystem<BaseSystem>(updateSystemName);
            system?.LateUpdate();
        }
    }

    /// <summary>
    /// 更新所有需要更新的系统名列表
    /// Note:
    /// 用于确保同一帧相同Update添加的系统不参与相同Update更新导致问题
    /// 同一帧移除的相同Update会正确移除
    /// </summary>
    protected void UpdateAllUpdateSystemNames()
    {
        mAllUpdateSystemNames.Clear();
        foreach(var system in mAllSystems)
        {
            mAllUpdateSystemNames.Add(system.SystemName);
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
        for(var index = mAllSystems.Count - 1; index >= 0; index--)
        {
            var system = mAllSystems[index];
            RemoveSystem(system.SystemName);
        }
    }

    /// <summary>
    /// 销毁Entity根节点
    /// </summary>
    protected void DestroyEntityRoot()
    {
        if(mEntityRootGo != null)
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
    public T CreateSystem<T>(string systemName, params object[] parameters) where T :BaseSystem, new()
    {
        var system = GetSystem<T>(systemName);
        if(system != null)
        {
            var sType = typeof(T);
            Debug.LogError($"World:{WorldName}已包含系统名:{systemName},添加指定系统类型:{sType.Name}和系统名:{systemName}失败！");
            return null;
        }
        system = new T();
        system.Init(this, systemName, parameters);
        var result = AddSystem(system);
        if(result)
        {
            system.OnAddToWorld();
        }
        return system;
    }

    /// <summary>
    /// 移除指定系统名的系统
    /// </summary>
    /// <param name="systemName"></param>
    /// <returns></returns>
    public bool RemoveSystem(string systemName)
    {
        BaseSystem system = GetSystem<BaseSystem>(systemName);
        if(system == null)
        {
            Debug.LogError($"World:{WorldName}找不到系统名:{systemName}的系统，移除指定系统失败！");
            return false;
        }
        mAllSystemMap.Remove(systemName);
        mAllSystems.Remove(system);
        system.OnRemoveFromWorld();
        return true;
    }

    /// <summary>
    /// 获取指定系统名的系统
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="systemName"></param>
    /// <returns></returns>
    public T GetSystem<T>(string systemName) where T : BaseSystem
    {
        BaseSystem targetSystem;
        if(!mAllSystemMap.TryGetValue(systemName, out targetSystem))
        {
            return null;
        }
        return targetSystem as T;
    }

    /// <summary>
    /// 添加指定系统对象
    /// </summary>
    /// <param name="system"></param>
    /// <returns></returns>
    protected bool AddSystem(BaseSystem system)
    {
        if(system == null)
        {
            Debug.LogError($"不允许添加空系统！");
            return false;
        }
        var systemName = system.SystemName;
        var targetSystem = GetSystem<BaseSystem>(systemName);
        if (targetSystem != null)
        {
            Debug.LogError($"已包含系统名:{systemName}的系统，添加系统失败！");
            return false;
        }
        mAllSystemMap.Add(systemName, targetSystem);
        mAllSystems.Add(system);
        return true;
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

        mEntityTypeParentMap = new Dictionary<EntityType, Transform>();
        var entityTypeNames = Enum.GetNames(typeof(EntityType));
        foreach (var entityTypeName in entityTypeNames)
        {
            var entityType = Enum.Parse<EntityType>(entityTypeName);
            var entityTypeParentGo = new GameObject(entityTypeName);
            entityTypeParentGo.transform.SetParent(entityRootTransform);
            mEntityTypeParentMap.Add(entityType, entityTypeParentGo.transform);
        }
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
    /// 获取指定EntityType的挂载父节点
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    protected Transform GetEntityTypeParent(EntityType entityType)
    {
        Transform entityTypeParent;
        if (!mEntityTypeParentMap.TryGetValue(entityType, out entityTypeParent))
        {
            Debug.LogError($"找不到EntityType:{entityType}的挂载父节点！");
            return null;
        }
        return entityTypeParent;
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
        var entityUuid = entity.Uuid;
        var entityType = entity.EntityType;
        List<BaseEntity> entityList;
        if (!mEntityTypeMap.TryGetValue(entityType, out entityList))
        {
            entityList = new List<BaseEntity>();
            mEntityTypeMap.Add(entityType, entityList);
        }
        entityList.Add(entity);
        return true;
    }

    /// <summary>
    /// 获取指定EntityType的Entity列表
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    protected List<BaseEntity> GetEntityList(EntityType entityType)
    {
        List<BaseEntity> entityList;
        if (!mEntityTypeMap.TryGetValue(entityType, out entityList))
        {
            return null;
        }
        return entityList;
    }

    /// <summary>
    /// 创建指定Entity
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public T CreateEtity<T>(params object[] parameters) where T : BaseEntity, new()
    {
        var entityType = typeof(T);
        T entity;
        if (entityType == PlayerEntityType)
        {
            entity = CreatePlayerEntity(parameters) as T;
        }
        else
        {
            entity = ObjectPool.Singleton.pop<T>();
            var entityUuid = GetNextEntityUuid();
            entity.Init(entityUuid, parameters);
        }
        AddEntity(entity);
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
        mEntityMap.Remove(uuid);
        var entityType = entity.EntityType;
        var entityList = GetEntityList(entityType);
        entityList.Remove(entity);
        entity.OnDestroy();
        return true;
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
    /// 销毁所有Entity
    /// </summary>
    public void DestroyAllEntity()
    {
        var allEntityUuids = mEntityMap.Keys;
        foreach(var entityUuid in allEntityUuids)
        {
            DestroyEntityByUuid(entityUuid);
        }
    }

    /// <summary>
    /// 创建玩家Entity
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    protected PlayerEntity CreatePlayerEntity(params object[] parameters)
    {
        var entity = ObjectPool.Singleton.pop<PlayerEntity>();
        var entityUuid = GetNextEntityUuid();
        entity.Init(entityUuid, parameters);
        var parent = GetEntityTypeParent(entity.EntityType);
        LoadEntityPrefabByPath(entity, entity.PrefabPath, parent);
        return entity;
    }

    /// <summary>
    /// 根据路径加载Entity预制体
    /// </summary>
    /// <param name="actorEntity"></param>
    /// <param name="prefabPath"></param>
    /// <param name="parent"></param>
    /// <param name="loadCompleteCb"></param>
    protected void LoadEntityPrefabByPath(BaseEntity actorEntity, string prefabPath, Transform parent, Action<BaseActorEntity> loadCompleteCb = null)
    {
        PoolManager.Singleton.pop(prefabPath, (instance) =>
        {
            actorEntity.Go = instance;
            if (parent != null)
            {
                instance.transform.SetParent(parent);
            }
            loadCompleteCb?.Invoke(actorEntity);
        });
    }
    #endregion
}