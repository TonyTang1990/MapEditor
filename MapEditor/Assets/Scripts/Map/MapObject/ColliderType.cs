/*
 * Description:             ColliderType.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/26
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// ColliderType.cs
    /// 碰撞器类型枚举
    /// </summary>
    public enum ColliderType
    {
        BOX = 0,           // 矩形碰撞器
        SPHERE = 1,        // 圆形碰撞器
    }
}