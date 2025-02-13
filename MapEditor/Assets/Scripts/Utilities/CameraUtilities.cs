﻿/*
 * Description:             CameraUtilities.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// CameraUtilities.cs
/// 摄像机静态工具类
/// </summary>
public static class CameraUtilities
{
    /// <summary>
    /// 四个角的视口坐标
    /// </summary>
    private static Vector3[] ViewPortPoints = new Vector3[5]
    {
        new Vector3(0, 0, 0),        // 左下角
        new Vector3(0, 1, 0),        // 左上角
        new Vector3(1, 1, 0),        // 右上角
        new Vector3(1, 0, 0),        // 右下角
        new Vector3(0.5f, 0.5f, 0),  // 中心
    };

    /// <summary>
    /// 获取指定摄像机的射线数据列表
    /// </summary>
    /// <param name="camera"></param>
    /// <param name="rayCastDataList"></param>
    /// <returns></returns>
    public static bool GetCameraRayCastDataList(Camera camera, ref List<KeyValuePair<Vector3, Vector3>> rayCastDataList)
    {
        rayCastDataList.Clear();
        if(camera == null)
        {
            Debug.LogError($"不允许传递空摄像机组件，获取摄像机的射线数据列表失败！");
            return false;
        }

        var cameraNearClipPlane = camera.nearClipPlane;
        ViewPortPoints[0].z = cameraNearClipPlane;
        ViewPortPoints[1].z = cameraNearClipPlane;
        ViewPortPoints[2].z = cameraNearClipPlane;
        ViewPortPoints[3].z = cameraNearClipPlane;
        ViewPortPoints[4].z = cameraNearClipPlane;

        var isOrthographic = camera.orthographic;
        if(isOrthographic)
        {
            // 转换为射线
            for (int i = 0; i < ViewPortPoints.Length; i++)
            {
                Ray ray = camera.ViewportPointToRay(ViewPortPoints[i]);
                var rayCastToPoint = ray.origin + ray.direction * camera.farClipPlane;
                var rayCastData = new KeyValuePair<Vector3, Vector3>(ray.origin, rayCastToPoint);
                rayCastDataList.Add(rayCastData);
            }
        }
        else
        {
            var radio = camera.farClipPlane / cameraNearClipPlane;
            var cameraPosition = camera.transform.position;

            // 获取饰扣四个角的屏幕映射世界坐标
            var lbNearPlaneWorldPoints = camera.ViewPortToWorldPoint(ViewPortPoints[0]);
            var ltNearPlaneWorldPoints = camera.ViewPortToWorldPoint(ViewPortPoints[1]);
            var rtNearPlaneWorldPoints = camera.ViewPortToWorldPoint(ViewPortPoints[2]);
            var rbNearPlaneWorldPoints = camera.ViewPortToWorldPoint(ViewPortPoints[3]);
            var ctNearPlaneWorldPoints = camera.ViewPortToWorldPoint(ViewPortPoints[4]);

            var lbNearPlaneCameraWorldPointDir = lbNearPlaneWorldPoints - cameraPosition;
            var ltNearPlaneCameraWorldPointDir = ltNearPlaneWorldPoints - cameraPosition;
            var rtNearPlaneCameraWorldPointDir = rtNearPlaneWorldPoints - cameraPosition;
            var rbNearPlaneCameraWorldPointDir = rbNearPlaneWorldPoints - cameraPosition;
            var ctNearPlaneCameraWorldPointDir = ctNearPlaneWorldPoints - cameraPosition;

            var lbFarPlaneWorldPoint = cameraPosition + lbNearPlaneCameraWorldPointDir * radio;
            var ltFarPlaneWorldPoint = cameraPosition + ltNearPlaneCameraWorldPointDir * radio;
            var rtFarPlaneWorldPoint = cameraPosition + rtNearPlaneCameraWorldPointDir * radio;
            var rbFarPlaneWorldPoint = cameraPosition + rbNearPlaneCameraWorldPointDir * radio;
            var ctFarPlaneWorldPoint = cameraPosition + ctNearPlaneCameraWorldPointDir * radio;

            var lbRayCastData = new KeyValuePair<Vector3, Vector3>(lbNearPlaneWorldPoints, lbFarPlaneWorldPoint);
            var ltRayCastData = new KeyValuePair<Vector3, Vector3>(ltNearPlaneWorldPoints, ltFarPlaneWorldPoint);
            var rtRayCastData = new KeyValuePair<Vector3, Vector3>(rtNearPlaneWorldPoints, rtFarPlaneWorldPoint);
            var rbRayCastData = new KeyValuePair<Vector3, Vector3>(rbNearPlaneWorldPoints, rbFarPlaneWorldPoint);        
            var ctRayCastData = new KeyValuePair<Vector3, Vector3>(ctNearPlaneWorldPoints, ctFarPlaneWorldPoint);        
            rayCastDataList.Add(lbRayCastData);
            rayCastDataList.Add(ltRayCastData);
            rayCastDataList.Add(rtRayCastData);
            rayCastDataList.Add(rbRayCastData);
            rayCastDataList.Add(ctRayCastData);
        }
        return true;
    }
}