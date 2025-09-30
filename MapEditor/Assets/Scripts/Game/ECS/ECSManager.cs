/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-09-30 16:00:00
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-09-30 16:00:00
 * @ Description:
 */

using System;
using System.Collections.Generic;
using System.Web.Configuration;

// 负责编写ECS统一的一些框架流程和接口

/// <summary>
/// ECSManager.cs
/// ECS管理单例类
/// </summary>
public class ECSManager : SingletonTemplate<ECSManager>
{
    /// <summary>
    /// 获取指定公共预制件名的路径
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    public string GetCommonPrefabPath(string prefabName)
    {
        var prefabPath = Path.Combine(ECSConst.CommonPrefabFolderPath, prefabName);
        return prefabPath;
    }

    /// <summary>
    /// 获取空预制件路径
    /// </summary>
    /// <returns></returns>
    public string GetEmptyPrefabPath()
    {
        return GetCommonPrefabPath(ECSConst.EmptyPrefabName);
    }

    /// <summary>
    /// 根据路径加载BaseObjectEntity预制体
    /// </summary>
    /// <param name="prefabPath"></param>
    /// <param name="entity"></param>
    /// <param name="parent"></param>
    /// <param name="worldPos"></param>
    /// <param name="eulerAngels"></param>
    /// <param name="loadCompleteCb"></param>
    public void LoadObjectEntityPrefabByPath(string prefabPath, BaseObjectEntity entity, Transform parent, Vector3 worldPos, Vector3 eulerAngels, Action<BaseObjectEntity> loadCompleteCb = null)
    {
        entity.RootPrefabPath = prefabPath;
        PoolManager.Singleton.Pop(prefabPath, (rootInstance) =>
        {
            var rootGoName = EntityUtilities.GetEntityRootGameObjectName(entity);
            rootInstance.name = rootGoName;
            entity.RootGameObject = rootInstance;
            var rootInstanceTransform = rootInstance.transform;
            if(parent != null)
            {
                rootInstanceTransform.SetParent(parent);
            }
            rootInstanceTransform.position = worldPos;
            rootInstanceTransform.eulerAngles = eulerAngels;
            PoolManager.Singleton.Pop(entity.ObjectPrefabPath, (objcetInstance) =>
            {
                var objectGoName = EntityUtilities.GetEntityObjectName(entity);
                rootInstance.name = objectGoName;
                entity.RootGameObject = rootInstance;
                var rootInstanceTransform = rootInstance.transform;
            });
        });
    }
}
