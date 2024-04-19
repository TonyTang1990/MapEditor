/*
 * Description:             MonsterMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

namespace MapEditor
{
    /// <summary>
    /// MonsterMapData.cs
    /// 怪物地图埋点数据
    /// </summary>
    public class MonsterMapData : MapData
    {
        /// <summary>
        /// 组Id(目前用于怪物分组归属)
        /// </summary>
        [Header("组Id")]
        public int GroupId = 1;

        public MonsterMapData(int uid) : base(uid)
        {

        }

        public MonsterMapData(int uid, Vector3 position) : base(uid, position)
        {

        }
    }
}
