/*
 * Description:             MonsterGroupMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MapData.cs
    /// 地图数据买点数据
    /// </summary>
    [Serializable]
    public class MapData
    {
        /// <summary>
        /// 唯一Id(用于标识地图对象配置唯一)
        /// </summary>
        [Header("唯一Id")]
        public int UID;

        /// <summary>
        /// 埋点位置
        /// </summary>
        [Header("埋点位置")]
        public Vector3 Position;

        /// <summary>
        /// 埋点旋转
        /// </summary>
        [Header("埋点旋转")]
        public Vector3 Rotation;

        /// <summary>
        /// 批量操作开关
        /// </summary>
        [Header("批量操作开关")]
        public bool BatchOperationSwitch;

        public MapData(int uid)
        {
            UID = uid;
        }

        public MapData(int uid, Vector3 position, Vector3 rotation)
        {
            UID = uid;
            Position = position;
            Rotation = rotation;
        }

        /// <summary>
        /// 复制自定义数据(相同类型才允许复制数据)
        /// </summary>
        /// <param name="sourceMapData"></param>
        /// <returns></returns>
        public virtual bool CopyCustomData(MapData sourceMapData)
        {
            var selfType = GetType();
            var sourceType = sourceMapData.GetType();
            if(selfType != sourceType)
            {
                Debug.LogError($"自身类型:{selfType.Name}和原数据类型:{sourceType.Name}不一致，复制自定义数据失败！");
                return false;
            }
            UID = sourceMapData.UID;
            Position = sourceMapData.Position;
            Rotation = sourceMapData.Rotation;
            BatchOperationSwitch = sourceMapData.BatchOperationSwitch;
            return true;
        }
    }
}