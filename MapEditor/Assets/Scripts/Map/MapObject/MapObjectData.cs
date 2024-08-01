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
        /// 埋点位置(地图对象可能删除还原，所以需要逻辑层面记录位置)
        /// </summary>
        [Header("埋点位置")]
        public Vector3 Position;

        /// <summary>
        /// 旋转(地图对象可能删除还原，所以需要逻辑层面记录旋转)
        /// </summary>
        [Header("旋转")]
        public Vector3 Rotation = Vector3.zero;

        /// <summary>
        /// 本地缩放(地图对象可能删除还原，所以需要逻辑层面记录缩放)
        /// </summary>
        [Header("缩放")]
        public Vector3 LocalScale = Vector3.one;

        /// <summary>
        /// 批量操作开关
        /// </summary>
        [Header("批量操作开关")]
        public bool BatchOperationSwitch;

        /// <summary>
        /// GUI关闭开关
        /// </summary>
        [Header("GUI关闭开关")]
        public bool GUISwitchOff = false;

        /// <summary>
        /// 碰撞器中心点
        /// </summary>
        [Header("碰撞器中心点")]
        public Vector3 ColliderCenter = new Vector3(0, 0, 0);

        /// <summary>
        /// 碰撞器大小
        /// </summary>
        [Header("碰撞器大小")]
        public Vector3 ColliderSize = new Vector3(1, 1, 1);

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