

using System;

/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:00:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:00:00
* @ Description:
*
*//// <summary>
  /// ECS常量
  /// </summary>
public static class ECSConst
{
    /// <summary>
    /// BaseEntity类型信息
    /// </summary>
    public static readonly Type BaseEntityType = typeof(BaseEntity);

    /// <summary>
    /// BaseComponent类型信息
    /// </summary>
    public static readonly Type BaseComponentType = typeof(BaseComponent);

    /// <summary>
    /// BaseSystem类型信息
    /// </summary>
    public static readonly Type BaseSystemType = typeof(BaseSystem);

    #region Entity类型信息
    /// <summary>
    /// PlayerEntity类型信息
    /// </summary>
    public static readonly Type PlayerEntityType = typeof(PlayerEntity);

    /// <summary>
    /// MonsterEntity类型信息
    /// </summary>
    public static readonly Type MonsterEntityType = typeof(MonsterEntity);

    /// <summary>
    /// TreasureBoxEntity类型信息
    /// </summary>
    public static readonly Type TreasureBoxEntityType = typeof(TreasureBoxEntity);

    /// <summary>
    /// TrapEntity类型信息
    /// </summary>
    public static readonly Type TrapEntityType = typeof(TrapEntity);

    /// <summary>
    /// CameraEntity类型信息
    /// </summary>
    public static readonly Type CameraEntityType = typeof(CameraEntity);

    /// <summary>
    /// MapGameEntity类型信息
    /// </summary>
    public static readonly Type MapGameEntityType = typeof(MapGameEntity);
    #endregion
}