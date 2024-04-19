/*
 * Description:             MapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

namespace MapEditor
{
    /// <summary>
    /// MapDataExport.cs
    /// 导出地图相关数据结构定义
    /// </summary>
    public class MapDataExport
    {
        /// <summary>
        /// 地图横向大小
        /// </summary>
        public int Width;

        /// <summary>
        /// 地图纵向大小
        /// </summary>
        public int Height;

        /// <summary>
        /// 地图起始位置
        /// </summary>
        public Vector3 StartPos;

        /// <summary>
        /// 出生点位置列表
        /// </summary>
        public List<PositionExport> BirthPos = new List<PositionExport>;
    }
}
