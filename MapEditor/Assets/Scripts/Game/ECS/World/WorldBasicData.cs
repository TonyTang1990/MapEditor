/**
* @ Author: TONYTANG
* @ Create Time: 2025-09-30 16:00:00
* @ Modified by: TONYTANG
* @ Modified time: 2025-09-30 16:00:00
* @ Description:
*
*/

using System;
using System.Collections.Generic;

/// <summary>
/// WorldBasicData.cs
/// 世界基础数据
/// </summary>
public class WorldBasicData
{
    /// <summary>
    /// 世界名
    /// </summary>
    public string WorldName
    {
        get;
        private set;
    }

    /// <summary>
    /// 世界更新类型
    /// </summary>
    public WorldUpdateType WorldUpdateType
    {
        get;
        private set;
    }

    /// <summary>
    /// 逻辑帧数
    /// </summary>
    public int LogicFrameRate
    {
        get;
        private set;
    }

    public WorldBasicData(string worldName, WorldUpdateType worldUpdateType = WorldUpdateType.UPDATE, int logicFrameRate = 20)
    {
        WorldName = worldName;
        WorldUpdateType = worldUpdateType;
        LogicFrameRate = logicFrameRate;
    }
}