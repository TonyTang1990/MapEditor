/*
 * Description:             BaseEntityGenericView.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/14
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseEntityGenericView.cs
/// Entity可视化显示泛型基类
/// </summary>
/// <typeparam name="T"></typeparam>
public abstract class BaseEntityGenericView<T> : BaseEntityView where T : BaseEntity
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
    /// 设置所属Entity
    /// </summary>
    /// <param name="entity"></param>
    public void SetOwnerEntity(T entity)
    {
        OwnerEntity = entity;
        var entityUuid = entity != null ? entity.Uuid : 0;
        SetEntityUuid(entityUuid);
    }
}