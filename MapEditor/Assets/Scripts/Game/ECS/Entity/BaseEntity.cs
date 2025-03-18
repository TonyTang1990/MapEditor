

// Note:
// 这里的BaseWorld,BaseSystem,BaseEntity,BaseComponent并非完整的ECS模式
// 只是设计上借鉴ECS和tiny-ecs的概念设计

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/**
 * @ Author: TONYTANG
 * @ Create Time: 2025-02-17 16:25:59
 * @ Modified by: TONYTANG
 * @ Modified time: 2025-03-18 22:58:16
 * @ Description:
 *//// <summary>
   /// BaseEntity.cs
   /// Entity基类(逻辑对象抽象，相当于ECS里的E部分)
public abstract class BaseEntity : IRecycle
{
    /// <summary>
    /// Entity Uuid
    /// </summary>
    public int Uuid
    {
        get;
        private set;
    }

    /// <summary>
    /// Entity类型信息
    /// </summary>
    public Type ClassType
    {
        get
        {
            if(mClassType == null)
            {
                mClassType = GetType();
            }
            return mClassType;
        }
    }
    protected Type mClassType;

    /// <summary>
    /// 组件类型和组件列表映射Map
    /// </summary>
    private Dictionary<Type, List<BaseComponent>> mComponentTypeMap;

