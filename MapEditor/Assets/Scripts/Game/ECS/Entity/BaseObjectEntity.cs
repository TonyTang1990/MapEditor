/**
* @ Author: TONYTANG
* @ Create Time: 2025-09-30 16:00:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-09-30 16:00:00
* @ Description:
*
*/

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Numerics;
using UnityEngine;

/// <summary>
/// BaseObjectEntity.cs
/// 
/// </summary>
public abstract class BaseObjectEntity : BaseEntity
{
    /// <summary>
    /// 根GameObject
    /// </summary>
    public GameObject RootGameObject
    {
        get;
        set;
    }

    /// <summary>
    /// 根预制件路径
    /// </summary>
    public string RootPrefabPath
    {
        get;
        set;
    }

    /// <summary>
    /// 实体GameObject
    /// </summary>
    public GameObject ObjectGameObject
    {
        get;
        set;
    }

    /// <summary>
    /// 实体预制件路径
    /// </summary>
    public string ObjectPrefabPath
    {
        get;
        protected set;
    }

    /// <summary>
    /// 根GameObject组件类型Map<组件类型信息，缓存组件>
    /// </summary>
    public Dictionary<Type, Component> RootComponentMap
    {
        get;
        protected set;
    }

    /// <summary>
    /// Object组件Map<节点相对路径, <组件类型信息, 缓存组件>>
    /// </summary>
    public Dictionary<string, Dictionary<Type, Component>> ObjectComponentMap
    {
        get;
        protected set;
    }

    /// <summary>
    /// 播放动画名
    /// </summary>
    public string PlayAnimName
    {
        get;
        set;
    }

    public BaseObjectEntity() : base()
    {
        RootComponentMap = new Dictionary<Type, Component>();
        ObjectComponentMap = new Dictionary<string, Dictionary<Type, Component>>();
    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        // 回收根GameObject和实体GameObject
        PoolManager.Singleton.Push(ObjectPrefabPath, ObjectGameObject);
        ObjectGameObject = null;
        ObjectPrefabPath = null;

        PoolManager.Singleton.Push(RootPrefabPath, RootGameObject);
        RootGameObject = null;
        RootPrefabPath = null;
    }

    /// <summary>
    /// 响应入池
    /// </summary>
    public override void OnDispose()
    {
        base.OnDispose();
        ClearRootComponentCache();
        ClearObjectComponentCache();
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="objectPrefabPath"></param>
    public void Init(string objectPrefabPath)
    {
        ObjectPrefabPath = objectPrefabPath;
    }

    /// <summary>
    /// 清理根Component缓存
    /// </summary>
    public void ClearRootComponentCache()
    {
        RootComponentMap.Clear();
    }

    /// <summary>
    /// 清理Object组件缓存
    /// </summary>
    public void ClearObjectComponentCache()
    {
        ObjectComponentMap.Clear();
    }

    /// <summary>
    /// 获取世界坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPos()
    {
        if (RootGameObject != null)
        {
            return RootGameObject.transform.position;
        }
        Debug.LogErorr($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，获取世界坐标失败！");
        return Vector3.zero;
    }

    /// <summary>
    /// 设置世界坐标
    /// </summary>
    /// <param name="worldPosX"></param>
    /// <param name="worldPosY"></param>
    /// <param name="worldPosZ"></param>
    public void SetWorldPos(float worldPosX, float worldPosY, float worldPosZ)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.position = new Vector3(worldPosX, worldPosY, worldPosZ);
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置世界坐标失败！");
        }
    }

