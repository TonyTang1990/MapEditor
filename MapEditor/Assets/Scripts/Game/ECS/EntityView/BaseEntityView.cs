/*
 * Description:             BaseEntityView.cs
 * Author:                  TONYTANG
 * Create Date:             2025/10/13
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseEntityView.cs
/// Entity可视化显示基类抽象
/// </summary>
public abstract class BaseEntityView : MonoBehaviour
{
    /// <summary>
    /// Entity Uuid
    /// </summary>
    [Header("Entity Uuid")]
    public int Uuid;

    /// <summary>
    /// EntityView类型信息
    /// </summary>
    public Type ClassType
    {
        get
        {
            if (mClassType == null)
            {
                mClassType = GetType();
            }
            return mClassType;
        }
    }
    protected Type mClassType;

    /// <summary>
    /// 设置Entity Uuid
    /// </summary>
    /// <param name="entityUuid"></param>
    protected void SetEntityUuid(int entityUuid)
    {
        Uuid = entityUuid;
    }

    /// <summary>
    /// 同步数据
    /// </summary>
    public abstract void SyncData();
}