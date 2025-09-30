/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:37:52
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-24 15:29:00
* @ Description:
*
*/

using System;
using UnityEngine;

/// <summary>
/// BaseBindEntity.cs
/// 带实体对象的Entity基类
/// </summary>
public abstract class BaseBindEntity : BaseEntity
{
    /// <summary>
    /// 绑定GameObject
    /// </summary>
    public GameObject BindGameObject
    {
        get;
        protected set;
    }

    /// <summary>
    /// 是否自动销毁
    /// </summary>
    public bool IsAutoDestroy
    {
        get;
        protected set;
    }

    public BaseBindEntity() : base()
    {

    }

    /// <summary>
    /// 响应销毁
    /// </summary>
    public override void OnDestroy()
    {
        base.OnDestroy();
        if(IsAutoDestroy && BindGameObject != null)
        {
            GameObject.Destroy(BindGameObject);
            BindGameObject = null;
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="bindGameObject"></param>
    /// <param name="isAutoDestroy"></param>
    public void Init(GameObject bindGameObject, bool isAutoDestroy = false)
    {
        BindGameObject = bindGameObject;
        IsAutoDestroy = isAutoDestroy;
    }

    /// <summary>
    /// 获取世界坐标
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldPos()
    {
        if(BindGameObject != null )
        {
            return BindGameObject.transform.position;
        }
        Debug.LogErorr($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，获取世界坐标失败！");
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
        if (BindGameObject != null)
        {
            BindGameObject.transform.position = new Vector3(worldPosX, worldPosY, worldPosZ);
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，设置世界坐标失败！");
        }
    }

    /// <summary>
    /// 设置世界坐标Vector3
    /// </summary>
    /// <param name="worldPosX"></param>
    public void SetWorldPosVector3(Vector3 worldPos)
    {
        if (BindGameObject != null)
        {
            BindGameObject.transform.position = worldPos;
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，设置世界坐标Vector3失败！");
        }
    }

    /// <summary>
    /// 获取世界欧拉角
    /// </summary>
    /// <returns></returns>
    public Vector3 GetWorldRotation()
    {
        if(BindGameObject != null)
        {
            return BindGameObject.transform.eulerAngles;
        }
        Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，获取世界欧拉角失败！");
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
        if (BindGameObject != null)
        {
            BindGameObject.transform.eulerAngles = new Vector3(worldRotationX, worldRotationY, worldRotationZ);
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，设置世界旋转欧拉角失败！");
        }
    }


    /// <summary>
    /// 设置世界旋转欧拉角Vector3
    /// </summary>
    /// <param name="worldRotation"></param>
    public void SetWorldRotationVector3(Vector3 worldRotation)
    {
        if (BindGameObject != null)
        {
            BindGameObject.transform.eulerAngles = worldRotation;
        }
        else
        {
            Debug.LogError($"Class Type:{ClassType}，Entity Uuid:{Uuid}的绑定实体对象不存在，设置世界旋转欧拉角Vector3失败！");
        }
    }
}