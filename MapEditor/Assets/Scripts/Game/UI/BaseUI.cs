/**
* @ Author: TONYTANG
* @ Create Time: 2025-02-17 16:39:04
* @ Modified by: TONYTANG
* @ Modified time: 2025-02-21 17:55:00
* @ Description:
*/

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// BaseUI.cs
/// UI窗口基类抽象
/// </summary>
public abstract class BaseUI
{
    /// <summary>
    /// 窗口名
    /// </summary>
    public string UIName
    {
        get;
        private set;
    }

    /// <summary>
    /// 实例对象
    /// </summary>
    public GameObject Instance
    {
        get;
        private set;
    }

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="uiName">窗口名</param>
    public BaseUI(string uiName)
    {
        UIName = uiName;
    }

    /// <summary>
    /// 响应窗口打开
    /// </summary>
    /// <param name="parent"></param>
    public void OnOpen(Transform parent)
    {
        LoadRes(parent);
    }

    /// <summary>
    /// 加载窗口资源
    /// </summary>
    /// <param name="parent"></param>
    protected void LoadRes(Transform parent)
    {
        var uiPrefabPath = $"{MapGameConst.UIPrefabFolderPath}/{UIName}.prefab";
        Instance = Resources.Load<GameObject>(uiPrefabPath);
        Instance.transform.SetParent(parent);
        OnLoadResComplete();
    }

    /// <summary>
    /// 响应资源加载完成
    /// </summary>
    protected void OnLoadResComplete()
    {
        InitComponents();
    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected virtual void InitComponents()
    {

    }

    /// <summary>
    /// 响应关闭
    /// </summary>
    public void OnClose()
    {

    }

    /// <summary>
    /// 更新
    /// </summary>
    public virtual void Update()
    {

    }
}
