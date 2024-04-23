/*
 * Description:             MapObjectData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapObjectData.cs
    /// 地图对象数据
    /// </summary>
    [Serializable]
    public class MapObjectData
    {
        /// <summary>
        /// 唯一Id(用于标识地图对象配置唯一)
        /// </summary>
        [Header("唯一Id")]
        public int UID;

        /// <summary>
        /// 实体对象
        /// </summary>
        [Header("实体对象")]
        public GameObject Go;

        /// <summary>
        /// 埋点位置
        /// </summary>
        [Header("埋点位置")]
        public Vector3 Position;

        /// <summary>
        /// 碰撞器中心点
        /// </summary>
        [Header("碰撞器中心点")]
        public Vector3 ColliderCenter;

        /// <summary>
        /// 碰撞器大小
        /// </summary>
        [Header("碰撞器大小")]
        public Vector3 ColliderSize;

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="go"></param>
        public MapObjectData(int uid, GameObject go)
        {
            UID = uid;
            Go = go;
        }
    }
}