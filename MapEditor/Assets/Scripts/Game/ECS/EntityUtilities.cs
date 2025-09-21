/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:15:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:15:00
* @ Description:
*
*/

using UnityEngine;

/// <summary>
/// EntityUtilities.cs
/// 地图游戏Entity基类
/// </summary>
public static class EntityUtilities
{
    #region Entity组件相关初始化
    /// <summary>
    /// 初始化Entity组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    /// <returns></returns>
    public static bool InitEntityComponents<T>(T entity, params object[] parameters) where T : BaseEntity
    {
        if (entity == null)
        {
            Debug.LogError($"不允许初始化空Entity组件！");
            return false;
        }
        var entityClassType = entity.ClassType;
        if(entityClassType == ECSConst.PlayerEntityType)
        {
            InitPlayerEntityComponents(entity as PlayerEntity, parameters);
        }
        else if(entityClassType == ECSConst.MonsterEntityType)
        {
            InitMonsterEntityComponents(entity as MonsterEntity, parameters);
        }
        else if(entityClassType == ECSConst.TreasureBoxEntityType)
        {
            InitTreasureBoxEntityComponents(entity as TreasureBoxEntity, parameters);
        }
        else if(entityClassType == ECSConst.TrapEntityType)
        {
            InitTrapEntityComponents(entity as TrapEntity, parameters);
        }
        else if(entityClassType == ECSConst.CameraEntityType)
        {
            InitCameraEntityComponents(entity as CameraEntity, parameters);
        }
        else if(entityClassType == ECSConst.MapGameEntityType)
        {
            InitMapGameEntityComponents(entity as MapGameEntity, parameters);
        }
        return true;
    }

