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
        /// 怪物创建半径
        /// </summary>
        [Header("怪物创建半径")]
        public float MonsterCreateRadius = 4;

        /// <summary>
        /// 怪物警戒半径
        /// </summary>
        [Header("怪物警戒半径")]
        public float MonsterActiveRadius = 3;

        public MonsterMapData(int uid) : base(uid)
        {

        }

        public MonsterMapData(int uid, Vector3 position, Vector3 rotation, float monsterCreateRadius, float monsterActiveRadius)
                                : base(uid, position, rotation)
        {
            MonsterCreateRadius = monsterCreateRadius;
            MonsterActiveRadius = monsterActiveRadius;
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
            MonsterCreateRadius = realSourceMapData.MonsterCreateRadius;
            MonsterActiveRadius = realSourceMapData.MonsterActiveRadius;
            return true;
        }
    }
}
