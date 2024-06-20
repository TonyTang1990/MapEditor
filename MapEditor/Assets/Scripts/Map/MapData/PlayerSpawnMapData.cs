/*
 * Description:             MonsterMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2024/06/19
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// PlayerSpawnMapData.cs
    /// 玩家出生点埋点数据
    /// </summary>
    [Serializable]
    public class PlayerSpawnMapData : MapData
    {
        public PlayerSpawnMapData(int uid) : base(uid)
        {

        }

        public PlayerSpawnMapData(int uid, Vector3 position, Vector3 rotation) : base(uid, position, rotation)
        {

        }
    }
}
