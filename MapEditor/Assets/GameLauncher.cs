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
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        GameManager.Singleton.Update(deltaTime);
    }

    /// <summary>
    /// 固定更新
    /// </summary>
    /// <param name="fixedDeltaTime"></param>
    public void FixedUpdate(float fixedDeltaTime)
    {
        GameManager.Singleton.FixedUpdate(fixedDeltaTime);
    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void LateUpdate(float deltaTime)
    {
        GameManager.Singleton.LateUpdate(deltaTime);
    }
}
