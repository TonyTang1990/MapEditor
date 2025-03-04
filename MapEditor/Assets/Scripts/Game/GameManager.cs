/*
 * Description:             GameManager.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// GameManager.cs
/// 游戏管理单例类
/// </summary>
public class GameManager : SingletonTemplate<GameManager>
{
    /// <summary>
    /// 当前逻辑帧数
    /// </summary>
    public int LogicFrame
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑帧经历时常
    /// </summary>
    public float LogicFramePassedTime
    {
        get;
        private set;
    }

    /// <summary>
    /// 渲染帧率
    /// </summary>
    public int FPS
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑帧数
    /// </summary>
    private const int LogicFrameRate = 30;

    /// <summary>
    /// 单帧时长
    /// </summary>
    private const float LogicFrameTime = 1f / LogicFrameRate;

    /// <summary>
    /// 渲染帧数
    /// </summary>
    private int mRenderFrameCount;

    /// <summary>
    /// 渲染帧经历时常
    /// </summary>
    private float mRenderFramePassedTime;

    /// <summary>
    /// 初始化
    /// </summary>
    public void Init()
    {
        LogicFrame = 0;
        LogicFramePassedTime = 0;
        mRenderFrameCount = 0;
        mRenderFramePassedTime = 0;
        MapGameManager.Singleton.EnterGame();
    }
    
    /// <summary>
    /// 更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void Update(float deltaTime)
    {
        LogicFramePassedTime += Time.deltaTime;
        while (LogicFramePassedTime >= LogicFrameTime)
        {
            LogicFramePassedTime -= LogicFrameTime;
            LogicUpdate();
        }

        mRenderFrameCount++;
        mRenderFramePassedTime += Time.deltaTime;
        if(mRenderFramePassedTime >= 1)
        {
            FPS = mRenderFrameCount;
            mRenderFramePassedTime -= 1;
            mRenderFrameCount = 0;
        }
        
        MapGameManager.Singleton.Update(deltaTime);
    }

    /// <summary>
    /// 逻辑更新
    /// </summary>
    private void LogicUpdate()
    {
        LogicFrame++;
        MapGameManager.Singleton.LogicUpdate();
    }

    /// <summary>
    /// 固定更新
    /// </summary>
    /// <param name="fixedDeltaTime"></param>
    public void FixedUpdate(float fixedDeltaTime)
    {
        MapGameManager.Singleton.FixedUpdate(fixedDeltaTime);
    }

    /// <summary>
    /// 延迟更新
    /// </summary>
    /// <param name="deltaTime"></param>
    public void LateUpdate(float deltaTime)
    {
        MapGameManager.Singleton.LateUpdate(deltaTime);
    }
}