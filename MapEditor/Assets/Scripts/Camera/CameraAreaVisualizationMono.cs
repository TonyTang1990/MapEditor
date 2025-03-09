/*
 * Description:             CameraAreaVisualizationMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraAreaVisualizationMono.cs
/// 摄像机指定平面映射可视化Mono脚本
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraAreaVisualizationMono : MonoBehaviour
{
    /// <summary>
    /// 区域顶点
    /// </summary>
    [Header("区域顶点")]
    public Vector3 AreaPoint = Vector3.zero;

    /// <summary>
    /// 区域法线
    /// </summary>
    [Header("区域法线")]
    public Vector3 AreaNormal = Vector3.up;

    /// <summary>
    /// 摄像机组件
    /// </summary>
    private Camera mCameraComponent;

    /// <summary>
    /// 摄像机指定平面映射区域顶点数据列表
    /// </summary>
    private List<Vector3> mAreaPointsList = new List<Vector3>();

    /// <summary>
    /// 摄像机指定平面映射矩形区域顶点数据列表
    /// </summary>
    private List<Vector3> mRectAreaPointsList = new List<Vector3>();

    /// <summary>
    /// 摄像机指定平面映射区域线段数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mAreaLinesList = new List<KeyValuePair<Vector3, Vector3>>();

    /// <summary>
    /// 摄像机指定平面映射矩形区域线段数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mRectAreaLinesList = new List<KeyValuePair<Vector3, Vector3>>();

    /// <summary>
    /// 摄像机射线数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mRayCastDataList = new List<KeyValuePair<Vector3, Vector3>>();

    /// <summary>
    /// Gizmos球体大小
    /// </summary>
    private const float SphereSize = 0.5f;

    void Awake()
    {
        UpdateCameraComponent();
        UpdateAreaDatas();
    }

    /// <summary>
    /// 更新摄像机组件数据
    /// </summary>
    public void UpdateCameraComponent()
    {
        if (mCameraComponent != null)
        {
            return;
        }
        mCameraComponent = gameObject.GetComponent<Camera>();
    }

    /// <summary>
    /// 更新摄像机指定平面映射区域数据
    /// </summary>
    public void UpdateAreaDatas()
    {
        CameraUtilities.GetCameraRayCastDataList(mCameraComponent, ref mRayCastDataList);
        CameraUtilities.GetCameraVisibleArea(mCameraComponent, AreaPoint, AreaNormal, ref mAreaPointsList, ref mRectAreaPointsList);
        mAreaLinesList.Clear();
        mRectAreaLinesList.Clear();
        if(mAreaPointsList.Count > 0)
        {
            var lbToLtLine = new KeyValuePair<Vector3, Vector3>(mAreaPointsList[0], mAreaPointsList[1]);
            var ltToRtLine = new KeyValuePair<Vector3, Vector3>(mAreaPointsList[1], mAreaPointsList[2]);
            var rtToRbLine = new KeyValuePair<Vector3, Vector3>(mAreaPointsList[2], mAreaPointsList[3]);
            var rbToLbLine = new KeyValuePair<Vector3, Vector3>(mAreaPointsList[3], mAreaPointsList[0]);
            mAreaLinesList.Add(lbToLtLine);
            mAreaLinesList.Add(ltToRtLine);
            mAreaLinesList.Add(rtToRbLine);
            mAreaLinesList.Add(rbToLbLine);

            var rectLbToLtLine = new KeyValuePair<Vector3, Vector3>(mRectAreaPointsList[0], mRectAreaPointsList[1]);
            var rectLtToRtLine = new KeyValuePair<Vector3, Vector3>(mRectAreaPointsList[1], mRectAreaPointsList[2]);
            var rectRtToRbLine = new KeyValuePair<Vector3, Vector3>(mRectAreaPointsList[2], mRectAreaPointsList[3]);
            var rectRbToLbLine = new KeyValuePair<Vector3, Vector3>(mRectAreaPointsList[3], mRectAreaPointsList[0]);
            mRectAreaLinesList.Add(rectLbToLtLine);
            mRectAreaLinesList.Add(rectLtToRtLine);
            mRectAreaLinesList.Add(rectRtToRbLine);
            mRectAreaLinesList.Add(rectRbToLbLine);
        }
    }

    /// <summary>
    /// Gizmos绘制
    /// </summary>

    private void OnDrawGizmos()
    {
        DrawAreaInfoGUI();
        DrawRectAreaGUI();
        DrawAreaGUI();
        DrawCameraGUI();
    }

    /// <summary>
    /// 绘制区域信息Gizmos
    /// </summary>
    private void DrawAreaInfoGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(AreaPoint, SphereSize);
        var areaPointTo = AreaPoint + AreaNormal * 5;
        Gizmos.DrawLine(AreaPoint, areaPointTo);
        Gizmos.color = preGizmosColor;
    }

    /// <summary>
    /// 绘制矩形区域
    /// </summary>
    private void DrawRectAreaGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = new Color(0, 0.5f, 0);
        if(mRectAreaLinesList.Count > 0)
        {
            Gizmos.DrawLine(mRectAreaLinesList[0].Key, mRectAreaLinesList[0].Value);
            Gizmos.DrawLine(mRectAreaLinesList[1].Key, mRectAreaLinesList[1].Value);
            Gizmos.DrawLine(mRectAreaLinesList[2].Key, mRectAreaLinesList[2].Value);
            Gizmos.DrawLine(mRectAreaLinesList[3].Key, mRectAreaLinesList[3].Value);
        }
        Gizmos.color = preGizmosColor;
    }

    /// <summary>
    /// 绘制区域Gizmos
    /// </summary>
    private void DrawAreaGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.green;
        if(mAreaLinesList.Count > 0)
        {
            Gizmos.DrawLine(mAreaLinesList[0].Key, mAreaLinesList[0].Value);
            Gizmos.DrawLine(mAreaLinesList[1].Key, mAreaLinesList[1].Value);
            Gizmos.DrawLine(mAreaLinesList[2].Key, mAreaLinesList[2].Value);
            Gizmos.DrawLine(mAreaLinesList[3].Key, mAreaLinesList[3].Value);
        }
        if(mAreaPointsList.Count > 0)
        {
            Gizmos.DrawSphere(mAreaPointsList[4], SphereSize);
        }
        Gizmos.color = preGizmosColor;
    }

    /// <summary>
    /// 绘制摄像机Gizmos
    /// </summary>
    private void DrawCameraGUI()
    {
        if(mCameraComponent == null)
        {
            return;
        }

        // 摄像机位置
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(mCameraComponent.transform.position, SphereSize);
        Gizmos.color = preGizmosColor;
        
        // 摄像机近截面
        var rayCastDataNum = mRayCastDataList.Count;
        if(rayCastDataNum > 0)
        {
            preGizmosColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(mRayCastDataList[0].Key, mRayCastDataList[1].Key);
            Gizmos.DrawLine(mRayCastDataList[1].Key, mRayCastDataList[2].Key);
            Gizmos.DrawLine(mRayCastDataList[2].Key, mRayCastDataList[3].Key);
            Gizmos.DrawLine(mRayCastDataList[3].Key, mRayCastDataList[0].Key);
            Gizmos.color = preGizmosColor;
        }

        // 摄像机远截面
        if(rayCastDataNum > 0)
        {
            preGizmosColor = Gizmos.color;
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(mRayCastDataList[0].Value, mRayCastDataList[1].Value);
            Gizmos.DrawLine(mRayCastDataList[1].Value, mRayCastDataList[2].Value);
            Gizmos.DrawLine(mRayCastDataList[2].Value, mRayCastDataList[3].Value);
            Gizmos.DrawLine(mRayCastDataList[3].Value, mRayCastDataList[0].Value);
            Gizmos.color = preGizmosColor;
        }

        // 摄像机射线
        if(rayCastDataNum > 0)
        {
            preGizmosColor = Gizmos.color;
            Gizmos.color = Color.red;
            Gizmos.DrawLine(mRayCastDataList[0].Key, mRayCastDataList[0].Value);
            Gizmos.DrawLine(mRayCastDataList[1].Key, mRayCastDataList[1].Value);
            Gizmos.DrawLine(mRayCastDataList[2].Key, mRayCastDataList[2].Value);
            Gizmos.DrawLine(mRayCastDataList[3].Key, mRayCastDataList[3].Value);
            Gizmos.color = preGizmosColor;
        }
    }
}