    /// <summary>
    /// 设置世界坐标Vector3
    /// </summary>
    /// <param name="worldPosX"></param>
    public void SetWorldPosVector3(Vector3 worldPos)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.position = worldPos;
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置世界坐标Vector3失败！");
        }
    }

    /// <summary>
    /// 获取世界欧拉角
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldRotation()
    {
        if (RootGameObject != null)
        {
            return RootGameObject.transform.eulerAngles;
        }
        Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，获取世界欧拉角失败！");
        return Vector3.zero;
    }

    /// <summary>
    /// 设置世界旋转欧拉角
    /// </summary>
    /// <param name="worldRotationX"></param>
    /// <param name="worldRotationY"></param>
    /// <param name="worldRotationZ"></param>
    public void SetWorldRotation(float worldRotationX, float worldRotationY, float worldRotationZ)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.eulerAngles = new Vector3(worldRotationX, worldRotationY, worldRotationZ);
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置世界旋转欧拉角失败！");
        }
    }


    /// <summary>
    /// 设置世界旋转欧拉角Vector3
    /// </summary>
    /// <param name="worldRotation"></param>
    public void SetWorldRotationVector3(Vector3 worldRotation)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.eulerAngles = worldRotation;
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置世界旋转欧拉角Vector3失败！");
        }
    }

    /// <summary>
    /// 设置局部缩放
    /// </summary>
    /// <param name="localScaleX"></param>
    /// <param name="localScaleY"></param>
    /// <param name="localScaleZ"></param>
    public void SetLocalScale(float localScaleX, float localScaleY, float localScaleZ)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.localScale = new Vector3(localScaleX, localScaleY, localScaleZ);
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置局部缩放失败！");
        }
    }

    /// <summary>
    /// 设置局部缩放Vector3
    /// </summary>
    /// <param name="localScale"></param>
    public void SetLocalScaleVector3(Vector3 localScale)
    {
        if (RootGameObject != null)
        {
            RootGameObject.transform.localScale = localScale;
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，设置局部缩放Vector3失败！");
        }
    }

    /// <summary>
    /// 播放指定动画
    /// </summary>
    /// <param name="animName"></param>
    public void PlayAnim(string animName)
    {
        PlayAnimName = animName;
        var objectAnimator = GetRootComponent<Animator>();
        if(objectAnimator != null)
        {
            objectAnimator.Play(PlayAnimName);
        }
    }

    /// <summary>
    /// 获取根节点指定类型组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T GetRootComponent<T>() where T : Component
    {
        if(RootGameObject == null)
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象不存在，获取根组件失败！");
            return null;
        }
        Component targetComponent;
        var targetType = typeof(T);
        if(RootComponentMap.TryGetValue(targetType, out targetComponent))
        {
            return targetComponent as T;
        }
        targetComponent = RootGameObject.GetComponent<T>();
        if(targetComponent == null)
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的根实体对象找不到组件类型:{targetType.Name}的组件，获取根组件失败！");
            return null;
        }
        return targetComponent as T;
    }

    /// <summary>
    /// 获取实体对象指定相对节点路径的指定类型组件
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="childRelativePath">节点相对路径(不传表示获取实体对象自身组件)</param>
    /// <returns></returns>
    public T GetObjectChildComponent<T>(string childRelativePath = "") where T : Component
    {
        if (ObjectGameObject == null)
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的实体对象不存在，获取实体对象组件失败！");
            return null;
        }
        Dictionary<Type, Component> childComponentMap;
        if (!ObjectComponentMap.TryGetValue(childRelativePath, out childComponentMap))
        {
            childComponentMap = new Dictionary<Type, Component>();
            ObjectComponentMap.Add(childRelativePath, childComponentMap);
        }
        var targetType = typeof(T);
        Component targetComponent;
        if (childComponentMap.TryGetValue(targetType, out targetComponent))
        {
            return targetComponent as T;
        }
        if(string.IsNullOrEmpty(childRelativePath))
        {
            targetComponent = ObjectGameObject.GetComponent<T>();
        }
        else
        {
            var childTransform = ObjectGameObject.transform.Find(childRelativePath);
            if(childTransform == null)
            {
                Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的实体对象找不到相对节点路径:{childRelativePath}，获取实体对象组件失败！");
                return null;
            }
            targetComponent = childTransform.GetComponent<T>();
        }
        if (targetComponent == null)
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的实体对象找不到相对节点路径:{childRelativePath}的组件类型:{targetType.Name}的组件，获取实体对象组件失败！");
            return null;
        }
        return childComponentMap as T;
    }
}
