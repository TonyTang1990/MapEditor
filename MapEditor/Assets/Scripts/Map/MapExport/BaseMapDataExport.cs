/*
 * Description:             BaseMapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// BaseMapDataExport.cs
    /// 地图埋点数据基类导出定义
    /// </summary>
    [Serializable]
    public class BaseMapDataExport
    {
        /// <summary>
        /// 关联Id
        /// </summary>
        public int ConfId;

        /// <summary>
        /// 位置信息
        /// </summary>
        public Vector3 Position;

        public BaseMapDataExport(int confId, Vector3 position)
        {
            ConfId = confId;
            Position = position;
        }
    }
}
