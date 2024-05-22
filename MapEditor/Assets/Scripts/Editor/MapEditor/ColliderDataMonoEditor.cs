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
        /// 碰撞器中心点位置属性
        /// </summary>
        private SerializedProperty mCenterProperty;

        /// <summary>
        /// 碰撞器中心店位置属性
        /// </summary>
        private SerializedProperty mSizeProperty;

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
            mCenterProperty ??= serializedObject.FindProperty("Center");
            mSizeProperty ??= serializedObject.FindProperty("Size");
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
            EditorGUILayout.PropertyField(mCenterProperty);
            EditorGUILayout.PropertyField(mSizeProperty);

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
            var meshFilters = mTarget.GetComponentsInChildrend<MeshFilters>();
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
            mSizeProperty.vector3Value = size;
            mCenterProperty.vector3Value = center;
            Debug.Log($"Center:{center.ToString()} Size:{size.ToString()}");
        }
    }
}