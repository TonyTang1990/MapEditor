/*
 * Description:             MeshUtilities.cs
 * Author:                  TangHuan
 * Create Date:             2021/03/25
 */

using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Mesh静态工具类
/// </summary>
public static class MeshUtilities
{
    /// <summary>
    /// 创建Mesh
    /// </summary>
    /// <param name="vertices">存储顶点的列表</param>
    /// <param name="normal"></param>
    /// <returns></returns>
    public static Mesh CreateMesh(List<Vector3> vertices)
    {
        Mesh mesh = new Mesh();
        if (vertices != null && vertices.Count >= 3)
        {
            int[] triangles;
            // 为了闭合，这里多画一个三角形
            int triangleAmount = vertices.Count - 1;
            triangles = new int[3 * triangleAmount];

            //根据三角形的个数，来计算绘制三角形的顶点顺序（索引）   
            //顺序必须为顺时针或者逆时针      
            for (int i = 0; i < triangleAmount - 1; i++)
            {
                triangles[3 * i] = 0;//固定第一个点      
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = i + 2;
            }
            // 最后一个闭合三角形单独处理
            triangles[3 * (triangleAmount - 1)] = 0;//固定第一个点      
            triangles[3 * (triangleAmount - 1) + 1] = vertices.Count - 1;
            triangles[3 * (triangleAmount - 1) + 2] = 1;

            mesh.SetVertices(vertices);
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
        }
        return mesh;
    }
}