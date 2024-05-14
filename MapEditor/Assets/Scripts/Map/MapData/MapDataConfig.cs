/*
 * Description:             MapDataConfig.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/08
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapDataConfig.cs
    /// 地图埋点数据配置
    /// </summary>
    [Serializable]
    public class MapDataConfig
    {
        /// <summary>
        /// 唯一ID(用于标识地图埋点配置唯一)
        /// </summary>
        [Header("唯一ID")]
        public int UID;

        /// <summary>
        /// 地图数据类型
        /// </summary>
        [Header("地图数据类型")]
        public MapDataType DataType;

        /// <summary>
        /// 关联Id
        /// </summary>
        [Header("关联Id")]
        public int ConfId;

        /// <summary>
        /// 场景球体颜色
        /// </summary>
        [Header("场景球体颜色")]
        public Color SceneSphereColor = Color.red;

        /// <summary>
        /// 初始旋转
        /// </summary>
        [Header("初始旋转")]
        public Vector3 Rotation = Vector3.zero;

        /// <summary>
        /// 描述
        /// </summary>
        [Header("描述")]
        public string Des;

        public MapDataConfig(int uid, MapDataType dataType, Color sceneSphereColor, string des = "")
        {
            UID = uid;
            DataType = dataType;
            SceneSphereColor = sceneSphereColor;
            Des = des;
        }

        /// <summary>
        /// 获取地图埋点配置选项名字
        /// </summary>
        /// <returns></returns>
        public string GetOptionName()
        {
            return $"{UID}({DataType.ToString()})({Des})";
        }
    }
}