/**
* @ Author: TONYTANG
* @ Create Time: 2025-03-13 18:15:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-03-13 18:15:00
* @ Description:
*
*/

using System;
using System.Collections.Generic;
using System.Text;

/// <summary>
/// EntityViewUtilities.cs
/// Entity可视化工具类
/// </summary>
public static class EntityViewUtilities
{
    /// <summary>
    /// 获取指定Entity的EntityView GameObject名字
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="entity"></param>
    /// <returns></returns>
    public static string GetEntityViewGoName<T>(T entity) where T : BaseEntity
    {
        if (entity == null)
        {
            return string.Empty;
        }
        // 根GameObject名规则 = 类型名 + "_" + Entity Uuid
        var entityUuid = entity.Uuid;
        var entityType = entity.GetType();
        var entityTypeName = entityType.Name;
        var entityViewGoName = $"{entityTypeName}_{entityUuid}";
        return entityViewGoName;
    }
}