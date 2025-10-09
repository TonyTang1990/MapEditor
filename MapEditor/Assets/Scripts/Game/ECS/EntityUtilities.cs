/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:15:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:15:00
* @ Description:
*
*/

using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// EntityUtilities.cs
/// 地图游戏Entity基类
/// </summary>
public static class EntityUtilities
{
    /// <summary>
    /// 临时位置
    /// </summary>
    private static Vector3 TempPosition = Vector3.zero;

    #region Entity公共逻辑方法
    /// <summary>
    /// 获取指定Entity的根GameObject名字
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string GetEntityRootGoName(BaseEntity entity)
    {
        if(entity == null)
        {
            Debug.LogError($"不允许获取空Entity的根GameObject名字！");
            return string.Empty;
        }
        var classType = entity.ClassType;
        var classTypeName = classType.Name;
        var entityTypeComponent = entity.GetComponent<EntityTypeComponent>();
        var entityType = entityTypeComponent.EntityType;
        var entityUuid = entity.Uuid;
        // 根GameObject名规则 = 类型名 + "_" + EntityType + "_" + Entity Uuid
        var rootGameObjectName = $"{classTypeName}_{entityType}_{entityUuid}";
        return rootGameObjectName;
    }

    /// <summary>
    /// 获取指定Entity的实体GameObject名字
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string GetEntityObjectGoName(BaseEntity entity)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许获取空Entity的实体对象GameObject名字！");
            return string.Empty;
        }
        var classType = entity.ClassType;
        var classTypeName = classType.Name;
        var entityTypeComponent = entity.GetComponent<EntityTypeComponent>();
        var entityType = entityTypeComponent.EntityType;
        var entityUuid = entity.Uuid;
        // 提示GameObject名规则 = 实体Object前缀名 + "_" + 类型名 + "_" + EntityType + "_" + Entity Uuid
        var objectGameObjectName = $"{ECSConst.ObjectNodePrefixName}_{classTypeName}_{entityType}_{entityUuid}";
        return objectGameObjectName;
    }

    /// <summary>
    /// 设置BaseObjectEntity在NavMesh上的指定世界坐标(SamplePosition + position设置位置)
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="worldPosX"></param>
    /// <param name="worldPosY"></param>
    /// <param name="worldPosZ"></param>
    public static void SetObjectEntityWorldPosOnNav(BaseObjectEntity entity, float worldPosX, float worldPosY, float worldPosZ)
    {
        if(entity == null)
        {
            Debug.LogError($"不允许设置空BaseObjectEntity在NavMesh的指定世界坐标！");
            return;
        }
        TempPosition.x = worldPosX;
        TempPosition.y = worldPosY;
        TempPosition.z = worldPosZ;
        UnityEngine.AI.NavMeshHit navMeshHit;
        UnityEngine.AI.NavMesh.SamplePosition(TempPosition, out navMeshHit, MapGameConst.ActorNavMeshHitDistance, UnityEngine.AI.NavMesh.AllAreas);
        if(!navMeshHit.hit)
        {
            return;
        }
        entity.SetWorldPosVector3(navMeshHit.position);
    }

    /// <summary>
    /// 设置NavMeshAgent的BaseObjectEntity指定偏移移动
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="offsetPos"></param>
    public static void MoveNavMeshAgentEntity(BaseObjectEntity entity, Vector3 offsetPos)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许设置空NavMeshAgent的BaseObjectEntity在NavMesh的指定偏移移动！");
            return;
        }
        var navMeshAgent = entity.GetRootComponent<NavMeshAgent>();
        if(navMeshAgent == null)
        {
            Debug.LogError($"Class Type:{entity.ClassType}，Entity Uuid:{entity.Uuid}找不到根NavMeshAgent组件，NavMesh指定偏移移动失败！");
            return;
        }
        navMeshAgent.Move(offsetPos);
    }

    /// <summary>
    /// 设置BaesBindEntity世界坐标
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="worldPosX"></param>
    /// <param name="worldPosY"></param>
    /// <param name="worldPosZ"></param>
    public static void SetBindEntityWorldPos(BaseBindEntity entity, float worldPosX, float worldPosY, float worldPosZ)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许设置空BaseBindEntity的指定世界坐标！");
            return;
        }
        entity.SetWorldPos(worldPosX, worldPosY, worldPosZ);
    }
    
    /// <summary>
    /// 设置BaseObjectEntity世界旋转
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="worldRotationX"></param>
    /// <param name="worldRotationY"></param>
    /// <param name="worldRotationZ"></param>
    public static void SetObjectEntityWorldRotation(BaseObjectEntity entity, float worldRotationX, float worldRotationY, float worldRotationZ)
    {
        if (entity == null)
        {
            Debug.LogError($"不允许设置空BaseObjectEntity的世界旋转！");
            return;
        }
        entity.SetWorldRotation(worldRotationX, worldRotationY, worldRotationZ);
    }
    #endregion
}
