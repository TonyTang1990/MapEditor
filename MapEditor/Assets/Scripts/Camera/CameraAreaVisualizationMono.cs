/*
 * Description:             CameraAreaVisualizationMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

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
    /// 摄像机指定平面映射区域线段数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mAreaLinesList = new List<KeyValuePair<Vector3, Vector3>>();

    /// <summary>
    /// 摄像机射线数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mRayCastDataList = new List<KeyValuePair<Vector3, Vector3>>();

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
        mRayCastDataList.Clear();
        CameraUtilities.GetCameraRayCastDataList(mCameraComponent, ref mRayCastDataList);
        mAreaPointsList.Clear();
        foreach(var rayCastData in mRayCastDataList)
        {
            var rayCastDirection = rayCastData.Value - rayCastData.Key;
            var areaData = Vector3Utilities.GetRayAndPlaneIntersect(rayCastData.Key, rayCastDirection, AreaPoint, AreaNormal);
            if(areaData != null)
            {
                mAreaPointsList.Add((Vector3)areaData);
            }
        }
        mAreaLinesList.Clear();
        for(int i = 0, length = mAreaPointsList.Count; i < length; i++)
        {
            var startPointIndex = i;
            var endPointIndex = (i + 1) % length;
            var startPoint = mAreaPointsList[startPointIndex];
            var endPoint = mAreaPointsList[endPointIndex];
            var areaLine = new KeyValuePair<Vector3, Vector3>(startPoint, endPoint);
            mAreaLinesList.Add(areaLine);
        }
    }


    private void OnDrawGizmos()
    {
        DrawAreaInfoGUI();
        DrawAreaGUI();
    }

    /// <summary>
    /// 绘制区域信息Gizmos
    /// </summary>
    private void DrawAreaInfoGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(AreaPoint, 1);
        var areaPointTo = AreaPoint + AreaNormal * 5;
        Gizmos.DrawLine(AreaPoint, areaPointTo);
        Gizmos.color = preGizmosColor;
    }

    /// <summary>
    /// 绘制区域Gizmos
    /// </summary>
    private void DrawAreaGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.green;
        foreach (var areaData in mAreaPointsList)
        {
            if(areaData == null)
            {
                continue;
            }
            Gizmos.DrawSphere(AreaPoint, 0.5f);
        }
        foreach(var areaLine in mAreaLinesList)
        {
            Gizmos.DrawLine(areaLine.Key, areaLine.Value);
        }
        Gizmos.color = preGizmosColor;
    }
}