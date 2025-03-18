

using UnityEngine;

/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:44:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:44:00
* @ Description:
*
*//// <summary>
  /// ComponentUtilities.cs
  /// 组件工具类
  /// </summary>
public static class ComponentUtilities
{
    #region 组件创建相关
    // Note:
    // 因为BaseComponent设计了池，又不想统一用params object[]的方式初始化数据(考虑到会有封箱装箱问题)
    // 所有的组件构建方法都写在这里

    /// <summary>
    /// 创建EntityTypeComponent
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static EntityTypeComponent CreateEntityTypeComponent(EntityType entityType)
    {
        var entityTypeComponent = ObjectPool.Singleton.Pop<EntityTypeComponent>();
        entityTypeComponent.EntityType = entityType;
        return entityTypeComponent;
    }

    /// <summary>
    /// 创建PositionComponent
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    public static PositionComponent CreatePositionComponent(Vector3? position = null)
    {
        var positionComponent = ObjectPool.Singleton.Pop<PositionComponent>();
        positionComponent.Position = position != null ? (Vector3)position : Vector3.zero;
        return positionComponent;
    }

    /// <summary>
    /// 创建RotationComponent
    /// </summary>
    /// <param name="rotation"></param>
    /// <returns></returns>
    public static RotationComponent CreateRotationComponent(Vector3? rotation = null)
    {
        var rotationComponent = ObjectPool.Singleton.Pop<RotationComponent>();
        rotationComponent.Rotation = rotation != null ? (Vector3)rotation : Vector3.zero;
        return rotationComponent;
    }

    /// <summary>
    /// 创建GameObjectComponent
    /// </summary>
    /// <param name="go"></param>
    /// <param name="prefabPath"></param>
    /// <returns></returns>
    public static GameObjectComponent CreateGameObjectComponent(GameObject go = null, string prefabPath = null)
    {
        var gameObjectComponent = ObjectPool.Singleton.Pop<GameObjectComponent>();
        gameObjectComponent.Go = go;
        gameObjectComponent.PrefabPath = prefabPath;
        return gameObjectComponent;
    }

    /// <summary>
    /// 创建GameObjectBindComponent
    /// </summary>
    /// <param name="isAutoDestroyBindGo"></param>
    /// <returns></returns>
    public static GameObjectBindComponent CreateGameObjectBindComponent(bool isAutoDestroyBindGo = false)
    {
        var gameObjectBindComponent = ObjectPool.Singleton.Pop<GameObjectBindComponent>();
        gameObjectBindComponent.IsAutoDestroyBindGo = isAutoDestroyBindGo;
        return gameObjectBindComponent;
    }

    /// <summary>
    /// 创建GameObjectSyncComponent
    /// </summary>
    /// <param name="entityType"></param>
    /// <returns></returns>
    public static GameObjectSyncComponent CreateGameObjectSyncComponent(bool syncPosition = false, bool syncRotation = false)
    {
        var gameObjectSyncComponent = ObjectPool.Singleton.Pop<GameObjectSyncComponent>();
        gameObjectSyncComponent.SyncPosition = syncPosition;
        gameObjectSyncComponent.SyncRotation = syncRotation;
        return gameObjectSyncComponent;
    }

    /// <summary>
    /// 创建AnimatorPlayComponent
    /// </summary>
    /// <param name="animator"></param>
    /// <param name="playAnimName">
    /// <returns></returns>
    public static AnimatorPlayComponent CreateAnimatorPlayComponent(Animator animator = null, string playAnimName = null)
    {
        var animatorPlayComponent = ObjectPool.Singleton.Pop<AnimatorPlayComponent>();
        animatorPlayComponent.Animator = animator;
        animatorPlayComponent.PlayAnimName = playAnimName;
        return animatorPlayComponent;
    }

    /// <summary>
    /// 创建ActorComponent
    /// </summary>
    /// <returns></returns>
    public static ActorComponent CreateActorComponent()
    {
        var actorComponent = ObjectPool.Singleton.Pop<ActorComponent>();
        return actorComponent;
    }
    #endregion
}
