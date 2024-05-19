/*
 * Description:             CameravisualizationMonoEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CameraRayCastVisualizationMonoEditor.cs
/// </summary>
[CustomEditor(typeof(CameraRayCastVisualizationMono))]
[ExecuteInEditMode]
public class CameravisualizationMonoEditor : Editor
{
    /// <summary>
    /// 目标组件
    /// </summary>
    private CameraRayCastVisualizationMono mTarget;

    private void Awake()
    {
        UpdateTarget();
        mTarget?.UpdateCameraComponent();
        mTarget?.UpdateRayCastDatas();
    }

    /// <summary>
    /// 更新Target对象
    /// </summary>
    private void UpdateTarget()
    {
        mTarget = target as CameraRayCastVisualizationMono;
    }

    private void OnEnable()
    {
        EditorApplication.update += OnEditorUpdate;
    }

    private void OnDisable()
    {
        EditorApplication.update -= OnEditorUpdate;
    }

    /// <summary>
    /// Editor更新
    /// </summary>
    private void OnEditorUpdate()
    {
        mTarget?.UpdateCameraComponent();
        mTarget?.UpdateRayCastDatas();
    }
}