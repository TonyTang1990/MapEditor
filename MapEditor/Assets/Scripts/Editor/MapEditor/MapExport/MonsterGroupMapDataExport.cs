/*
 * Description:             BaseMapDynamicExport.cs
 * Author:                  TONYTANG
 * Create Date:             2024/04/24
 */

using System;
using System.Collections.Generic;
using UnityEngine;

namespace MapEditor
{
    /// <summary>
    /// MonsterGroupMapDataExport.cs
    /// 怪物组数据导出定义
    /// </summary>
    [Serializable]
    public class MonsterGroupMapDataExport : BaseMapDataExport
    {
        /// <summary>
        /// 怪物组Id
        /// </summary>
        public int GroupId;

        /// <summary>
        /// 怪物创建半径
        /// </summary>
        public float MonsterCreateRadius;

        /// <summary>
        /// 怪物警戒半径
        /// </summary>
        public float MonsterActiveRadius;

        /// <summary>
        /// 所有归属当前怪物组的怪物导出数据列表
        /// </summary>
        public List<MonsterMapDataExport> AllMonsterMapExportDatas = new List<MonsterMapDataExport>();

        public MonsterGroupMapDataExport(MapDataType mapDataType, int confId, Vector3 position, Vector3 rotation, int groupId, float monsterCreateRadius, float monsterActiveRadius)
                                            : base(mapDataType, confId, position, rotation)
        {
            GroupId = groupId;
            MonsterCreateRadius = monsterCreateRadius;
            MonsterActiveRadius = monsterActiveRadius;
        }

        /// <summary>
        /// 添加怪物导出数据
        /// </summary>
        /// <param name="monsterMapDataExport"></param>
        /// <returns></returns>
        public bool AddMonsterMapExportData(MonsterMapDataExport monsterMapDataExport)
        {
            AllMonsterMapExportDatas.Add(monsterMapDataExport);
            return true;
        }

        /// <summary>
        /// 移除怪物导出数据
        /// </summary>
        /// <param name="monsterMapDataExport"></param>
        /// <returns></returns>
        public bool RemoveMonsterMapExportData(MonsterMapDataExport monsterMapDataExport)
        {
            return AllMonsterMapExportDatas.Remove(monsterMapDataExport);
        }
    }
}
