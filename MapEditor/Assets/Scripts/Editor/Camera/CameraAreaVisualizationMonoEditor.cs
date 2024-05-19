/*
 * Description:             CameraAreaVisualizationMonoEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/19
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// CameraAreaVisualizationMonoEditor.cs
/// </summary>
[CustomEditor(typeof(CameraAreaVisualizationMono))]
[ExecuteInEditMode]
public class CameraAreaVisualizationMonoEditor : Editor
{
    /// <summary>
    /// 目标组件
    /// </summary>
    private CameraAreaVisualizationMono mTarget;

    private void Awake()
    {
        UpdateTarget();
        mTarget?.UpdateCameraComponent();
        mTarget?.UpdateAreaDatas();
    }

    /// <summary>
    /// 更新Target对象
    /// </summary>
    private void UpdateTarget()
    {
        mTarget = target as CameraAreaVisualizationMono;
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
        mTarget?.UpdateAreaDatas();
    }
}