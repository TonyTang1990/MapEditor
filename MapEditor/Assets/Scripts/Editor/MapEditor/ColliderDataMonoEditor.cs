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
        /// 碰撞体中心位置属性
        /// </summary>
        private SerializedProperty mCenterProperty;

        /// <summary>
        /// 碰撞体大小属性
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
            EditorGUILayout.PropertyField(mColliderTypeProperty);
            EditorGUILayout.PropertyField(mCenterProperty);
            EditorGUILayout.PropertyField(mSizeProperty);
            EditorGUILayout.PropertyField(mRadiusProperty);

            if (GUILayout.Button("自动根据Renderer计算", GUILayout.ExpandWidth(true)))
            {
                DoAutomaticFullfillData();
            }
            if (GUILayout.Button("自动填充到BoxCollider", GUILayout.ExpandWidth(true)))
            {
                DoAutomaticFullfillBoxCollider();
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
            var renderers = mTarget.GetComponentsInChildren<Renderer>();
            if (renderers == null || renderers.Length == 0)
            {
                Debug.LogError($"目标对象子节点找不到任何Renderer组件，自动根据Renderer填充数据失败！");
                return;
            }
            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var renderer in renderers)
            {
                bounds.Encapsulate(renderer.bounds);
            }
            var size = bounds.size;
            size.x = (float)Math.Round((double)size.x, 2);
            size.y = (float)Math.Round((double)size.y, 2);
            size.z = (float)Math.Round((double)size.z, 2);
            var center = bounds.center;
            center.x = (float)Math.Round((double)center.x, 2);
            center.y = (float)Math.Round((double)center.y, 2);
            center.z = (float)Math.Round((double)center.z, 2);
            mCenterProperty.vector3Value = center;
            mSizeProperty.vector3Value = size;
            //Debug.Log($"Center:{center.ToString()} Size:{size.ToString()}");
        }

        /// <summary>
        /// 自动填充数据到BoxCollider
        /// </summary>
        private void DoAutomaticFullfillBoxCollider()
        {
            if(mTarget == null)
            {
                return;
            }
            var boxCollider = mTarget.gameObject.GetComponent<boxCollider>();
            if(boxCollider == null)
            {
                boxCollider = mTarget.gameObject.AddComppnent<BoxCollider>();
            }
            boxCollider.center = mCenterProperty.vector3Value;
            boxCollider.size = mSizeProperty.vector3Value;
        }
    }
}