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

    /// <summary>
    /// Gizmos绘制
    /// </summary>

    private void OnDrawGizmos()
    {
        DrawAreaInfoGUI();
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
            preGizmosColor.DrawSphere(mAreaPointsList[4], SphereSize);
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