    /// <summary>
    /// 初始化角色Entity通用组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityType"></param>
    /// <param name="prefabPath"></param>
    private static void InitActorEntityCommonComponents(BaseEntity entity, EntityType entityType, string prefabPath)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(entityType);
        var positionComponent = ComponentUtilities.CreatePositionComponent();
        var rotationComponent = ComponentUtilities.CreateRotationComponent();
        var gameObjectComponent = ComponentUtilities.CreateGameObjectComponent(null, prefabPath);
        var gameObjectSyncComponent = ComponentUtilities.CreateGameObjectSyncComponent();
        var animatorPlayerComponent = ComponentUtilities.CreateAnimatorPlayComponent();
        var actorComponent = ComponentUtilities.CreateActorComponent();
        entity.AddComponents(entityTypeComponent, positionComponent, rotationComponent, gameObjectComponent, gameObjectSyncComponent, animatorPlayerComponent, actorComponent);
    }

    /// <summary>
    /// 初始化绑定Entity通用组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="entityType"></param>
    /// <param name="bindGo"></param>
    /// <param name="IsAutoDestroyBindGo"></param>
    private static void InitBindEntityCommonComponents(BaseEntity entity, EntityType entityType, GameObject bindGo, bool IsAutoDestroyBindGo)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(entityType);
        var positionComponent = ComponentUtilities.CreatePositionComponent();
        var rotationComponent = ComponentUtilities.CreateRotationComponent();
        var gameObjectComponent = ComponentUtilities.CreateGameObjectComponent(bindGo);
        var gameObjectSyncComponent = ComponentUtilities.CreateGameObjectSyncComponent();
        var animatorPlayerComponent = ComponentUtilities.CreateAnimatorPlayComponent();
        var gameObjectBindComponent = ComponentUtilities.CreateGameObjectBindComponent();
        gameObjectBindComponent.IsAutoDestroyBindGo = IsAutoDestroyBindGo;
        entity.AddComponents(entityTypeComponent, positionComponent, rotationComponent, gameObjectComponent, gameObjectSyncComponent, animatorPlayerComponent, gameObjectBindComponent);
    }

    /// <summary>
    /// 初始化PlayerEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitPlayerEntityComponents(PlayerEntity entity, params object[] parameters)
    {
        var prefabPath = parameters[0] as string;
        InitActorEntityCommonComponents(entity, EntityType.Player, prefabPath);
    }
    
    /// <summary>
    /// 初始化MonsterEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitMonsterEntityComponents(MonsterEntity entity, params object[] parameters)
    {
        var prefabPath = parameters[0] as string;
        InitActorEntityCommonComponents(entity, EntityType.Monster, prefabPath);        
    }
    
    /// <summary>
    /// 初始化TreasureBoxEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitTreasureBoxEntityComponents(TreasureBoxEntity entity, params object[] parameters)
    {
        var prefabPath = parameters[0] as string;
        InitActorEntityCommonComponents(entity, EntityType.TreasureBox, prefabPath);
    }
    
    /// <summary>
    /// 初始化TrapEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitTrapEntityComponents(TrapEntity entity, params object[] parameters)
    {
        var prefabPath = parameters[0] as string;
        InitActorEntityCommonComponents(entity, EntityType.Trap, prefabPath);        
    }
    
    /// <summary>
    /// 初始化CameraEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitCameraEntityComponents(CameraEntity entity, params object[] parameters)
    {
        var cameraGameObject = parameters[0] as GameObject;
        var isAutoDestroyBindGo = (bool)parameters[1];
        InitBindEntityCommonComponents(entity, EntityType.Camera, cameraGameObject, isAutoDestroyBindGo); 
    }
    
    /// <summary>
    /// 初始化MapGameEntity组件
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="parameters"></param>
    private static void InitMapGameEntityComponents(MapGameEntity entity, params object[] parameters)
    {
        var entityTypeComponent = ComponentUtilities.CreateEntityTypeComponent(EntityType.MapGame);
        entity.AddComponents(entityTypeComponent);
    }
    #endregion

    #region Entity公共逻辑方法
    /// <summary>
    /// 获取指定Entity的根GameObject名字
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string GetEntityRootGameObjectName(BaseEntity entity)
    {
        if(entity == null)
        {
            Debug.LogError($"不允许获取空Entity的根GameObject名字！");
            return string.Empty;
        }
        var classType = entity.ClassType;
        var classTypeName = classType.Name;
        var entityUuid = entity.Uuid;
        var rootGameObjectName = $"{classTypeName}_{entityUuid}";
        return rootGameObjectName;
    }

    /// <summary>
    /// 临时位置
    /// </summary>
    private static Vector3 TempPosition = Vector3.zero;

    /// <summary>
    /// 设置Entity在NavMesh上的位置
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetPositionOnNav(BaseEntity entity, float x, float y, float z)
    {
        var positionComponent = entity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有PositionComponent组件，设置NavMesh上的位置失败！");
            return;
        }
        TempPosition.x = x;
        TempPosition.y = y;
        TempPosition.z = z;
        UnityEngine.AI.NavMeshHit navMeshHit;
        UnityEngine.AI.NavMesh.SamplePosition(TempPosition, out navMeshHit, MapGameConst.ActorNavMeshHitDistance, UnityEngine.AI.NavMesh.AllAreas);
        if(!navMeshHit.hit)
        {
            return;
        }
        positionComponent.Position = navMeshHit.position;
        var syncPositionComponent = entity.GetComponent<GameObjectSyncComponent>();
        if(syncPositionComponent != null)
        {
            syncPositionComponent.SyncPosition = true;
        }
    }

    /// <summary>
    /// 设置Entity位置
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetPosition(BaseEntity entity, float x, float y, float z)
    {
        var positionComponent = entity.GetComponent<PositionComponent>();
        if(positionComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有PositionComponent组件，设置位置失败！");
            return;
        }
        positionComponent.Position = new Vector3(x, y, z);
        var gameObjectSyncComponent = entity.GetComponent<GameObjectSyncComponent>();
        if(gameObjectSyncComponent != null)
        {
            gameObjectSyncComponent.SyncPosition = true;
        }
    }
    
    /// <summary>
    /// 设置Entity旋转
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetRotation(BaseEntity entity, float x, float y, float z)
    {
        var rotationComponent = entity.GetComponent<RotationComponent>();
        if(rotationComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有RotationComponent组件，设置旋转失败！");
            return;
        }
        rotationComponent.Rotation = new Vector3(x, y, z);
        var gameObjectSyncComponent = entity.GetComponent<GameObjectSyncComponent>();
        if(gameObjectSyncComponent != null)
        {
            gameObjectSyncComponent.SyncRotation = true;
        }
    }

    /// <summary>
    /// 设置Entity的GameObjectComponent数据
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="gameObject"></param>
    /// <param name="prefabPath"></param>
    public static void SetGameObjectComponent(BaseEntity entity, GameObject gameObject, string prefabPath = null)
    {
        var gameObjectComponent = entity.GetComponent<GameObjectComponent>();
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有GameObjectComponent组件，设置GameObjectComponent数据失败！");
            return;
        }
        gameObjectComponent.Go = gameObject;
        gameObjectComponent.PrefabPath = prefabPath;
    }

    /// <summary>
    /// 设置Entity的GameObject位置
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetGoPosition(BaseEntity entity, float x, float y, float z)
    {
        var gameObjectComponent = entity.GetComponent<GameObjectComponent>();
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有GameObjectComponent组件，设置GameObject位置失败！");
            return;
        }
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}的GameObjectComponent没有Go对象，设置GameObject位置失败！");
            return;
        }
        gameObjectComponent.Go.transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// 设置Entity的GameObject旋转
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="gameObject"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="z"></param>
    public static void SetGoRotation(BaseEntity entity, float x, float y, float z)
    {
        var gameObjectComponent = entity.GetComponent<GameObjectComponent>();
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有GameObjectComponent组件，设置GameObject位置失败！");
            return;
        }
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}的GameObjectComponent没有Go对象，设置GameObject位置失败！");
            return;
        }
        gameObjectComponent.Go.transform.position = new Vector3(x, y, z);
    }

    /// <summary>
    /// Entity播放指定动画名
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="animName"></param>
    public static void PlayAnim(BaseEntity entity, string animName)
    {
        var animatorPlayerComponent = entity.GetComponent<AnimatorPlayComponent>();
        if(animatorPlayerComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有AnimatorPlayComponent组件，播放动画失败！");
            return;
        }
        animatorPlayerComponent.PlayAnimName = animName;
        if(animatorPlayerComponent.Animator != null && !string.IsNullOrEmpty(animatorPlayerComponent.PlayAnimName))
        {
            animatorPlayerComponent.Animator.Play(animatorPlayerComponent.PlayAnimName);
        }
    }

    /// <summary>
    /// 销毁Entity实体实例
    /// </summary>
    /// <param name="entity"></param>
    public static void DestroyInstance(BaseEntity entity)
    {
        var gameObjectComponent = entity.GetComponent<GameObjectComponent>();
        if(gameObjectComponent == null)
        {
            Debug.LogError($"Entity类型:{entity.ClassType}，Entity uuid:{entity.Uuid}没有GameObjectComponent组件，销毁实体对象失败！");
            return;
        }
        if(gameObjectComponent.Go != null)
        {
            if(!string.IsNullOrEmpty(gameObjectComponent.PrefabPath))
            {
                PoolManager.Singleton.Push(gameObjectComponent.PrefabPath, gameObjectComponent.Go);
            }
            else
            {
                GameObject.Destroy(gameObjectComponent.Go);
            }
            gameObjectComponent.Go = null;
        }
    }
    #endregion
}
