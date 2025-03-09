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
using UnityEngine.UI;

/// <summary>
/// MainUI.cs
/// 主界面
/// </summary>
public class MainUI : BaseUI
{
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

    /// <summary>
    /// 摄像机指定平面映射矩形区域顶点数据列表
    /// </summary>
    private List<Vector3> mCameraRectAreaPointsList = new List<Vector3>();

    /// <summary>
    /// 构造函数
    /// </summary>
    /// <param name="uiName">窗口们</param>
    public MainUI(string uiName) : base(uiName)
    {

    }

    /// <summary>
    /// 初始化组件
    /// </summary>
    protected override void InitComponents()
    {
        base.InitComponents();
        var uiTransform = Instance.transform;
        LeftBottomPointTxt = uiTransform.Find("CameraAreaInfo/_leftBottomPointTxt").GetComponent<Text>();
        LeftTopPointTxt = uiTransform.Find("CameraAreaInfo/_leftTopPointTxt").GetComponent<Text>();
        RightTopPointTxt = uiTransform.Find("CameraAreaInfo/_rightTopPointTxt").GetComponent<Text>();
        RightBottomPointTxt = uiTransform.Find("CameraAreaInfo/_rightBottomTxt").GetComponent<Text>();
        CenterPointTxt = uiTransform.Find("CameraAreaInfo/_centerPointTxt").GetComponent<Text>();
        FPSTxt = uiTransform.Find("CameraAreaInfo/_fpsTxt").GetComponent<Text>();
        LogicFrameTxt = uiTransform.Find("CameraAreaInfo/_frameTxt").GetComponent<Text>();
    }

    /// <summary>
    /// 更新
    /// </summary>
    public override void Update()
    {
        base.Update();
        UpdateCameraAreaInfoss();
        UpdateGameInfos();
    }

    /// <summary>
    /// 更新摄像机区域信息
    /// </summary>
    private void UpdateCameraAreaInfoss()
    {
        var mainCamera = MapGameManager.Singleton.MainCamera;
        CameraUtilities.GetCameraVisibleArea(mainCamera, Vector3.zero, Vector3.up, ref mCameraAreaPointsList, ref mCameraRectAreaPointsList);
        if (mCameraAreaPointsList.Count > 0)
        {
            if (LeftBottomPointTxt != null)
            {
                LeftBottomPointTxt.text = $"左下角顶点坐标:{mCameraAreaPointsList[0].ToString()}"; ;
            }
            if (LeftTopPointTxt != null)
            {
                LeftTopPointTxt.text = $"左上角顶点坐标:{mCameraAreaPointsList[1].ToString()}";
            }
            if (RightTopPointTxt != null)
            {
                RightTopPointTxt.text = $"右上角顶点坐标:{mCameraAreaPointsList[2].ToString()}";
            }
            if (RightBottomPointTxt != null)
            {
                RightBottomPointTxt.text = $"右下角顶点坐标:{mCameraAreaPointsList[3].ToString()}";
            }
            if (CenterPointTxt != null)
            {
                CenterPointTxt.text = $"中间顶点坐标:{mCameraAreaPointsList[4].ToString()}";
            }
        }
    }

    /// <summary>
    /// 更新游戏信息
    /// </summary>
    private void UpdateGameInfos()
    {
        if (FPSTxt != null)
        {
            FPSTxt.text = $"FPS:{GameManager.Singleton.FPS}";
        }
        if (LogicFrameTxt != null)
        {
            LogicFrameTxt.text = $"逻辑帧:{GameManager.Singleton.LogicFrame}";
        }
    }
}
