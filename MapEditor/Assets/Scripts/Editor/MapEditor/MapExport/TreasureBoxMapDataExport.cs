﻿/*
 * Description:             TreasureBoxMapDataExport.cs
 * Author:                  TONYTANG
 * Create Date:             2025/02/14
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// TreasureBoxMapDataExport.cs
    /// 宝箱数据导出定义
    /// </summary>
    [Serializable]
    public class TreasureBoxMapDataExport : BaseMapDataExport
    {
        public TreasureBoxMapDataExport(MapDataType mapDataType, int confId, Vector3 position, Vector3 rotation)
                                            : base(mapDataType, confId, position, rotation)
        {

        }
    }
}
