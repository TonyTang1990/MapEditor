/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-10-13 10:55:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-10-13 10:55:00
 * @ Description:
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseEntityView.cs
/// Entity可视化显示基类抽象
/// </summary>
public abstract class BaseEntityView<T> : MonoBehaviour where T : BaseEntity
{
    /// <summary>
    /// 所属Entity
    /// </summary>
    public T OwnerEntity
    {
        get;
        private set;
    }

    /// <summary>
    /// Entity Uuid
    /// </summary>
    [Header("Entity Uuid")]
    public int Uuid;

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="entity"></param>
    public void Init(T entity)
    {
        OwnerEntity = entity;
        Uuid = OwnerEntity.Uuid;
    }

    /// <summary>
    /// 同步数据
    /// </summary>
    public abstract void SyncData();
}