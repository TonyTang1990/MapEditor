/*
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
        // 获取视口四个角的屏幕坐标
        Vector3[] screenPoints = new Vector3[4];
        screenPoints[0] = new Vector3(0, 0, 0);                        // 左下角
        screenPoints[1] = new Vector3(0, Screen.height, 0);            // 左上角
        screenPoints[2] = new Vector3(Screen.width, Screen.height, 0); // 右上角
        screenPoints[3] = new Vector3(Screen.width, 0, 0);             // 右下角

        // 转换为射线
        for (int i = 0; i < screenPoints.Length; i++)
        {
            Ray ray = camera.ScreenPointToRay(screenPoints[i]);
            var rayCastToPoint = ray.origin + ray.direction * camera.farClipPlane;
            KeyValuePair<Vector3, Vector3> rayCastData = new KeyValuePair<Vector3, Vector3>(ray.origin, rayCastToPoint);
            rayCastDataList.Add(rayCastData);
        }
        return true;
    }
}