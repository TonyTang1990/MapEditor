/*
 * Description:             TrapMapData.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// TrapMapData.cs
    /// 陷阱埋点数据
    /// </summary>
    [Serializable]
    public class TrapMapData : MapData
    {
        public TrapMapData(int uid) : base(uid)
        {

        }

        public TrapMapData(int uid, Vector3 position, Vector3 rotation) : base(uid, position, rotation)
        {

        }
    }
}
