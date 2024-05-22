/*
 * Description:             ColliderDataMonoEditor.cs
 * Author:                  TONYTANG
 * Create Date:             2024/05/22
 */

using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// ColliderDataMonoEditor.cs
    /// ColliderDataMono Editor扩展
    /// </summary>
    [CustomEditor(typeof(ColliderDataMono))]
    public class ColliderDataMonoEditor : Editor
    {
        /// <summary>
        /// 目标组件对象
        /// </summary>
        private ColliderDataMono mTarget;

        /// <summary>
        /// 碰撞器类型属性
        /// </summary>
        private SerializedProperty mColliderTypeProperty;

        /// <summary>
        /// 碰撞体中心位置属性
        /// </summary>
        private SerializedProperty mCenterProperty;

        /// <summary>
        /// 碰撞体大小属性
        /// </summary>
        private SerializedProperty mSizeProperty;

        /// <summary>
        /// 碰撞体半径属性
        /// </summary>
        private SerializedProperty mRadiusProperty;

        private void Awake()
        {
            InitTarget();
            InitProperties();
        }

        /// <summary>
        /// 初始化目标组件对象
        /// </summary>
        private void InitTarget()
        {
            mTarget ??= (target as ColliderDataMono);
        }

        /// <summary>
        /// 初始化属性
        /// </summary>
        private void InitProperties()
        {
            mColliderTypeProperty ??= serializedObject.FindProperty("ColliderType");
            mCenterProperty ??= serializedObject.FindProperty("Center");
            mSizeProperty ??= serializedObject.FindProperty("Size");
            mRadiusProperty ??= serializedObject.FindProperty("Radius");
        }

        /// <summary>
        /// Inspector自定义显示
        /// </summary>
        public override void OnInspectorGUI()
        {
            InitTarget();
            InitProperties();

            // 确保对SerializedObject和SerializedProperty的数据修改每帧同步
            serializedObject.Update();

            EditorGUILayout.BeginVertical();
            EditorGUILayout.PropertyField(mColliderTypeProperty);
            EditorGUILayout.PropertyField(mCenterProperty);
            EditorGUILayout.PropertyField(mSizeProperty);
            EditorGUILayout.PropertyField(mRadiusProperty);

            if (GUILayout.Button("自动根据Mesh填充", GUILayout.ExpandWidth(true)))
            {
                DoAutomaticFullfillData();
            }
            EditorGUILayout.EndVertical();

            // 确保对SerializedObject和SerializedProperty的数据修改写入生效
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// 执行自动根据Mesh填充数据
        /// </summary>
        private void DoAutomaticFullfillData()
        {
            if (mTarget == null || mCenterProperty == null || mSizeProperty == null)
            {
                Debug.LogError($"目标组件或Center属性或Size属性为空，自动根据Mesh填充数据失败！");
                return;
            }
            var meshFilters = mTarget.GetComponentsInChildren<MeshFilter>();
            if (meshFilters == null || meshFilters.Length == 0)
            {
                Debug.LogError($"目标对象子节点找不到任何MeshFilter组件，自动根据Mesh填充数据失败！");
                return;
            }
            List<Mesh> allMeshList = new List<Mesh>();
            foreach (var meshFilter in meshFilters)
            {
                if (meshFilter.sharedMesh != null)
                {
                    allMeshList.Add(meshFilter.sharedMesh);
                }
            }
            if (allMeshList.Count == 0)
            {
                Debug.LogError($"目标对象组件子节点找不到任何MeshFilter组件有有效Mesh，自动根据Mesh填充数据失败！");
                return;
            }
            var meshNum = allMeshList.Count;
            var totalCenter = Vector3.zero;
            var totalSize = Vector3.zero;
            foreach (var mesh in allMeshList)
            {
                totalCenter += mesh.bounds.center;
                totalSize += mesh.bounds.size;
            }
            var size = totalSize / meshNum;
            size.x = (float)Math.Round((double)size.x, 2);
            size.y = (float)Math.Round((double)size.y, 2);
            // 为了寻路烘焙考虑，y最低不低于0.5
            size.y = Mathf.Max(size.y, 0.5f);
            size.z = (float)Math.Round((double)size.z, 2);
            var center = totalCenter / meshNum;
            center.y = center.y / 2;
            center.x = (float)Math.Round((double)center.x, 2);
            center.x = (float)Math.Round((double)center.x, 2);
            center.x = (float)Math.Round((double)center.x, 2);
            mCenterProperty.vector3Value = center;
            mSizeProperty.vector3Value = size;
            var radius = Mathf.Max(size.x, size.y, size.z) / 2;
            mRadiusProperty.floatValue = radius;
            //Debug.Log($"Center:{center.ToString()} Size:{size.ToString()} Radius:{radius}");
        }
    }
}