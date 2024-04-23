/*
 * Description:             PositionExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// PositionExport.cs
    /// 位置导出数据定义
    /// </summary>
    public class PositionExport
    {
        /// <summary>
        /// 位置信息
        /// </summary>
        public Vector3 Position;

        public PositionExport(Vector3 position)
        {
            Position = position;
        }
    }
}