    /// <summary>
    /// 构造函数
    /// </summary>
    public BaseEntity()
    {
        mComponentTypeMap =  new Dictionary<Type, List<BaseComponent>>();
    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public virtual void OnDestroy()
    {
        RemoveAllComponents();
    }

    /// <summary>
    /// 出池
    /// </summary>
    public virtual void OnCreate()
    {

    }

    /// <summary>
    /// 入池
    /// </summary>
    public virtual void OnDispose()
    {
        ResetDatas();
    }

    /// <summary>
    /// 重置数据(子类重写)
    /// </summary>
    protected virtual void ResetDatas()
    {
        Uuid = 0;
        mComponentTypeMap.Clear();
    }

    /// <summary>
    /// 设置Uuid
    /// </summary>
    /// <param name="uuid"></param>
    public void SetUuid(int uuid)
    {
        Uuid = uuid;
    }

    #region Component相关
    /// <summary>
    /// 添加指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool AddComponent<T>(T component) where T : BaseComponent
    {
        if(component == null)
        {
            Debug.LogError($"不允许添加空组件，添加组件失败！");
            return false;
        }
        var componentType = component.GetType();
        if(!ECSConst.BaseComponentType.IsAssignableFrom(componentType))
        {
            Debug.LogError($"不允许给Entity添加未继承至BaseComponent的组件类型:{componentType.Name}，添加组件失败！");
            return false;
        }
        List<BaseComponent> componentList;
        if(!mComponentTypeMap.TryGetValue(componentType, out componentList))
        {
            componentList = new List<BaseComponent>();
            mComponentTypeMap.Add(componentType, componentList);
        }
        if(componentList.Contains(component))
        {
            Debug.LogError($"Entity Uuid:{Uuid}重复添加组件类型:{componentType}的组件对象，添加组件失败！");
            return false;
        }
        componentList.Add(component);
        return true;
    }

    /// <summary>
    /// 添加指定组件类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T AddComponent<T>() where T : BaseComponent, new()
    {
        var component = new T();
        var result = AddComponent<T>(component);
        return result ? component : null;
    }

    /// <summary>
    /// 添加指定组件列表
    /// </summary>
    /// <param name="components"></param>
    /// <returns></returns>
    public bool AddComponents(params object[] components)
    {
        if(components == null)
        {
            return true;
        }
        foreach(var component in components)
        {
            AddComponent<BaseComponent>(component as BaseComponent);
        }
        return true;
    }

    /// <summary>
    /// 移除指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool RemoveComponent<T>(T component) where T : BaseComponent
    {
        var componentType = typeof(T);
        var componentList = GetComponentListByType(componentType);
        if(componentList == null)
        {
            Debug.LogError($"Entity Uuid:{Uuid}找不到匹配组件类型:{componentType}的组件对象，移除组件失败！");
            return false;
        }
        var result = componentList.Remove(component);
        if(!result)
        {
            Debug.LogError($"Entity Uuid:{Uuid}匹配的组件类型:{componentType}里找不到对应的组件对象，移除组件失败！");
            return false;
        }
        ObjectPool.Singleton.PushAsObj(component);
        return true;
    }

    /// <summary>
    /// 移除指定组件类型的所有组件
    /// </summary>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public bool RemoveComponentByType(Type componentType)
    {
        var componentList = GetComponentListByType(componentType);
        if(componentList == null)
        {
            return true;
        }
        for(int index = componentList.Count - 1; index >= 0; index--)
        {
            var component = componentList[index];
            componentList.RemoveAt(index);
            ObjectPool.Singleton.PushAsObj(component);
        }
        mComponentTypeMap.Remove(componentType);
        return true;
    }

    /// <summary>
    /// 移除所有组件
    /// </summary>
    public void RemoveAllComponents()
    {
        var componentTypes = mComponentTypeMap.Keys.ToList();
        foreach(var componentType in componentTypes)
        {
            RemoveComponentByType(componentType);
        }
    }

    /// <summary>
    /// 获取或添加指定组件类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetOrAddComponent<T>() where T : BaseComponent, new()
    {
        var componentType = typeof(T);
        List<BaseComponent> componentList;
        if(mComponentTypeMap.TryGetValue(componentType, out componentList))
        {

        }
        T component;
        if(componentList == null || componentList.Count == 0)
        {
            component = AddComponent<T>();
        }
        else
        {
            component = componentList[0] as T;
        }
        return component;
    }

    /// <summary>
    /// 获取指定组件类型的组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetComponent<T>() where T : BaseComponent
    {
        var componentType = typeof(T);
        List<BaseComponent> componentList;
        if(mComponentTypeMap.TryGetValue(componentType, out componentList))
        {
        }
        if(componentList == null)
        {
            return null;
        }
        return componentList.Count > 0 ? componentList[0] as T : null;
    }

    /// <summary>
    /// 获取指定组件类型的组件列表
    /// </summary>
    /// <typeparam name="BaseComponent"></typeparam>
    /// <param name=""></param>
    /// <param name=""></param>
    /// <returns></returns>
    public List<BaseComponent> GetComponentListByType(Type componentType)
    {
        if(componentType == null)
        {
            Debug.LogError($"不允许传递空组件类型，获取指定组件类型的组件列表失败！");
            return null;
        }
        List<BaseComponent> componentList;
        if(mComponentTypeMap.TryGetValue(componentType, out componentList))
        {
        }
        return componentList;
    }

    /// <summary>
    /// 是否包含指定组件类型的组件
    /// </summary>
    /// <param name="componentType"></param>
    /// <returns></returns>
    public bool ContainComponentByType(Type componentType)
    {
        List<BaseComponent> componentList;
        if(!mComponentTypeMap.TryGetValue(componentType, out componentList))
        {
            return false;
        }
        return componentList.Count > 0;
    }

    /// <summary>
    /// 是否包含指定组件类型列表的组件
    /// </summary>
    /// <param name="componentTyes"></param>
    /// <returns></returns>
    public bool ContainComponentByTypes(params object[] componentTyes)
    {
        if(componentTyes == null)
        {
            return true;
        }
        foreach(var componentType in componentTyes)
        {
            if(componentType == null)
            {
                continue;
            }
            var cType = componentType as Type;
            if(cType == null)
            {
                continue;
            }
            if(!ECSConst.BaseComponentType.IsAssignableFrom(cType))
            {
                Debug.LogError($"组件类型:{cType}不是BaseComponent类型，不应该参与组件类型是否包含的判断！");
                continue;
            }
            if(!ContainComponentByType(cType))
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// 是否包含指定组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="component"></param>
    /// <returns></returns>
    public bool ContainComponent<T>(T component) where T : BaseComponent
    {
        var componentType = typeof(T);
        List<BaseComponent> componentList;
        if(!mComponentTypeMap.TryGetValue(componentType, out componentList))
        {
            return false;
        }
        return componentList.Contains(component);
    }
    #endregion
}