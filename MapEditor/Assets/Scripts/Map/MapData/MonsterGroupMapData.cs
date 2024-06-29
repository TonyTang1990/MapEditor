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
    /// MonsterGroupMapData.cs
    /// 怪物组地图埋点数据
    /// </summary>
    [Serializable]
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

        /// <summary>
        /// GUI关闭开关
        /// </summary>
        [Header("GUI关闭开关")]
        public bool GUISwitchOff = false;

        public MonsterGroupMapData(int uid) : base(uid)
        {

        }

        public MonsterGroupMapData(int uid, Vector3 position, Vector3 rotation, Vector3? templateLocalPosition = null, Vector3? templateLocalRotation = null)
                                    : base(uid, position, rotation, templateLocalPosition, templateLocalRotation)
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
            var realSourceMapData = sourceMapData as MonsterGroupMapData;
            GroupId = realSourceMapData.GroupId;
            MonsterCreateRadius = realSourceMapData.MonsterCreateRadius;
            MonsterActiveRadius = realSourceMapData.MonsterActiveRadius;
            GUISwitchOff = realSourceMapData.GUISwitchOff;
            return true;
        }
    }
}
