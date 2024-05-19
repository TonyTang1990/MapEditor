/*
 * Description:             CameraRayCastVisualizationMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraRayCastVisualizationMono.cs
/// 摄像机射线可视化Mono脚本
/// </summary>
[RequireComponent(typeof(Camera))]
public class CameraRayCastVisualizationMono : MonoBehaviour
{
    /// <summary>
    /// 摄像机组件
    /// </summary>
    private Camera mCameraComponent;

    /// <summary>
    /// 摄像机射线数据列表
    /// </summary>
    private List<KeyValuePair<Vector3, Vector3>> mRayCastDataList = new List<KeyValuePair<Vector3, Vector3>>();

    void Awake()
    {
        UpdateCameraComponent();
        UpdateRayCastDatas();
    }

    /// <summary>
    /// 更新摄像机组件数据
    /// </summary>
    public void UpdateCameraComponent()
    {
        if(mCameraComponent != null)
        {
            return;
        }
        mCameraComponent = gameObject.GetComponent<Camera>();
    }

    /// <summary>
    /// 更新摄像机射线数据
    /// </summary>
    public void UpdateRayCastDatas()
    {
        mRayCastDataList.Clear();
        var result = CameraUtilities.GetCameraRayCastDataList(mCameraComponent, ref mRayCastDataList);
        if(!result)
        {
            Debug.LogError($"更新摄像机射线数据失败！");
            return;
        }
    }

    private void OnDrawGizmosSelected()
    {
        DrawRayCastLinesGUI();
    }

    /// <summary>
    /// 绘制射线GUI
    /// </summary>
    private void DrawRayCastLinesGUI()
    {
        var preGizmosColor = Gizmos.color;
        Gizmos.color = Color.red;
        for (int i = 0; i < mRayCastDataList.Count; i++)
        {
            Gizmos.DrawLine(mRayCastDataList[i].Key, mRayCastDataList[i].Value);
        }
        Gizmos.color = preGizmosColor;
    }
}