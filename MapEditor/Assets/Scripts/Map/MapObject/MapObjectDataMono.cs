/*
 * Description:             MapObjectDataMono.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/20
 */

using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapObjectDataMono.cs
    /// 地图数据挂在脚本
    /// </summary>
    public class MapObjectDataMono : MonoBehaviour
    {
        /// <summary>
        /// 唯一ID(用于标识地图对象配置唯一)
        /// </summary>
        [Header("唯一ID")]
        public int UID;

        /// <summary>
        /// 地图对象类型
        /// </summary>
        [Header("地图对象类型")]
        public MapObjectType ObjectType;

        /// <summary>
        /// 关联配置Id
        /// </summary>
        [Header("关联配置Id")]
        public int ConfId;
    }
}
