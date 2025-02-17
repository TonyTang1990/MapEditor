/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:38:33
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-02-17 18:12:44
 * @ Description:
 */

/// <summary>
/// EntityManager.cs
/// Entity管理单例类
/// </summary>
public class EntityManager : SingletonTemplate<EntityManager>
{
    /// <summary>
    /// 下一个Entity Uuid
    /// </summary>
    private static int NextEntityUuid = 1;

    /// <summary>
    /// Entity根节点GameObject
    /// </summary>
    private GameObject mEntityRootGo;

    /// <summary>
    /// Entity类型父节点Map
    /// </summary>
    private Dictionary<EntityType, Transform> mEntityTypeParentMap;

    /// <summary>
    /// Entity Uuid Map<Entity Uuid, BaseEntity>
    /// </summary>
    private Dictionary<int, BaseEntity> mEntityMap;

    /// <summary>
    /// Entity Type Map<EntityType, Entity列表>
    /// </summary>
    private Dictionary<EntityType, List<BaseEntity>> mEntityTypeMap;
    
    /// <summary>
    /// 玩家Entity类型信息
    /// </summary>
    private readonly Type PlayerEntityType = typeof(PlayerEntity);

    public EntityManager()
    {
        CreateEntityRootGo();

        mEntityMap = new Dictionary<int, BaseEntity>();
        mEntityTypeMap = new Dictionary<EntityType, List<BaseEntity>>();
    }

    /// <summary>
    /// 创建Entity根节点GameObject
    /// </summary>
    private void CreateEntityRootGo()
    {
        mEntityRootGo = new GameObject("EntityRoot");
        var entityRootTransform = mEntityRootGo.transform;
        entityRootTransform.position = Vector3.zero;
        entityRootTransform.rotation = Quaternion.identity;

        mEntityTypeParentMap = new Dictionary<EntityType, Transform>();
        var entityTypeNames = Enum.GetNames(typeof(EntityType));
        foreach(var entityTypeName in entityTypeNames)
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
    private static int GetNextEntityUuid()
    {
        return ++NextEntityUuid;
    }

    /// <summary>
    /// 获取指定EntityType的挂载父节点
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    private Transform GetEntityTypeParent(EntityType entityType)
    {
        Transform entityTypeParent;
        if(!mEntityTypeParentMap.TryGetValue(entityType, out entityTypeParent))
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
        if(!mEntityMap.TryGetValue(uuid, out entity))
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
    private bool AddEntity<T>(T entity) where T : BaseEntity
    {
        if(entity == null)
        {
            Debug.LogError($"不允许添加空Entity！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var entityType = entity.EntityType;
        List<BaseEntity> entityList;
        if(!mEntityTypeMap.TryGetValue(entityType, out entityList))
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
    private List<BaseEntity> GetEntityList(EntityType entityType)
    {
        List<BaseEntity> entityList;
        if(!mEntityTypeMap.TryGetValue(entityType, out entityList))
        {
            return null
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
        if(entityType == PlayerEntityType)
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
        if(entity == null)
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
        if(entity == null)
        {
            Debug.LogError($"不允许销毁空Entity！");
            return false;
        }
        var entityUuid = entity.Uuid;
        var getEntity = GetEntityByUuid<BaseEntity>(entityUuid);
        if(getEntity == null)
        {
            Debug.LogError($"找不到Uuid:{entityUuid}的Entity，销毁指定Entity失败！");
            return false;
        }
        if(getEntity != entity)
        {
            Debug.LogError($"Uuid:{entityUuid}的Entity与传入的Entity不一致，销毁指定Entity失败！");
            return false;
        }
        return DestroyEntityByUuid(entityUuid);
    }
    
    /// <summary>
    /// 创建玩家Entity
    /// </summary>
    /// <param name="parameters"></param>
    /// <returns></returns>
    private PlayerEntity CreatePlayerEntity(params object[] parameters)
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
    private void LoadEntityPrefabByPath(BaseEntity actorEntity, string prefabPath, Transform parent, Action<BaseActorEntity> loadCompleteCb = null)
    {
        PoolManager.Singleton.pop(prefabPath, (instance) =>
        {
            actorEntity.Go = instance;
            if(parent != null)
            {
                instance.transform.SetParent(parent);
            }
            loadCompleteCb?.Invoke(actorEntity);
        });
    }
}
