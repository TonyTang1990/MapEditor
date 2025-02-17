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
    /// <summary>
    /// 主摄像机
    /// </summary>
    [Header("主摄像机")]
    public Camera MainCamera;

    /// <summary>
    /// 左下区域顶点坐标文本
    /// </summary>
    [Header("左下区域顶点坐标文本")]
    public Text LeftBottomPointTxt;

    /// <summary>
    /// 左上区域顶点坐标文本
    /// </summary>
    [Header("左上区域顶点坐标文本")]
    public Text LeftTopPointTxt;

    /// <summary>
    /// 右上区域顶点坐标文本
    /// </summary>
    [Header("右上区域顶点坐标文本")]
    public Text RightTopPointTxt;

    /// <summary>
    /// 右下区域顶点坐标文本
    /// </summary>
    [Header("右下区域顶点坐标文本")]
    public Text RightBottomPointTxt;

    /// <summary>
    /// 中间区域顶点坐标文本
    /// </summary>
    [Header("中间区域顶点坐标文本")]
    public Text CenterPointTxt;

    /// <summary>
    /// FPS文本
    /// </summary>
    [Header("FPS文本")]
    public Text FPSTxt;

    /// <summary>
    /// 逻辑帧率文本
    /// </summary>
    [Header("逻辑帧率文本")]
    public Text LogicFrameTxt;

    /// <summary>
    /// 摄像机区域顶点坐标列表
    /// </summary>
    private List<Vector3> mCameraAreaPointsList = new List<Vector3>();


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
        UpdateCameraAreaInfoss();
        UpdateGameInfos();
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

    /// <summary>
    /// 更新摄像机区域信息
    /// </summary>
    private void UpdateCameraAreaInfoss()
    {
        CameraUtilities.GetCameraVisibleArea(MainCamera, Vector3.zero, Vector3.up, ref mCameraAreaPointsList);
        
    }

    /// <summary>
    /// 更新游戏信息
    /// </summary>
    private void UpdateGameInfos()
    {

    }
}
