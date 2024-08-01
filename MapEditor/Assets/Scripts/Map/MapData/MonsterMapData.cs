/*
 * Description:             MonsterMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MonsterMapData.cs
    /// 怪物地图埋点数据
    /// </summary>
    [Serializable]
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

        public MonsterMapData(int uid, Vector3 position, Vector3 rotation)
                                : base(uid, position, rotation)
        {

        }

        /// <summary>
        /// 复制自定义数据
        /// </summary>
        /// <param name="sourceMapData"></param>
        /// <returns></returns>
        public override bool CopyCustomData(MapData sourceMapData)
        {
            if(!base.CopyCustomData(sourceMapData))
            {
                return false;
            }
            var realSourceMapData = sourceMapData as MonsterMapData;
            GroupId = realSourceMapData.GroupId;
            return true;
        }
    }
}
