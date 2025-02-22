/*
 * Description:             GameLauncher.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// GameLauncher.cs
/// 游戏启动器
/// </summary>
public class GameLauncher : MonoBehaviour
{
    private void Start()
    {
        GameManager.Singleton.Init();
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update()
    {
        GameManager.Singleton.Update();
    }

    /// <summary>
    /// 固定更新
    /// </summary>
    public void FixedUpdate()
    {
        GameManager.Singleton.FixedUpdate();
    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    public void LateUpdate()
    {
        GameManager.Singleton.LateUpdate();
    }
}
