/*
 * Description:             MonsterGroupMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MonsterGroupMapData.cs
    /// 怪物组地图埋点数据
    /// </summary>
    public class MonsterGroupMapData : MapData
    {
        /// <summary>
        /// 组Id(目前用于怪物分组归属)
        /// </summary>
        [Header("组Id")]
        public int GroupId = 1;

        /// <summary>
        /// 怪物创建半径
        /// </summary>
        [Header("怪物创建半径")]
        public float MonsterCreateRadius = 4;

        /// <summary>
        /// 怪物警戒半径
        /// </summary>
        [Header("怪物警戒半径")]
        public float MonsterActiveRadius = 3;

        public MonsterGroupMapData(int uid) : base(uid)
        {

        }

        public MonsterGroupMapData(int uid, Vector3 position) : base(uid, position)
        {

        }
    }
}
