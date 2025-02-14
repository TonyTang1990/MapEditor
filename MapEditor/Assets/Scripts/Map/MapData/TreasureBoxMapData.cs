/*
 * Description:             TreasureBoxMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// TreasureBoxMapData.cs
    /// 宝箱埋点数据
    /// </summary>
    [Serializable]
    public class TreasureBoxMapData : MapData
    {
        public TreasureBoxMapData(int uid) : base(uid)
        {

        }

        public TreasureBoxMapData(int uid, Vector3 position, Vector3 rotation) : base(uid, position, rotation)
        {

        }
    }
}
